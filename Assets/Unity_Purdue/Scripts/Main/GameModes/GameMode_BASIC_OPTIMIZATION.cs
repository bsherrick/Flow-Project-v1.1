using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_BASIC_OPTIMIZATION : MonoBehaviour
{

    GameFlowFramework_Environment env;

    void Start()
    {
        env = GetComponentInParent<GameFlowFramework_Environment>();
    }

    public void Init()
    {
        env.SpawnStartingPatches(); //the first 4 patches to show up
        BASIC_OPTIMIZATION();
    }

    void BASIC_OPTIMIZATION()
    {
        if (env.enableDebugLog)
        {
            Debug.Log("-----BASIC_OPTIMIZATION SETTINGS [START]-----");
            Debug.Log("Patch Amount: " + env.patchAmount);
            Debug.Log("Total Patch Difficulty: " + env.totalPatchDifficulty);
            Debug.Log("Iterations: " + env.iterations);
            Debug.Log("Easy Patch Difficulty: " + env.easyPatchDif);
            Debug.Log("Medium Patch Difficulty: " + env.mediumPatchDif);
            Debug.Log("Hard Patch Difficulty: " + env.hardPatchDif);
            Debug.Log("Extreme Patch Difficulty: " + env.extremePatchDif);
            Debug.Log("-----BASIC_OPTIMIZATION SETTINGS [END]-----");
        }

        //find the most difficult patch
        int maxDiffType = 0;
        float maxDiff = 0;
        if (env.easyPatchDif > maxDiff) { maxDiff = env.easyPatchDif; maxDiffType = 1; }
        if (env.mediumPatchDif > maxDiff) { maxDiff = env.mediumPatchDif; maxDiffType = 2; }
        if (env.hardPatchDif > maxDiff) { maxDiff = env.hardPatchDif; maxDiffType = 3; }
        if (env.extremePatchDif > maxDiff) { maxDiff = env.extremePatchDif; maxDiffType = 4; }

        //create array of empty patches
        env.InitEmptyPatches();

        //keep randomly adding/removing patches until "iterations" is reached
        bool useReplace = false; //if all indexes are filled we will replace instead of add patches
        for (int i = 0; i < env.iterations; i++)
        {
            float currentPatchDifficulty = env.gameMode_SimulatedAnnealingScript.GetCurrentDifficulty(); //compute current difficulty level
            int randIndex = Random.Range(0, env.patchAmount); //random index

            //see if we are above or below the user-defined difficulty level
            if (env.enableDebugLog) { Debug.Log("Patch Difficulty Difference = " + Mathf.Abs(currentPatchDifficulty - env.totalPatchDifficulty)); }
            if (currentPatchDifficulty > env.totalPatchDifficulty) //ABOVE
            {
                env.gameMode_SimulatedAnnealingScript.RemovePatch(randIndex); //remove a patch
            }
            else if (currentPatchDifficulty < env.totalPatchDifficulty) //BELOW
            {
                if (!useReplace)
                {
                    int randType = Random.Range(1, 5); //random type
                    env.gameMode_SimulatedAnnealingScript.AddPatch(randType, randIndex); //add a patch
                }
                else
                {

                    env.gameMode_SimulatedAnnealingScript.ReplacePatch(maxDiffType, randIndex); //replace a patch with the most difficult patch
                }
            }
            else //JUST RIGHT (happens very rarely)
            {
                break; //we have reached the max optimization
            }

            //check if all indexes are filled
            if (!useReplace)
            {
                for (int j = 0; j < env.patchAmount; j++)
                {
                    if (env.patchList[j].type == 0) //found free index
                    {
                        break;
                    }
                    if (j == env.patchAmount - 1) //reached the end
                    {
                        useReplace = true;
                    }
                }
            }
        }

        //debugging log
        if (env.enableDebugLog) { env.gameMode_SimulatedAnnealingScript.PatchResultDebug(); }
    }
}
