using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFlowFramework_Environment : MonoBehaviour
{
    //Default Values
    /****************************************************************************/
    //General
    static bool enableDebugLog_Default = false;
    static bool showTimer_Default = true;
    static bool timerIsCountdown_Default = false;
    static float timerStartingTime_Default = 0;
    static bool showHealth_Default = true;
    static bool showHits_Default = false;
    static bool showScore_Default = true;
    static Vector3Int startingPatchPosition_Default = new Vector3Int(0, 0, 0);
    static int patchLength_Default = 20;
    static bool finalPatchSpawned_Default = false;
    //GameModes:
    static bool gameMode0_Default = false; //Jumper (old)
    static bool gameMode1_Default = true; //1:INFINITE_RUNNER
    static bool gameMode2_Default = false; //2:BASIC_OPTIMIZATION
    static bool gameMode3_Default = false; //3:SIMULATED_ANNEALING
    static bool gameMode4_Default = false; //4:MAKE_MY_OWN
    //GameMode Jumper (old) Settings
    static int minDistance_Default = 1;
    static float spawnChance_Default = 60;
    //GameMode 1:INFINITE_RUNNER Settings
    static int emptyPatchSpawnPriority_Default = 1;
    static int easyPatchSpawnPriority_Default = 1;
    static int mediumPatchSpawnPriority_Default = 1;
    static int hardPatchSpawnPriority_Default = 1;
    static int extremePatchSpawnPriority_Default = 1;
    //GameMode 2:BASIC_OPTIMIZATION Settings and
    //GameMode 3:SIMULATED_ANNEALING Settings
    static int patchAmount_Default = 30;
    static float totalPatchDifficulty_Default = 35;
    static int iterations_Default = 300;
    static float easyPatchDif_Default = 1;
    static float mediumPatchDif_Default = 2;
    static float hardPatchDif_Default = 3;
    static float extremePatchDif_Default = 4;
    //GameMode 4:MAKE_MY_OWN Settings
    static string patchSeed_Default = "";
    static int totalIterations_Default = 1;
    static int numIterations_Default = 0;
    static bool isEndless_Default = false;
    static bool diffIsActive_Default = false;
    static bool usePercentage_Default = false;
    static int annealingIterations_Default = 100;
    static float difficultyGoal_Default = 0;
    static float difficultyConstant_Default = 0;
    static float difficultyCurrent_Default = 0;
    static bool summarizeAnnealing_Default = false;

    /****************************************************************************/

    [HideInInspector]
    public bool gameMode0 = gameMode0_Default;
    [HideInInspector]
    public bool gameMode1 = gameMode1_Default;
    [HideInInspector]
    public bool gameMode2 = gameMode2_Default;
    [HideInInspector]
    public bool gameMode3 = gameMode3_Default;
    [HideInInspector]
    public bool gameMode4 = gameMode4_Default;

    [Header("General:")]
    [Header("__________________________________________________________________")]
    [Tooltip("Canvas script attached to the main canvas.")] public CanvasScript canvas;
    [Tooltip("Show debugging log?")] public bool enableDebugLog = enableDebugLog_Default;
    [Tooltip("Show player Health?")] public bool showHealth = showHealth_Default;
    [Tooltip("Show hits (everytime an obstacle is collided)?")] public bool showHits = showHits_Default;
    [Tooltip("Show player Score?")] public bool showScore = showScore_Default;
    [Tooltip("Show timer?")] public bool showTimer = showTimer_Default;
    [Tooltip("Does the timer countdown?")] public bool timerIsCountdown = timerIsCountdown_Default;
    [Tooltip("Starting time in seconds of the timer.")] public float timerStartingTime = timerStartingTime_Default;
    [Tooltip("The position of the starting patch.")] public Vector3Int startingPatchPosition = startingPatchPosition_Default;
    [Tooltip("Patch length, used for calculating position.")] public int patchLength = patchLength_Default;

    [Header("Patch Prefabs (to be spawned in-game):")]
    [Header("__________________________________________________________________")]
    [Tooltip("First patch to spawn, must be a Genesis patch (patches that lack trigger functionalities).")] public GameObject genesisPatch;
    [Tooltip("1st Normal Patch to show up.")] public GameObject initPatch1;
    [Tooltip("2nd Normal Patch to show up.")] public GameObject initPatch2;
    [Tooltip("3rd Normal Patch to show up.")] public GameObject initPatch3;
    [Tooltip("This patch will act as the finish line if the game mode does not infinitely spawn patches. Does not count toward the total patches spawned.")] public GameObject finishPatch;
    [Tooltip("An array of patches that do not have obstacles in them.")] public GameObject[] patchEmpty;
    [Tooltip("An array of EASY patches.")] public GameObject[] patchEasy;
    [Tooltip("An array of MEDIUM patches.")] public GameObject[] patchMedium;
    [Tooltip("An array of HARD patches.")] public GameObject[] patchHard;
    [Tooltip("An array of EXTREME patches.")] public GameObject[] patchExtreme;

    [Header("0:Jumper (Old) Settings:")]
    [Header("__________________________________________________________________")]
    [Tooltip("Parent Gameobject of Jumper Mode.")]
    public GameObject jumperModeParent;
    [Tooltip("The minimum amount of distance (units) that two obstacles can spawn beside each other. The lower the more difficult.")]
    public int minDistance = minDistance_Default;
    [Tooltip("The probability (percentage) that an obstacle is spawned. The higher the more difficult.")]
    [Range(0f, 100f)]
    public float spawnChance = spawnChance_Default;

    [Header("1:INFINITE_RUNNER Settings:")]
    [Header("__________________________________________________________________")]
    [Tooltip("The chance of this patch spawning will be its individual priority over the all the priorities combined. A value of 0 implies that the patch will never be spawned.")]
    public int emptyPatchSpawnPriority = emptyPatchSpawnPriority_Default;
    [Tooltip("The chance of this patch spawning will be its individual priority over the all the priorities combined. A value of 0 implies that the patch will never be spawned.")]
    public int easyPatchSpawnPriority = easyPatchSpawnPriority_Default;
    [Tooltip("The chance of this patch spawning will be its individual priority over the all the priorities combined. A value of 0 implies that the patch will never be spawned.")]
    public int mediumPatchSpawnPriority = mediumPatchSpawnPriority_Default;
    [Tooltip("The chance of this patch spawning will be its individual priority over the all the priorities combined. A value of 0 implies that the patch will never be spawned.")]
    public int hardPatchSpawnPriority = hardPatchSpawnPriority_Default;
    [Tooltip("The chance of this patch spawning will be its individual priority over the all the priorities combined. A value of 0 implies that the patch will never be spawned.")]
    public int extremePatchSpawnPriority = extremePatchSpawnPriority_Default;

    [Header("2:BASIC_OPTIMIZATION Settings & 3:SIMULATED_ANNEALING Settings")]
    [Header("__________________________________________________________________")]
    [Tooltip("The total amount of patches to be spawned.")]
    public int patchAmount = patchAmount_Default;
    [Tooltip("The total difficulty of the game.")]
    public float totalPatchDifficulty = totalPatchDifficulty_Default;
    [Tooltip("The maximum time in seconds that the algorithm should take to run.")]
    public int iterations = iterations_Default;
    [Tooltip("Assign a difficulty level for EASY patches.")]
    public float easyPatchDif = easyPatchDif_Default;
    [Tooltip("Assign a difficulty level for MEDIUM patches.")]
    public float mediumPatchDif = mediumPatchDif_Default;
    [Tooltip("Assign a difficulty level for HARD patches.")]
    public float hardPatchDif = hardPatchDif_Default;
    [Tooltip("Assign a difficulty level for EXTREME patches.")]
    public float extremePatchDif = extremePatchDif_Default;

    [Header("4:MAKE_MY_OWN Settings")]
    [Header("__________________________________________________________________")]
    [Tooltip("The string of digits used to generate the patch list.")]
    public string patchSeed = patchSeed_Default;
    [Tooltip("The number of times the patch list should generate patches.")]
    public int totalIterations = totalIterations_Default;
    [Tooltip("Amouth of times the patch list has generated patches.")]
    public int numIterations = numIterations_Default;
    [Tooltip("TRUE if the patch list should generate patches indefinitely.")]
    public bool isEndless = isEndless_Default;
    [Tooltip("TRUE if the difficulty should increase for every new iteration.")]
    public bool diffIsActive = diffIsActive_Default;
    [Tooltip("TRUE if the difficulty should increase by a percentage (as opposed to a constant value).")]
    public bool usePercentage = usePercentage_Default;
    [Tooltip("Amount of times the annealing algorithm should run.")]
    public int annealingIterations = annealingIterations_Default;
    [Tooltip("The difficulty value that the patch list should reach.")]
    public float difficultyGoal = difficultyGoal_Default;
    [Tooltip("The constant value that modifies the difficulty by addition/multiplication.")]
    public float difficultyConstant = difficultyConstant_Default;
    [Tooltip("The current difficulty value based on the patch list.")]
    public float difficultyCurrent = difficultyCurrent_Default;
    [Tooltip("TRUE to show annealing dubugging logs regardless of ")]
    public bool summarizeAnnealing = summarizeAnnealing_Default;
    public Preset[] presets;

    [Header("OTHER VALUES (Do not modify):")]
    [Header("__________________________________________________________________")]
    [Tooltip("Index of the patch that the player is on (e.g. 0 for True Genesis patch).")]
    public int flowObjects_current;
    [Tooltip("Amount of regular patches entered so far.")]
    public int flowObjects_totalEntered;
    [Tooltip("Amount of patches entered so far.")]
    public int flowObjects_totalRegularEntered;
    [Tooltip("Amount of patches spawned so far.")]
    public int flowObjects_totalSpawned;
    [Tooltip("Amount of regular patches spawned so far (excluding Genesis patches).")]
    public int flowObjects_totalRegularSpawned;
    [Tooltip("Current location for new patches to spawn into.")]
    public Vector3Int currentSpawnLocation;
    [Tooltip("Whether or not the final patch has been spawned.")]
    public bool finalPatchSpawned = finalPatchSpawned_Default;

    public List<Patch> patchList;
    [HideInInspector]
    public GameFlowFramework_ScriptReferencer referencer;
    public List<FlowObject> flowObjects; //List of all EVERY patch object in the game

    [HideInInspector]
    public GameMode_Jumper gameMode_JumperScript;
    [HideInInspector]
    public GameMode_INFINITE_RUNNER gameMode_InfiniteRunnerScript;
    [HideInInspector]
    public GameMode_BASIC_OPTIMIZATION gameMode_BasicOptimizationScript;
    [HideInInspector]
    public GameMode_SIMULATED_ANNEALING gameMode_SimulatedAnnealingScript;
    [HideInInspector]
    public GameMode_MAKE_MY_OWN gameMode_MakeMyOwnScript;

    void Start()
    {
        gameMode_JumperScript = GetComponentInChildren<GameMode_Jumper>();
        gameMode_InfiniteRunnerScript = GetComponentInChildren<GameMode_INFINITE_RUNNER>();
        gameMode_BasicOptimizationScript = GetComponentInChildren<GameMode_BASIC_OPTIMIZATION>();
        gameMode_SimulatedAnnealingScript = GetComponentInChildren<GameMode_SIMULATED_ANNEALING>();
        gameMode_MakeMyOwnScript = GetComponentInChildren<GameMode_MAKE_MY_OWN>();

        referencer = GetComponent<GameFlowFramework_ScriptReferencer>();
        currentSpawnLocation = startingPatchPosition;
        flowObjects = new List<FlowObject>();
        flowObjects_current = 0;
        flowObjects_totalEntered = 0;
        flowObjects_totalRegularEntered = 0;
        flowObjects_totalSpawned = 0;
        flowObjects_totalRegularSpawned = 0;

        presets = new Preset[3];
        for (int i = 0; i < presets.Length; i++)
        {
            presets[i] = new Preset();
            presets[i].name = "00" + (i + 1);
            presets[i].seed = "11111";
            presets[i].iterations = 2;
            presets[i].isEndless = false;
            presets[i].difficulty = 0;
            presets[i].diffIsActive = false;
            presets[i].usePercentage = false;
        }
    }

    public void Init()
    {
        if (gameMode0) //Jumper (old)
        {
            gameMode_JumperScript.Init();
        }
        else if (gameMode1) //1:INFINITE_RUNNER
        {
            gameMode_InfiniteRunnerScript.Init();
        }
        else if (gameMode2) //2:BASIC_OPTIMIZATION
        {
            gameMode_BasicOptimizationScript.Init();
        }
        else if (gameMode3) //3:SIMULATED_ANNEALING
        {
            gameMode_SimulatedAnnealingScript.Init();
        }
        else if (gameMode4) //4:MAKE_MY_OWN
        {
            gameMode_MakeMyOwnScript.Init();
        }
        else
        {
            Debug.Log("Start(): ERROR INITIALIZING GAME MODE!!!");
        }
    }

    public void SetGameMode(int i)
    {
        gameMode0 = false;
        gameMode1 = false;
        gameMode2 = false;
        gameMode3 = false;
        gameMode4 = false;

        //For when changing the algorithm for Game Mode Dodger
        switch (i)
        {
            case 0: //Jumper (old)
                gameMode0 = true;
                break;
            case 1: //1:INFINITE_RUNNER
                gameMode1 = true;
                break;
            case 2: //2:BASIC_OPTIMIZATION
                gameMode2 = true;
                break;
            case 3: //3:SIMULATED_ANNEALING
                gameMode3 = true;
                break;
            case 4: //4:MAKE_MY_OWN
                gameMode4 = true;
                break;
            default:
                Debug.Log("SetGameMode(): ERROR SETTING GAMEMODE!");
                break;
        }
    }

    //Global Functions for the Game Modes
    /*-----------------------------------------------------------------*/

    /// <summary>
    /// This function is called everytime a patch is instantiated for keeping track
    /// </summary>
    /// <param name="patch">The GameObject that was instantiated.</param>
    /// <param name="role">0: genesis patch, 1: regular patch, 2: pre-question patch, 3: question patch, 4: finisher patch</param>
    public void NewFlow(GameObject patch, int role)
    {
        int index = flowObjects.Count;

        string header;
        switch (role)
        {
            case 0:
                header = "flow[" + index + "]:GENESIS:<";
                break;
            case 1:
                header = "flow[" + index + "]:REGULAR:<";
                flowObjects_totalRegularSpawned++;
                break;
            case 2:
                header = "flow[" + index + "]:PRE-QUES.:<";
                break;
            case 3:
                header = "flow[" + index + "]:QUESTION:<";
                break;
            case 4:
                header = "flow[" + index + "]:FINISHER:<";
                break;
            default:
                Debug.Log("Newflow(): ERROR!!! Invalid Role");
                return;
        }
        if (index == 0) { header = "flow[" + index + "]:REAL-GENESIS:<"; }
        flowObjects_totalSpawned++;

        FlowObject flow = new FlowObject();
        flow.patch = patch;
        flow.role = role;
        flow.patch.name = header + patch.name + ">";
        flow.spawnTime = referencer.CanvasScript.realTimeCountInt + "s";
        flowObjects.Add(flow);

        currentSpawnLocation.x += patchLength;
        
        if (enableDebugLog)
        {
            Debug.Log(flow.patch.name + " at location " + flow.patch.transform.position);
        }
    }

    /// <summary>
    /// Spawn the first four patches (these patches do not count towards the amount of spawned patches)
    /// </summary>
    public void SpawnStartingPatches()
    {
        GameObject a = Instantiate(genesisPatch, currentSpawnLocation, Quaternion.identity);
        NewFlow(a, 0);

        GameObject b = Instantiate(initPatch1, currentSpawnLocation, Quaternion.identity);
        NewFlow(b, 0);

        GameObject c = Instantiate(initPatch2, currentSpawnLocation, Quaternion.identity);
        NewFlow(c, 0);

        GameObject d = Instantiate(initPatch3, currentSpawnLocation, Quaternion.identity);
        NewFlow(d, 0);
    }

    /// <summary>
    /// This function is called everytime the player enters a patch
    /// </summary>
    /// <param name="role">0: genesis patch, 1: regular patch, 2: pre-question patch, 3: question patch, 4: finisher patch</param>
    public void EnteredPatch(int role)
    {
        flowObjects_current++; //update the current patch that the player is on top of
        if (flowObjects[flowObjects_current].role == 1) //if the patch is of type "regular"
        {
            flowObjects_totalRegularEntered++; //increment total regular patches entered
        }
        flowObjects_totalEntered++; //increment total patches entered (regardless of type)

        //if gamemode is of type:
        //BASIC_OPTIMIZATION,
        //SIMULATED ANNEALING
        if (gameMode2 || gameMode3)
        {
            gameMode_SimulatedAnnealingScript.SpawnWhenEnter_PatchListBased(); //spawn next patch
            return;
        }

        //if gamemode is of type:
        //MAKE_MY_OWN
        if (gameMode4)
        {
            gameMode_MakeMyOwnScript.SpawnWhenEnter_PatchListBasedWithIterations(); //spawn next patch
            return;
        }

        //if gamemode is of type:
        //INFINITE_RUNNER
        if (gameMode1 && role == 1) //if gamemode is INFINITE_RUNNER and the patch is of type "regular"
        {
            gameMode_InfiniteRunnerScript.SpawnWhenEnter_PriorityBased(); //spawn next patch
            return;
        }
    }

    /// <summary>
    /// Initialize the patch list to be of size "patchAmount" filled with patches of type "empty"
    /// </summary>
    public void InitEmptyPatches()
    {
        //creates array of empty patches
        patchList = new List<Patch>();
        for (int i = 0; i < patchAmount; i++)
        {
            Patch emptyPatch = new Patch();
            emptyPatch.type = 0;
            emptyPatch.difficulty = 0;
            patchList.Add(emptyPatch);
        }
    }

    /// <summary>
    /// Print out the patch info by index (note: index of flowObjects, NOT patchList)
    /// </summary>
    public void FlowPatchPrintInfo(int i)
    {
        Debug.Log("<---Flow Patch [START]--->");
        Debug.Log("Index: " + i);
        Debug.Log("Patch Name: " + flowObjects[i].patch.name);
        Debug.Log("Role: " + flowObjects[i].role);
        Debug.Log("SpawnTime: " + flowObjects[i].spawnTime);
        Debug.Log("Current Location: " + flowObjects[i].patch.transform.position);
        Debug.Log("<---Flow Patch [END]--->");
    }

    /// <summary>
    /// Spawns a patch from the patch list.
    /// </summary>
    public void SpawnPatchFromPatchList()
    {
        //check if we should spawn a question given that the player had just entered a new patch
        referencer.GameFlowFramework_Questions.TriggerPatchCheck(flowObjects_totalRegularSpawned);

        //if the patch list has run out of patches to be spawned
        if (patchList.Count == 0)
        {
            FinishPatch(); //spawn the final patch
            return;
        }

        int chosenVariation = 0; //patch variation (for every patch type, there exists an array of variations of itself)
        GameObject p; //the patch to be spawned
        switch (patchList[0].type) //extract a patch from the patch list
        {
            case 0: //empty patch
                chosenVariation = Random.Range(0, patchEmpty.Length); //pick variation
                p = Instantiate(patchEmpty[chosenVariation], currentSpawnLocation, Quaternion.identity); //spawn
                break;
            case 1: //easy patch
                chosenVariation = Random.Range(0, patchEasy.Length); //pick variation
                p = Instantiate(patchEasy[chosenVariation], currentSpawnLocation, Quaternion.identity); //spawn
                break;
            case 2: //medium patch
                chosenVariation = Random.Range(0, patchMedium.Length); //pick variation
                p = Instantiate(patchMedium[chosenVariation], currentSpawnLocation, Quaternion.identity); //spawn
                break;
            case 3: //hard patch
                chosenVariation = Random.Range(0, patchHard.Length); //pick variation
                p = Instantiate(patchHard[chosenVariation], currentSpawnLocation, Quaternion.identity); //spawn
                break;
            case 4: //extreme patch
                chosenVariation = Random.Range(0, patchExtreme.Length); //pick variation
                p = Instantiate(patchExtreme[chosenVariation], currentSpawnLocation, Quaternion.identity); //spawn
                break;
            default:
                Debug.Log("ERROR: Invalid type for SpawnPatch()!");
                return;
        }
        NewFlow(p, 1); //register the new patch
        patchList.RemoveAt(0); //update patch list
    }

    /// <summary>
    /// Spawns the final patch.
    /// </summary>
    public void FinishPatch() //to spawn the last patch
    {
        if (finalPatchSpawned)
        {
            Debug.Log("ERROR: Why is final patch being spawned again?");
            return;
        }
        GameObject p = Instantiate(finishPatch, currentSpawnLocation, Quaternion.identity); //spawn
        NewFlow(p, 4);
        finalPatchSpawned = true;
    }

    /*-----------------------------------------------------------------*/

    //Other
    /*-----------------------------------------------------------------*/

    public void UpdateCanvas()
    {
        //665.0f for X
        //for below: (up, middle, bottom)
        //heath: 465f, 380.2f, 295.4f for Y
        //score: 465f, 380.2f, 295.4f for Y
        //timer: 474.35f, 389.55f, 304.75f for Y

        canvas.health.enabled = showHealth;
        canvas.score.enabled = showScore;
        canvas.timer.enabled = showTimer;
        canvas.timerClock.enabled = showTimer;
        canvas.hits.enabled = showHits;

        /*
        if (!showHealth && !showScore && !showTimer) //000
        {
            //do nothing
        }
        else if (!showHealth && !showScore && showTimer) //001
        {
            canvas.timer.rectTransform.anchoredPosition = new Vector2(665.0f, 474.35f);
        }
        else if (!showHealth && showScore && !showTimer) //010
        {
            canvas.score.rectTransform.anchoredPosition = new Vector2(665.0f, 465f);
        }
        else if (!showHealth && showScore && showTimer) //011
        {
            canvas.score.rectTransform.anchoredPosition = new Vector2(665.0f, 465f);
            canvas.timer.rectTransform.anchoredPosition = new Vector2(665.0f, 389.55f);
        }
        else if (showHealth && !showScore && !showTimer) //100
        {
            canvas.health.rectTransform.anchoredPosition = new Vector2(665.0f, 465f);
        }
        else if (showHealth && !showScore && showTimer) //101
        {
            canvas.health.rectTransform.anchoredPosition = new Vector2(665.0f, 465f);
            canvas.timer.rectTransform.anchoredPosition = new Vector2(665.0f, 389.55f);
        }
        else if (showHealth && showScore && !showTimer) //110
        {
            canvas.health.rectTransform.anchoredPosition = new Vector2(665.0f, 465f);
            canvas.score.rectTransform.anchoredPosition = new Vector2(665.0f, 380.2f);
        }
        else if (showHealth && showScore && showTimer) //111
        {
            canvas.health.rectTransform.anchoredPosition = new Vector2(665.0f, 465f);
            canvas.score.rectTransform.anchoredPosition = new Vector2(665.0f, 380.2f);
            canvas.timer.rectTransform.anchoredPosition = new Vector2(665.0f, 304.75f);
        }
        */
    }

    public void CheckValues()
    {
        //Check bounds
        if (patchAmount < 1)
        {
            patchAmount = patchAmount_Default;
        }
        if (totalPatchDifficulty < 0)
        {
            totalPatchDifficulty = totalPatchDifficulty_Default;
        }
        if (iterations < 1)
        {
            iterations = iterations_Default;
        }
        if (easyPatchDif < 0)
        {
            easyPatchDif = easyPatchDif_Default;
        }
        if (mediumPatchDif < 0)
        {
            mediumPatchDif = mediumPatchDif_Default;
        }
        if (hardPatchDif < 0)
        {
            hardPatchDif = hardPatchDif_Default;
        }
        if (extremePatchDif < 0)
        {
            extremePatchDif = extremePatchDif_Default;
        }
    }

    public void ResetValues()
    {
        //Default Values
        /****************************************************************************/
        //General
        enableDebugLog = enableDebugLog_Default;
        showTimer = showTimer_Default;
        timerIsCountdown = timerIsCountdown_Default;
        timerStartingTime = timerStartingTime_Default;
        showHealth = showHealth_Default;
        showHits =  showHits_Default;
        showScore = showScore_Default;
        startingPatchPosition = startingPatchPosition_Default;
        patchLength = patchLength_Default;
        finalPatchSpawned = finalPatchSpawned_Default;

        //GameModes:
        gameMode0 = gameMode0_Default; //Jumper (old)
        gameMode1 = gameMode1_Default; //1:INFINITE_RUNNER
        gameMode2 = gameMode2_Default; //2:BASIC_OPTIMIZATION
        gameMode3 = gameMode3_Default; //3:SIMULATED_ANNEALING
        gameMode4 = gameMode4_Default; //4:MAKE_MY_OWN

        //GameMode Jumper (old) Settings
        minDistance = minDistance_Default;
        spawnChance = spawnChance_Default;
        //GameMode 1:INFINITE_RUNNER Settings
        emptyPatchSpawnPriority = emptyPatchSpawnPriority_Default;
        easyPatchSpawnPriority = easyPatchSpawnPriority_Default;
        mediumPatchSpawnPriority = mediumPatchSpawnPriority_Default;
        hardPatchSpawnPriority = hardPatchSpawnPriority_Default;
        extremePatchSpawnPriority = extremePatchSpawnPriority_Default;
        //GameMode 2:BASIC_OPTIMIZATION Settings and
        //GameMode 3:SIMULATED_ANNEALING Settings
        patchAmount = patchAmount_Default;
        totalPatchDifficulty = totalPatchDifficulty_Default;
        iterations = iterations_Default;
        easyPatchDif = easyPatchDif_Default;
        mediumPatchDif = mediumPatchDif_Default;
        hardPatchDif = hardPatchDif_Default;
        extremePatchDif = extremePatchDif_Default;
        //GameMode 4:MAKE_MY_OWN Settings
        patchSeed = patchSeed_Default;
        totalIterations = totalIterations_Default;
        numIterations = numIterations_Default;
        isEndless = isEndless_Default;
        diffIsActive = diffIsActive_Default;
        usePercentage = usePercentage_Default;
        annealingIterations = annealingIterations_Default;
        difficultyGoal = difficultyGoal_Default;
        difficultyConstant = difficultyConstant_Default;
        difficultyCurrent = difficultyCurrent_Default;
        summarizeAnnealing = summarizeAnnealing_Default;
        //presetAmount;
        //Preset[] presets;

    /****************************************************************************/
}

    public void ToggleUI(GeneralSettingsPreset gen)
    {
        showHealth = gen.showHealth;
        showScore = false;
        showHits = gen.showHits;
        showTimer = gen.showTimer;
        timerIsCountdown = gen.timerCountdown;
        UpdateCanvas();
    }

    public void ToggleCountdown()
    {
        timerIsCountdown = !timerIsCountdown;
    }

    /*-----------------------------------------------------------------*/
}

public class Patch
{
    /// <summary>
    ///0 for Empty Patch,
    ///1 for Easy Patch,
    ///2 for Medium Patch,
    ///3 for Hard Patch,
    ///4 for Extreme Patch
    /// </summary>
    public int type;

    public float difficulty;
}

public class FlowObject
{
    public GameObject patch;

    /// <summary>
    ///0 for genesis patch,
    ///1 for regular patch,
    ///2 for pre-question patch,
    ///3 for question patch,
    ///4 for finisher patch
    /// </summary>
    public int role;

    public string spawnTime;
}

//[System.Serializable]
public class Preset
{
    public string name;

    public string seed;

    public int iterations;
    public bool isEndless;

    public float difficulty;
    public bool diffIsActive;
    public bool usePercentage;
}

