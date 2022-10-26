using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unity_Purdue_Difficulty : MonoBehaviour
{
    //Default Values
    static bool enableDebugLog_Default = false;
    static int minDistance_Default = 1;
    static float spawnChance_Default = 60;
    static int livesOrHits_default = 5;
    static bool showTimeCounter_Default = true;
    static bool gameModeJump_Default = true;
    static bool gameModeDodge_Default = false;
    static float gameDuration_Default = 20;
    static int totalAmountOfQuestions_Default = 0;
    static Vector3Int currentPatchPosition_Default = new Vector3Int(0,0,0);
    static bool infiniteHits_Default = false;

    [HideInInspector]
    public bool gameModeJump = gameModeJump_Default;
    [HideInInspector]
    public bool gameModeDodge = gameModeDodge_Default;

    [Tooltip("The script that handles all the obstacles.")]
    public ObstacleArray obstacles;
    [Tooltip("Show debugging logs?")]
    public bool enableDebugLog = enableDebugLog_Default;
    [Tooltip("The script attached to the main canvas.")]
    public CanvasScript canvas;
    [Tooltip("The UI component (player life count) that will be updated during gameplay.")]
    public Text livesUI;

    [Header("Difficulty Settings (General):")]
    [Header("********************************************************")]
    [Tooltip("Number of lives/hits the player has. Lives decrease as you get hit, Hits increase as you get hit.")]
    public int livesOrHits = livesOrHits_default;
    public bool showTimeCounter = showTimeCounter_Default;

    [Header("Difficulty Settings (Jump Mode):")]
    [Header("********************************************************")]
    [Tooltip("Jump Mode Environment Parent Object.")]
    public GameObject jumpModeParent;
    [Tooltip("The minimum amount of distance (units) that two obstacles can spawn beside each other. The lower the more difficult.")]
    public int minDistance = minDistance_Default;
    [Tooltip("The probability (percentage) that an obstacle is spawned. The higher the more difficult.")]
    [Range(0f,100f)]
    public float spawnChance = spawnChance_Default;

    [Header("Difficulty Settings (Dodge Mode):")]
    [Header("********************************************************")]
    [Tooltip("Dodge Mode Environment Parent Object.")]
    public GameObject dodgeModeParent;
    [Tooltip("The duration of the game.")]
    public float gameDuration;
    [Tooltip("Whether or not you may collide with obstacles indefinitely.")]
    public bool infiniteHits = infiniteHits_Default;
    [Tooltip("The position of the current patch being spawned (DO NOT EDIT).")]
    public Vector3Int currentPatchPosition;
    [Tooltip("An array of patches to be randomly selected for spawning. Should be a Plane with the scale of 2,1,1 (x,y,z)")]
    public GameObject[] environmentPatches;
    [Tooltip("The patch that collects answers 1, 2, 3, 4, and 5. (CAUTION WHEN EDITING).")]
    public GameObject questionPatch;
    [Tooltip("The patch that collects answers Yes and No. (CAUTION WHEN EDITING).")]
    public GameObject questionPatchYesNo;
    int amountOfPatchesToChooseFrom;
    int patchIndex;

    [Tooltip("The total amount of questions (this value will dictate other values below).")]
    public int totalAmountOfQuestions;
    [Tooltip("Patches that were spawned so far during the game.")]
    public int patchesSpawnedSoFar = 0;
    [Tooltip("The questions that you would like to ask the player.")]
    public string[] questions;
    [Tooltip("Which of those questions are Yes/No questions?")]
    public bool[] isAYesNoQuestion;
    [Tooltip("The question should be spawned after what patch? The initial patches do not count. A value of 0 implies spawning right away. Duplicate values will be ignored.")]
    public int[] questionSpawnAfterPatch;


    /*([Tooltip("Amount of time a question shows up on screen.")]
    public float secondsPerQuestion;
    [Tooltip("An array of questions to ask the player in-game. Every question adds secondsPerQuestion seconds to the game.")]
    public string[] questions;*/


    void Start()
    {
        amountOfPatchesToChooseFrom = environmentPatches.Length;

        if (gameModeJump)
        {
            //The Game Mode is Jumping through obstacles
            gameModeJump_init();
        }
        else if (gameModeDodge)
        {
            //The Game Mode is dodging obstacles
            gameModeDodge_init();
        }
        else
        {
            Debug.Log("ERROR INITIALIZING GAME MODE!!!");
        }
    }

    void gameModeJump_init()
    {
        jumpModeParent.SetActive(true);

        int spawnCooldown = minDistance; //obstacles must remain a minimum distance from each other based on minDistance
        float randomNumber; //the chance of spawning an obstacle

        if (enableDebugLog) { Debug.Log("Number of obstacles: " + obstacles.obstacles.Length); }

        //for every obstacle in game
        for (int i = 0; i < obstacles.obstacles.Length; i++)
        {
            spawnCooldown--;
            //if minDistance reached (if the current obstacle is far enough away)
            if (spawnCooldown == 0)
            {
                randomNumber = Random.Range(0f, 100f); //generate random percentage
                if (randomNumber <= spawnChance) //if the random number is within spawnChance
                {
                    obstacles.obstacles[i].SetActive(true); //spawn obstacle

                    if (enableDebugLog)
                    {
                        Debug.Log("obstacle spawned.");
                        Debug.Log("obstacles[" + i + "] spawned with chance " + randomNumber);
                    }
                }
                spawnCooldown = minDistance; //reset minDistance
            }
        }
    }

    void gameModeDodge_init()
    {
        dodgeModeParent.SetActive(true);

        //First Patch (already Active), position: (x,y,z) -40,0,0
        //Second Patch (already Active), position: (x,y,z) -20,0,0
        //Third Patch (already Active), position: (x,y,z) 0,0,0

        StartCoroutine(endGameModeDodge());
    }

    void updateUI()
    {
        //change UI
        canvas.timer.enabled = showTimeCounter;
        if ((gameModeJump) || (gameModeDodge && !infiniteHits))
        {
            livesUI.text = "LIVES: " + livesOrHits;
        }
        else if (gameModeDodge && infiniteHits)
        {
            livesUI.text = "HITS: " + livesOrHits;
        }
        else
        {
            Debug.Log("SOMETHING WENT WRONG.");
        }
    }

    IEnumerator endGameModeDodge()
    {
        yield return new WaitForSeconds(gameDuration);
        canvas.GameOver();
    }

    public void endOfPatch()
    {
        bool questionSpawned = false;
        currentPatchPosition.x = currentPatchPosition.x + 20;

        for (int i = 0; i < totalAmountOfQuestions; i++)
        {
            if (patchesSpawnedSoFar == questionSpawnAfterPatch[i])
            {
                GameObject spawnedQuestionPatch;
                InteractiveAnswerParent questionScript;
                if (isAYesNoQuestion[i])
                {
                    spawnedQuestionPatch = Instantiate(questionPatchYesNo, currentPatchPosition, Quaternion.identity);
                }
                else
                {
                    spawnedQuestionPatch = Instantiate(questionPatch, currentPatchPosition, Quaternion.identity);
                }
                questionScript = spawnedQuestionPatch.GetComponent<InteractiveAnswerParent>();
                questionScript.question = questions[i];

                patchesSpawnedSoFar++;
                questionSpawned = true;
                break;
            }
        }

        if (!questionSpawned)
        {
            int chosenPatch;
            chosenPatch = Random.Range(0, amountOfPatchesToChooseFrom);
            Instantiate(environmentPatches[chosenPatch], currentPatchPosition, Quaternion.identity);
            patchesSpawnedSoFar++;
        }
    }

    public void checkValue()
    {
        //make sure that the values for minDistance
        if (minDistance <= 0){ minDistance = 1; }

        //make sure that livesOrHits do not break the game
        if (livesOrHits <= 0 && gameModeJump){ livesOrHits = 1; }

        //make sure to not have breaking game durations
        if (gameDuration < 10) { gameDuration = 10; }

        //make sure that the array sizes are dictacted by the total amount of questions
        if (questions.Length != totalAmountOfQuestions) { questions = new string[totalAmountOfQuestions]; }
        if (isAYesNoQuestion.Length != totalAmountOfQuestions) { isAYesNoQuestion = new bool[totalAmountOfQuestions]; }
        if (questionSpawnAfterPatch.Length != totalAmountOfQuestions) { questionSpawnAfterPatch = new int[totalAmountOfQuestions]; }

        //amount of patches initiallly
        patchesSpawnedSoFar = 0;

        //update UI
        updateUI();

        /*
        //make sure questionSpawnAfterPatch does not have duplicate values
        bool duplicateFound = false;
        for (int i = 0; i < totalAmountOfQuestions; i++)
        {
            for (int j = 0; j < totalAmountOfQuestions; j++)
            {
                if ((i != j) && (questionSpawnAfterPatch[i] == questionSpawnAfterPatch[j]))
                {
                    duplicateFound = true;
                    break;
                }
            }
            if (duplicateFound)
            {
                break;
            }
        }
        if (duplicateFound)
        {
            questionSpawnAfterPatch = new int[totalAmountOfQuestions];
        }
        */

        /*
        //make sure questionSpawnMarkInSeconds does not have impossible values

        if (totalAmountOfQuestions == 1) { resetQuestionMarks(); }
        currentPatchPosition = currentPatchPosition_Default;

        float currMark;
        for (int i = 0; i < totalAmountOfQuestions-1; i++)
        {
            currMark = questionSpawnAfterPatch[i];
            if (questionSpawnAfterPatch[i + 1] <= currMark) //if a future question has an earlier mark than the current question
            {
                //questionSpawnMarkInSeconds = new float[totalAmountOfQuestions]; //reset array to prevent impossible values
                resetQuestionMarks();
                break;
            }
        }
        */
    }

    public void damaged()
    {
        //if the player hits an obstacle

        if (gameModeJump)
        {
            damanged_lifeDamage();
        }
        else if (gameModeDodge)
        {
            if (infiniteHits)
            {
                damaged_hitIncrement();
            }
            else
            {
                damanged_lifeDamage();
            }
        }
    }

    void damanged_lifeDamage()
    {
        livesOrHits--;
        updateUI();

        if (livesOrHits <= 0)
        {
            canvas.GameOver();
        }
    }

    void damaged_hitIncrement()
    {
        livesOrHits++;
        updateUI();
    }

    public void setGame(int i)
    {
        switch (i)
        {
            case 0: //gamemode Jump
                gameModeJump = true;
                gameModeDodge = false;
                break;
            case 1: //gamemode Dodge
                gameModeJump = false;
                gameModeDodge = true;
                break;
            default:
                Debug.Log("ERROR SETTING GAME MODE!!!");
                break;
        }
        updateUI();
    }

    public void reset()
    {
        enableDebugLog = enableDebugLog_Default;
        minDistance = minDistance_Default;
        spawnChance = spawnChance_Default;
        livesOrHits = livesOrHits_default;
        gameModeJump = gameModeJump_Default;
        gameModeDodge = gameModeDodge_Default;
        gameDuration = gameDuration_Default;
        totalAmountOfQuestions = totalAmountOfQuestions_Default;
        questions = new string[0];
        isAYesNoQuestion = new bool[0];
        questionSpawnAfterPatch = new int[0];
        currentPatchPosition = currentPatchPosition_Default;
        patchesSpawnedSoFar = 0;
        infiniteHits = infiniteHits_Default;
        showTimeCounter = showTimeCounter_Default;
        updateUI();
    }
}