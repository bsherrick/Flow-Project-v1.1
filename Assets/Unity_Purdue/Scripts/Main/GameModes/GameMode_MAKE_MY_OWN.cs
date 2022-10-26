using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_MAKE_MY_OWN : MonoBehaviour
{

    GameFlowFramework_Environment env;

    bool done;
    public int uiIndex;

    void Start()
    {
        env = GetComponentInParent<GameFlowFramework_Environment>();
        done = false;
        uiIndex = 0;
        //env.referencer.GameFlowFramework_Web.DownloadQuestions();
    }

    public void Init()
    {
        env.patchList = new List<Patch>();

        //simulated annealing parameters
        env.patchAmount = env.patchSeed.Length;
        env.iterations = env.annealingIterations;
        env.easyPatchDif = 1f;
        env.mediumPatchDif = 2f;
        env.hardPatchDif = 3f;
        env.extremePatchDif = 4f;

        env.SpawnStartingPatches(); //the first four patches to show up

        env.numIterations = 0; //start from 0 iterations

        AddToPatchListFromSeed(); //generate patch list

        env.difficultyGoal = GetCurrentDifficultyFromSeed();
    }

    /// <summary>
    /// Check conditions to spawn a patch from the patch list, then act accordingly.
    /// </summary>
    public void SpawnWhenEnter_PatchListBasedWithIterations()
    {
        //check if we should spawn a question given that the player had just entered a new patch
        env.referencer.GameFlowFramework_Questions.TriggerPatchCheck(env.flowObjects_totalRegularSpawned);

        if (done) //if done flag is set, do nothing more
        {
            return;
        }

        if (env.patchList.Count == 0) //if the patch list has run out of patches
        {
            if (env.numIterations >= env.totalIterations && !env.isEndless) //if all iterations are completed and endless mode is off
            {
                env.FinishPatch(); //spawn finishing patch
                done = true; //set done flag
                return;
            }
            NewIteration(); //start a new iteration
        }
        env.SpawnPatchFromPatchList(); //spawn patch
    }

    /// <summary>
    /// Prepare to generate a new patch list, then act accordingly.
    /// </summary>
    void NewIteration()
    {
        if (env.diffIsActive) //if difficulty is set to active
        {
            if (env.usePercentage) //if difficulty should increase/decrease by a percentage
            {
                env.difficultyGoal *= env.difficultyConstant;
            }
            else //else difficuly should increase/decrease by a constant value
            {
                env.difficultyGoal += env.difficultyConstant;
            }
            env.totalPatchDifficulty = env.difficultyGoal;
            if (env.enableDebugLog || env.summarizeAnnealing)
            {
                Debug.Log("---BEFORE SIMULATED ANNEALING [START]---");
                Debug.Log("DIFF current: " + GetCurrentDifficultyFromSeed());
                Debug.Log("DIFF goal: " + env.difficultyGoal);
                Debug.Log("DIFF constant: " + env.difficultyConstant);
                Debug.Log("---BEFORE SIMULATED ANNEALING [END]---");
            }

            AddToPatchListFromSeed();
            env.gameMode_SimulatedAnnealingScript.SIMULATED_ANNEALING(); //run the simulated annealing algorithm
            UpdateSeed();

            if (env.enableDebugLog || env.summarizeAnnealing)
            {
                Debug.Log("---AFTER SIMULATED ANNEALING [START]---");
                Debug.Log("DIFF current: " + GetCurrentDifficultyFromSeed());
                Debug.Log("DIFF goal: " + env.difficultyGoal);
                Debug.Log("DIFF constant: " + env.difficultyConstant);
                Debug.Log("---AFTER SIMULATED ANNEALING [END]---");
            }
        }
        else //else if difficulty is inactive
        {
            AddToPatchListFromSeed();
        }
    }

    /// <summary>
    /// Returns the current sum of all the difficulties from the seed
    /// </summary>
    public float GetCurrentDifficultyFromSeed()
    {
        float currentPatchDifficulty = 0;
        for (int i = 0; i < env.patchSeed.Length; i++)
        {
            if (env.patchSeed[i] + "" == "0")
            {
                currentPatchDifficulty += env.gameMode_SimulatedAnnealingScript.TypeGetDifficulty(0);
            }
            else if (env.patchSeed[i] + "" == "1")
            {
                currentPatchDifficulty += env.gameMode_SimulatedAnnealingScript.TypeGetDifficulty(1);
            }
            else if (env.patchSeed[i] + "" == "2")
            {
                currentPatchDifficulty += env.gameMode_SimulatedAnnealingScript.TypeGetDifficulty(2);
            }
            else if (env.patchSeed[i] + "" == "3")
            {
                currentPatchDifficulty += env.gameMode_SimulatedAnnealingScript.TypeGetDifficulty(3);
            }
            else if (env.patchSeed[i] + "" == "4")
            {
                currentPatchDifficulty += env.gameMode_SimulatedAnnealingScript.TypeGetDifficulty(4);
            }
            else
            {
                Debug.Log("ERROR! BAD SEED!");
            }
        }
        return currentPatchDifficulty;
    }

    /// <summary>
    /// Generate a new patch list from seed.
    /// </summary>
    public void AddToPatchListFromSeed()
    {
        if (done)
        {
            return;
        }

        //grow our seed into a tree!
        for (int i = 0; i < env.patchSeed.Length; i++)
        {
            Patch patch = new Patch();

            if (env.patchSeed[i] + "" == "0")
            {
                patch.type = 0;
                patch.difficulty = 0;
            }
            else if (env.patchSeed[i] + "" == "1")
            {
                patch.type = 1;
                patch.difficulty = env.easyPatchDif;
            }
            else if (env.patchSeed[i] + "" == "2")
            {
                patch.type = 2;
                patch.difficulty = env.mediumPatchDif;
            }
            else if (env.patchSeed[i] + "" == "3")
            {
                patch.type = 3;
                patch.difficulty = env.hardPatchDif;
            }
            else if (env.patchSeed[i] + "" == "4")
            {
                patch.type = 4;
                patch.difficulty = env.extremePatchDif;
            }
            else
            {
                Debug.Log("ERROR! The Seed Did Not Grow Well!");
            }
            env.patchList.Add(patch);
        }
        env.numIterations++; //update number of iterations
    }

    /// <summary>
    /// Update the seed after having gone trhough simulated annealing.
    /// </summary>
    void UpdateSeed()
    {
        string newSeed = "";
        for (int i = 0; i < env.patchList.Count; i++)
        {
            newSeed += env.patchList[i].type.ToString();
        }
        if (env.enableDebugLog || env.summarizeAnnealing)
        {
            Debug.Log("Seed BEFORE: " + env.patchSeed);
            Debug.Log("Seed AFTER: " + newSeed);
        }
        env.patchSeed = newSeed;
    }

    //Below are UI functions for the MAKE_MY_OWN gamemode setting screen

    /// <summary>
    /// Set parameters for MAKE_MY_OWN based on preset number.
    /// </summary>
    ///<param name="i">The preset number.</param>
    public void SetPreset(int i)
    {
        env.referencer.CanvasScript.input4PresetName.text = env.presets[i].name;
        env.referencer.CanvasScript.input4PresetIndex.text = (i + 1) + " of " + env.presets.Length;

        env.referencer.CanvasScript.input4Seed.SetTextWithoutNotify(env.presets[i].seed);
        env.referencer.CanvasScript.input4Iterations.SetTextWithoutNotify(env.presets[i].iterations.ToString());
        env.referencer.CanvasScript.input4Endless.SetIsOnWithoutNotify(env.presets[i].isEndless);
        env.referencer.CanvasScript.input4Diff.SetTextWithoutNotify(env.presets[i].difficulty.ToString());
        env.referencer.CanvasScript.input4DiffActive.SetIsOnWithoutNotify(env.presets[i].diffIsActive);
        env.referencer.CanvasScript.input4UsePercentage.SetIsOnWithoutNotify(env.presets[i].usePercentage);

        env.referencer.CanvasScript.Gamemode4GetValues();

        //env.referencer.CanvasScript.Gamemode4GetValues();
        /*
        env.referencer.CanvasScript.input4PresetName.text = env.presets[i].name;
        env.referencer.CanvasScript.input4Seed.text = env.presets[i].seed;
        env.referencer.CanvasScript.input4Iterations.text = env.presets[i].iterations.ToString();
        env.referencer.CanvasScript.input4PresetIndex.text = (i + 1) + " of " + env.presets.Length;
        env.referencer.CanvasScript.input4Endless.isOn = env.presets[i].isEndless;
        env.referencer.CanvasScript.input4Diff.text = env.presets[i].difficulty.ToString();
        env.referencer.CanvasScript.input4DiffActive.isOn = env.presets[i].diffIsActive;
        env.referencer.CanvasScript.input4UsePercentage.isOn = env.presets[i].usePercentage;
        env.referencer.CanvasScript.Gamemode4GetValues();
        */
    }

    /// <summary>
    /// Set values for MAKE_MY_OWN based on preset number.
    /// </summary>
    ///<param name="i">The preset number.</param>
    public void SetValue()
    {
        int i = uiIndex;

        string seed = env.referencer.CanvasScript.input4Seed.text;

        string pureSeed = "";
        int r;

        for (int j = 0; j < seed.Length; j++)
        {
            if (seed[j] + "" == "0" || seed[j] + "" == "1" || seed[j] + "" == "2"
                || seed[j] + "" == "3" || seed[j] + "" == "4")
            {
                pureSeed += (seed[j] + "");
            }
            else
            {
                r = Random.Range(0, 5);
                pureSeed += (r + "");
            }
        }
        env.referencer.CanvasScript.input4Seed.text = pureSeed;

        env.presets[i].seed = env.referencer.CanvasScript.input4Seed.text;
        env.presets[i].iterations = int.Parse(env.referencer.CanvasScript.input4Iterations.text);
        env.presets[i].isEndless = env.referencer.CanvasScript.input4Endless.isOn;
        env.presets[i].difficulty = float.Parse(env.referencer.CanvasScript.input4Diff.text);
        env.presets[i].diffIsActive = env.referencer.CanvasScript.input4DiffActive.isOn;
        env.presets[i].usePercentage = env.referencer.CanvasScript.input4UsePercentage.isOn;
    }

    /// <summary>
    /// For browsing presets.
    /// </summary>
    ///<param name="b">TRUE for NEXT, FALSE for PREV</param>
    public void NextQuestion(bool b)
    {
        if (b)
        {
            //next
            if (uiIndex >= env.presets.Length - 1) { uiIndex = 0; } else { uiIndex += 1; }

        }
        else
        {
            //prev
            if (uiIndex <= 0) { uiIndex = env.presets.Length - 1; } else { uiIndex -= 1; }
        }

        SetPreset(uiIndex);
    }
}
