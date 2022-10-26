using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_SIMULATED_ANNEALING : MonoBehaviour
{

    GameFlowFramework_Environment env;

    void Start()
    {
        env = GetComponentInParent<GameFlowFramework_Environment>();
    }

    public void Init()
    {
        env.SpawnStartingPatches(); //the first 4 patches to show up
        SIMULATED_ANNEALING();
    }

    public void SIMULATED_ANNEALING()
    {
        if (env.enableDebugLog)
        {
            Debug.Log("-----SIMULATED_ANNEALING SETTINGS [START]-----");
            Debug.Log("Patch Amount: " + env.patchAmount);
            Debug.Log("Total Patch Difficulty: " + env.totalPatchDifficulty);
            Debug.Log("Iterations: " + env.iterations);
            Debug.Log("Easy Patch Difficulty: " + env.easyPatchDif);
            Debug.Log("Medium Patch Difficulty: " + env.mediumPatchDif);
            Debug.Log("Hard Patch Difficulty: " + env.hardPatchDif);
            Debug.Log("Extreme Patch Difficulty: " + env.extremePatchDif);
            Debug.Log("-----SIMULATED_ANNEALING SETTINGS [END]-----");
        }

        //create array of empty patches
        env.InitEmptyPatches();

        //keep randomly adding/removing patches until "iterations" is reached
        for (int i = 0; i < env.iterations; i++)
        {
            int randIndex = Random.Range(0, env.patchAmount); //random index
            int randType = Random.Range(1, 5); //random type

            //compute current difficulty level
            float currentPatchDifficulty = GetCurrentDifficulty();

            //compute new proposed difficulty level
            float newPatchDifficulty = currentPatchDifficulty;
            if (env.patchList[randIndex].type != 0) { newPatchDifficulty -= PatchGetDifficulty(randIndex); }
            newPatchDifficulty += TypeGetDifficulty(randType);

            //simulated annealing algorithm components
            float t = env.iterations / (i + 1);
            float e_new = Mathf.Abs(env.totalPatchDifficulty - newPatchDifficulty);
            float e = Mathf.Abs(env.totalPatchDifficulty - currentPatchDifficulty);

            if (env.enableDebugLog) { Debug.Log("Patch Difficulty Difference = " + e); }
            string passed; //only used for debugging

            //accept/reject conditions
            if (e_new < e || Mathf.Exp(-(e_new - e) / t) >= Random.Range(0f, 1f))
            {
                //accept the change
                passed = "Y";
                ReplacePatch(randType, randIndex);
            }
            else
            {
                //reject the change
                passed = "N";
            }

            if (env.enableDebugLog)
            {
                Debug.Log("i = " + i + ", " + passed + ", EXP = "
                    + Mathf.Exp(-(e_new - e) / t) + ", e_new = "
                    + e_new + ", e = " + e);
            }
        }

        //debugging log
        if (env.enableDebugLog) { PatchResultDebug(); }
    }

    /// <summary>
    /// Check conditions to spawn a patch from the patch list, then act accordingly.
    /// </summary>
    public void SpawnWhenEnter_PatchListBased()
    {
        //check if we should spawn a question given that the player had just entered a new patch
        env.referencer.GameFlowFramework_Questions.TriggerPatchCheck(env.flowObjects_totalRegularSpawned);

        //if the patch list has run out of patches to be spawned
        if (env.patchList.Count == 0)
        {
            env.FinishPatch(); //spawn the final patch
            return;
        }

        env.SpawnPatchFromPatchList(); //spawn patch
    }

    /// <summary>
    /// Returns the current sum of all the difficulties in the patch list
    /// </summary>
    public float GetCurrentDifficulty()
    {
        float currentPatchDifficulty = 0;
        for (int j = 0; j < env.patchList.Count; j++)
        {
            currentPatchDifficulty += env.patchList[j].difficulty;
        }
        return currentPatchDifficulty;
    }

    /// <summary>
    /// Removes a patch from the patchList.
    /// </summary>
    ///<param name="index">The index of the patch list.</param>
    public void RemovePatch(int index)
    {
        env.patchList[index].type = 0;
        PatchSetDifficulty(index);
    }

    /// <summary>
    /// Adds a patch to the patchList.
    /// </summary>
    ///<param name="patchType">Patch Type 0: Empty, 1: Easy, 2: Medium, 3: Hard, 4: Extreme.</param>
    ///<param name="index">The index of the patch list.</param>
    public void AddPatch(int patchType, int index)
    {
        if (env.patchList[index].type != 0) //if the index is not empty then don't add
        {
            return;
        }
        env.patchList[index].type = patchType;
        PatchSetDifficulty(index);
    }

    /// <summary>
    /// Replaces a patch in the patchList.
    /// </summary>
    ///<param name="patchType">Patch Type 0: Empty, 1: Easy, 2: Medium, 3: Hard, 4: Extreme.</param>
    ///<param name="index">The index of the patch list.</param>
    public void ReplacePatch(int patchType, int index)
    {
        RemovePatch(index);
        AddPatch(patchType, index);
    }

    /// <summary>
    /// Returns the difficulty of a patch from the patch list by index.
    /// </summary>
    ///<param name="index">The index of the patch list.</param>
    public float PatchGetDifficulty(int index)
    {
        switch (env.patchList[index].type)
        {
            case 0: //empty patch
                return 0;
            case 1: //easy patch
                return env.easyPatchDif;
            case 2: //medium patch
                return env.mediumPatchDif;
            case 3: //hard patch
                return env.hardPatchDif;
            case 4: //extreme patch
                return env.extremePatchDif;
            default:
                Debug.Log("ERROR: Invalid type for PatchGetDifficulty()");
                return -1;
        }
    }

    /// <summary>
    /// Returns the difficulty set for the patch type.
    /// </summary>
    ///<param name="type">Patch Type 0: Empty, 1: Easy, 2: Medium, 3: Hard, 4: Extreme.</param>
    public float TypeGetDifficulty(int type)
    {
        switch (type)
        {
            case 0: //empty patch
                return 0;
            case 1: //easy patch
                return env.easyPatchDif;
            case 2: //medium patch
                return env.mediumPatchDif;
            case 3: //hard patch
                return env.hardPatchDif;
            case 4: //extreme patch
                return env.extremePatchDif;
            default:
                Debug.Log("ERROR: Invalid type for GetDifficulty()");
                return -1;
        }
    }

    /// <summary>
    /// Sets the difficulty of a patch from the patch list by index based on its patch type.
    /// </summary>
    ///<param name="index">The index of the patch list.</param>
    public void PatchSetDifficulty(int index)
    {
        switch (env.patchList[index].type)
        {
            case 0: //empty patch
                env.patchList[index].difficulty = 0;
                break;
            case 1: //easy patch
                env.patchList[index].difficulty = env.easyPatchDif;
                break;
            case 2: //medium patch
                env.patchList[index].difficulty = env.mediumPatchDif;
                break;
            case 3: //hard patch
                env.patchList[index].difficulty = env.hardPatchDif;
                break;
            case 4: //extreme patch
                env.patchList[index].difficulty = env.extremePatchDif;
                break;
            default:
                Debug.Log("ERROR: Invalid type for PatchSetDifficulty()");
                break;
        }
    }

    /// <summary>
    /// Details of the patches spawned.
    /// </summary>
    public void PatchResultDebug()
    {
        Debug.Log("-----THE PATCH LIST [START]-----");
        for (int i = 0; i < env.patchList.Count; i++)
        {
            Debug.Log("[" + i + "]: type " + env.patchList[i].type + ", diff " + env.patchList[i].difficulty);
        }
        Debug.Log("-----THE PATCH LIST [END]-----");

        Debug.Log("-----THE PATCH LIST FOR PLOTTING [START]-----");
        for (int i = 0; i < env.patchList.Count; i++)
        {
            Debug.Log("PLOT: " + env.patchList[i].difficulty);
        }
        Debug.Log("-----THE PATCH LIST FOR PLOTTING [END]-----");
    }
}
