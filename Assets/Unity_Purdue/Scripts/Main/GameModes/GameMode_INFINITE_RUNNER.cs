using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode_INFINITE_RUNNER : MonoBehaviour
{

    GameFlowFramework_Environment env;

    /*The lower and upper bounds of the patches based on their priority relative to the total priority*/
    /*see CalculateBounds()*/
    int emptyLB = 0, emptyUB = 0;
    int easyLB = 0, easyUB = 0;
    int mediumLB = 0, mediumUB = 0;
    int hardLB = 0, hardUB = 0;
    int extremeLB = 0, extremeUB = 0;
    int totalPriority;

    void Start()
    {
        env = GetComponentInParent<GameFlowFramework_Environment>();
    }

    public void Init()
    {
        env.SpawnStartingPatches(); //the first 4 patches to show up
        CalculateBounds(); //calculate the patch priorities
    }

    void CalculateBounds()
    {
        //calculate the total priority
        totalPriority =
            + env.emptyPatchSpawnPriority
            + env.easyPatchSpawnPriority
            + env.mediumPatchSpawnPriority
            + env.hardPatchSpawnPriority
            + env.extremePatchSpawnPriority;

        //put them in an array
        int[] priorities = {
            env.emptyPatchSpawnPriority,
            env.easyPatchSpawnPriority,
            env.mediumPatchSpawnPriority,
            env.hardPatchSpawnPriority,
            env.extremePatchSpawnPriority };

        //the bounds of each patch type
        int[] emptyB = { 0, 0 };
        int[] easyB = { 0, 0 };
        int[] mediumB = { 0, 0 };
        int[] hardB = { 0, 0 };
        int[] extremeB = { 0, 0 };

        //put all bounds in an array
        int[][] bounds = { emptyB, easyB, mediumB, hardB, extremeB };

        int priority_curr = 0;

        //incrementally set the bounds to each of the patches
        for (int i = 0; i < 5; i++)
        {
            if (priorities[i] > 0)
            {
                bounds[i][0] = priority_curr + 1;
                bounds[i][1] = bounds[i][0] + priorities[i] - 1;
                priority_curr += priorities[i];
            }
        }

        //assign bounds
        emptyLB = bounds[0][0]; emptyUB = bounds[0][1];
        easyLB = bounds[1][0]; easyUB = bounds[1][1];
        mediumLB = bounds[2][0]; mediumUB = bounds[2][1];
        hardLB = bounds[3][0]; hardUB = bounds[3][1];
        extremeLB = bounds[4][0]; extremeUB = bounds[4][1];

        if (env.enableDebugLog)
        {
            Debug.Log("-----SPAWN PRIORITY CHECK START-----");
            Debug.Log("Total Priority: " + totalPriority);
            Debug.Log("emptyLB: " + emptyLB);
            Debug.Log("emptyUB: " + emptyUB);
            Debug.Log("easyLB: " + easyLB);
            Debug.Log("easyUB: " + easyUB);
            Debug.Log("mediumLB: " + mediumLB);
            Debug.Log("mediumUB: " + mediumUB);
            Debug.Log("hardLB: " + hardLB);
            Debug.Log("hardUB: " + hardUB);
            Debug.Log("extremeLB: " + extremeLB);
            Debug.Log("extremeUB: " + extremeUB);
            Debug.Log("-----SPAWN PRIORITY CHECK END-----");
        }
    }

    public void SpawnWhenEnter_PriorityBased()
    {
        GameObject p;

        //check if we should spawn a question given that the player had just entered a new patch
        env.referencer.GameFlowFramework_Questions.TriggerPatchCheck(env.flowObjects_totalRegularSpawned);

        //Calculate what patch to spawn based on their priorities (see CalculateBounds())
        int chosenPatch = 0;
        int chosenVariation = 0;
        chosenPatch = Random.Range(1, totalPriority + 1);
        if (chosenPatch >= emptyLB && chosenPatch <= emptyUB) //spawn an empty patch
        {
            chosenVariation = Random.Range(0, env.patchEmpty.Length); //pick variation
            p = Instantiate(env.patchEmpty[chosenVariation], env.currentSpawnLocation, Quaternion.identity); //spawn
        }
        else if (chosenPatch >= easyLB && chosenPatch <= easyUB) //spawn an easy patch
        {
            chosenVariation = Random.Range(0, env.patchEasy.Length); //pick variation
            p = Instantiate(env.patchEasy[chosenVariation], env.currentSpawnLocation, Quaternion.identity); //spawn
        }
        else if (chosenPatch >= mediumLB && chosenPatch <= mediumUB) //spawn a medium patch
        {
            chosenVariation = Random.Range(0, env.patchMedium.Length); //pick variation
            p = Instantiate(env.patchMedium[chosenVariation], env.currentSpawnLocation, Quaternion.identity); //spawn
        }
        else if (chosenPatch >= hardLB && chosenPatch <= hardUB) //spawn a hard patch
        {
            chosenVariation = Random.Range(0, env.patchHard.Length); //pick variation
            p = Instantiate(env.patchHard[chosenVariation], env.currentSpawnLocation, Quaternion.identity); //spawn
        }
        else if (chosenPatch >= extremeLB && chosenPatch <= extremeUB) //spawn an extreme patch
        {
            chosenVariation = Random.Range(0, env.patchExtreme.Length); //pick variation
            p = Instantiate(env.patchExtreme[chosenVariation], env.currentSpawnLocation, Quaternion.identity); //spawn
        }
        else
        {
            Debug.Log("SpawnWhenEnter(): Critical Error while choosing patches!!!");
            return;
        }

        env.NewFlow(p, 1);
    }
}
