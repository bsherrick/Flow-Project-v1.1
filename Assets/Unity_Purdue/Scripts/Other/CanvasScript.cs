using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{
    public bool gameIsPaused = false;
    public bool gameHasStarted = false;
    public bool gameHasEnded = false;
    public bool enableDebugLog = false;


    [Header("CANVASES")]
    public GameObject[] canvas;
    //0: FocusGroupStartScreen
    //1: GameSelection
    //2: PreGameSettings
    //3: GeneralSettings
    //4: QuestionEditor
    //5: GameScreen
    //6: PauseScreen
    //7: GameOverScreen
    //8: ErrorScreen
    //9: Gamemode1
    //10: Gamemode2
    //11: Gamemode3
    //12: Gamemode4
    //13: LoadingScreen
    //14: PasswordScreen


    public GameObject canvasCurrent;

    [Header("GAME OVER FIELDS")]
    public Text gameOverHealth;
    public Text gameOverHits;
    public Text gameOverTime;

    [Header("INPUT FIELDS - 1:INFINITE_RUNNER")]
    public InputField input1EmptyPriority;
    public InputField input1EasyPriority;
    public InputField input1MediumPriority;
    public InputField input1HardPriority;
    public InputField input1ExtremePriority;

    [Header("INPUT FIELDS - 2:BASIC_OPTIMIZATION")]
    public InputField input2PatchAmount;
    public InputField input2Difficulty;
    public InputField input2Iterations;
    public InputField input2EasyDiff;
    public InputField input2MediumDiff;
    public InputField input2HardDiff;
    public InputField input2ExtremeDiff;

    [Header("INPUT FIELDS - 3:SIMULATED_ANNEALING")]
    public InputField input3PatchAmount;
    public InputField input3Difficulty;
    public InputField input3Iterations;
    public InputField input3EasyDiff;
    public InputField input3MediumDiff;
    public InputField input3HardDiff;
    public InputField input3ExtremeDiff;

    [Header("INPUT FIELDS - 4:MAKE_MY_OWN")]
    public InputField input4Seed;
    public InputField input4Iterations;
    public Toggle input4Endless;
    public InputField input4Diff;
    public Toggle input4DiffActive;
    public Toggle input4UsePercentage;
    public Text input4PresetName;
    public Text input4PresetInfo;
    public Text input4PresetIndex;

    [Header("INPUT FIELDS - PASSWORD")]
    public InputField pass;

    [Header("INPUT FIELDS - IDENTIFICATION")]
    public InputField identification;

    [Header("OTHER SETTINGS")]
    public bool focusStudyMode = false;
    public Text timer;
    public Image timerClock;
    public float timeCount = 0;  //time in seconds since start of game
    public int timeCountFormated = 0;
    public float realTimeCount = 0; //time in seconds since start of game independent of user settings
    public int realTimeCountInt = 0;
    string minutes;
    string seconds;
    string gameTime;
    string realTime;
    public Text loadingMessage; //for the start of the application

    public Text health; //UI object that displays health
    public int healthCount = 0; //health of the player

    public Text hits; //UI object that displays the hit count
    public int hitsCount = 0; //hits that the player went through

    public Text score; //UI object that displays score
    public int scoreCount = 0; //score of the player

    public Text gameOverMessage; //the message that says GameOver

    public Text question; //the message that displays whatever is the current question
    public Text answer; //the message that displays whatever the player is currently selecting

    public float gameSpeed = 1;

    public GameFlowFramework_ScriptReferencer referencer;

    public GameObject getFocusStudyResult;

    bool m = false;

    void Start()
    {
        timeCount = referencer.GameFlowFramework_Environment.timerStartingTime;
        StopTime(true);
        //StartCoroutine(temp());
        referencer.GameFlowFramework_Web.All();
    }

    /*IEnumerator temp()
    {
        yield return new WaitForSecondsRealtime(2);
        referencer.GameFlowFramework_Web.UploadPresets();
    }*/

    public void Start2()
    {
        canvas[13].SetActive(false); //13: LoadingScreen
        canvasCurrent = canvas[0]; //0: FocusGroupStartScreen
        canvasCurrent.SetActive(true);

        referencer.QuestionEditor.SetQuestions(referencer.QuestionEditor.currentIndex);
        referencer.GameFlowFramework_Environment.gameMode_MakeMyOwnScript.SetPreset(referencer.GameFlowFramework_Environment.gameMode_MakeMyOwnScript.uiIndex);
        referencer.GeneralSettings.SetPreset(referencer.GeneralSettings.uiIndex);
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("PRESSED 1: PRINT STATUS");
            PrintCurrentStats("MASTER MODE");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("PRESSED 2: PRINT FLOW OBJECTS");
            PrintFlowObject("MASTER MODE");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("PRESSED 3: TRIGGER QUESTION");
            referencer.GameFlowFramework_PlayerCharacter.HealthToFifty();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("PRESSED 4: SEND GET");
            referencer.GameFlowFramework_Web.DownloadQuestions();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("PRESSED 5: SEND POST");
            referencer.GameFlowFramework_Web.UploadQuestions();
        }
        */

        if (Input.GetKeyDown(KeyCode.Escape) && gameHasStarted && !gameHasEnded)
        {
            PauseGame(!gameIsPaused);
        }

        UpdateTime();
        UpdateUI();

        if (!gameHasStarted || gameIsPaused)
        {
            return;
        }
    }

    public void SwitchCanvas(int i)
    {
        if (i < 0 || i >= canvas.Length)
        {
            Debug.Log("CanvasScript: SwitchCanvas(): ERROR! NO VALID CANVAS.");
            return;
        }

        if (i == 1 && pass.text != "coconutpie") //14: PasswordScreen
        {
            return;
        }

        //0: FocusGroupStartScreen
        //1: GameSelection
        //2: PreGameSettings
        //3: GeneralSettings
        //4: QuestionEditor
        //5: GameScreen
        //6: PauseScreen
        //7: GameOverScreen
        //8: ErrorScreen
        //9: Gamemode1
        //10: Gamemode2
        //11: Gamemode3
        //12: Gamemode4
        //13: LoadingScreen
        //14: PasswordScreen
        canvasCurrent.SetActive(false);
        canvasCurrent = canvas[i];
        switch (i)
        {
            case 3: //3: GeneralSettings
                canvas[5].SetActive(true); //5: GameScreen
                break;
            case 4: //4: QuestionEditor
                referencer.QuestionEditor.SetQuestions(referencer.QuestionEditor.currentIndex);
                break;          
            case 9: //9: Gamemode1
                referencer.GameFlowFramework_Environment.SetGameMode(1);
                break;
            case 10: //10: Gamemode2
                referencer.GameFlowFramework_Environment.SetGameMode(2);
                break;
            case 11: //11: Gamemode3
                referencer.GameFlowFramework_Environment.SetGameMode(3);
                break;
            case 12: //12: Gamemode4
                referencer.GameFlowFramework_Environment.SetGameMode(4);
                referencer.GameFlowFramework_Environment.gameMode_MakeMyOwnScript.SetPreset(referencer.GameFlowFramework_Environment.gameMode_MakeMyOwnScript.uiIndex);
                break;
        }
        canvasCurrent.SetActive(true);
    }

    public void Back()
    {
        //0: FocusGroupStartScreen
        //1: GameSelection
        //2: PreGameSettings
        //3: GeneralSettings
        //4: QuestionEditor
        //5: GameScreen
        //6: PauseScreen
        //7: GameOverScreen
        //8: ErrorScreen
        //9: Gamemode1
        //10: Gamemode2
        //11: Gamemode3
        //12: Gamemode4
        //13: LoadingScreen
        //14: PasswordScreen

        if (canvasCurrent == canvas[0]) //0: FocusGroupStartScreen
        {
            Debug.Log("CanvasScript: Back(): Impossible request");
        }
        else if (canvasCurrent == canvas[6]) //6: PauseScreen
        {
            Debug.Log("CanvasScript: Back(): Impossible request");
        }
        else if (canvasCurrent == canvas[7]) //7: GameOverScreen
        {
            Debug.Log("CanvasScript: Back(): Impossible request");
        }
        else if (canvasCurrent == canvas[5]) //5: GameScreen
        {
            Debug.Log("CanvasScript: Back(): Impossible request");
        }
        else if (canvasCurrent == canvas[1]) //1: GameSelection
        {
            SwitchCanvas(0); //0: FocusGroupStartScreen
        }
        else if (canvasCurrent == canvas[9]) //9: Gamemode1
        {
            SwitchCanvas(1); //1: GameSelection
        }
        else if (canvasCurrent == canvas[10]) //10: Gamemode2
        {
            SwitchCanvas(1); //1: GameSelection
        }
        else if (canvasCurrent == canvas[11]) //11: Gamemode3
        {
            SwitchCanvas(1); //1: GameSelection
        }
        else if (canvasCurrent == canvas[12]) //12: Gamemode4
        {
            SwitchCanvas(1); //1: GameSelection
        }
        else if (canvasCurrent == canvas[14]) //14: PasswordScreen
        {
            SwitchCanvas(0); //0: FocusGroupStartScreen
        }
        else if (canvasCurrent == canvas[8]) //8: ErrorScreen
        {
            if (focusStudyMode)
            {
                SwitchCanvas(0); //0: FocusGroupStartScreen
                focusStudyMode = false;
            }
            else if (referencer.GameFlowFramework_Environment.gameMode1)
            {
                SwitchCanvas(9); //9: Gamemode1
            }
            else if (referencer.GameFlowFramework_Environment.gameMode2)
            {
                SwitchCanvas(10); //10: Gamemode2
            }
            else if (referencer.GameFlowFramework_Environment.gameMode3)
            {
                SwitchCanvas(11); //11: Gamemode3
            }
            else if (referencer.GameFlowFramework_Environment.gameMode4)
            {
                SwitchCanvas(12);//12: Gamemode4
            }
            else
            {
                Debug.Log("CanvasScript: Back(): ERROR! INVALID GAME MODE.");
            }
        }
        else if (canvasCurrent == canvas[3]) //3: GeneralSettings
        {
            SwitchCanvas(2); //2: PreGameSettings
            canvas[5].SetActive(false); //5: GameScreen
        }
        else if (canvasCurrent == canvas[2]) //2: PreGameSettings
        {
            SwitchCanvas(1); //1: GameSelection
        }
        else if (canvasCurrent == canvas[4]) //4: QuestionEditor
        {
            SwitchCanvas(2); //2: PreGameSettings
        }
        else
        {
            Debug.Log("CanvasScript: Back(): ERROR! NO VALID CANVAS.");
        }

    }

    void UpdateTime()
    {
        if (referencer.GameFlowFramework_Environment.timerIsCountdown)
        {
            timeCount -= Time.deltaTime;
        }
        else
        {
            timeCount += Time.deltaTime;
        }
        timeCountFormated = Mathf.RoundToInt(timeCount);

        realTimeCount += Time.deltaTime;
        realTimeCountInt = Mathf.RoundToInt(realTimeCount);

        int time = timeCountFormated;
        if (time < 0)
        {
            time *= -1;
            minutes = "-";
        }
        else
        {
            minutes = "";
        }

        minutes = minutes + (time / 60).ToString("D2");
        seconds = (time % 60).ToString("D2");

        gameTime = minutes + ":" + seconds + " (m:s)";

        referencer.GameFlowFramework_WinLoseCondition.CheckConditions(healthCount, hitsCount, scoreCount, timeCountFormated);
    }

    public void UpdateUI()
    {
        //00:00 (m:s)
        timer.text = gameTime;
        health.text = "HEALTH: " + healthCount;
        score.text = "SCORE: " + scoreCount;
        hits.text = "HITS: " + hitsCount;
    }

    public void PauseGame(bool b)
    {
        click();
        canvas[6].SetActive(b); //6: PauseScreen
        gameIsPaused = b;
        StopTime(b);
    }

    public void GameOver()
    {
        if (focusStudyMode)
        {
            getFocusStudyResult.SetActive(true);
        }

        if (referencer.GameFlowFramework_Environment.showHealth)
        {
            gameOverHealth.text = health.text;
        }
        else
        {
            gameOverHealth.enabled = false;
        }

        if (referencer.GameFlowFramework_Environment.showHits)
        {
            gameOverHits.text = hits.text;
        }
        else
        {
            gameOverHits.enabled = false;
        }

        if (referencer.GameFlowFramework_Environment.showTimer)
        {
            gameOverTime.text = timer.text;
        }
        else
        {
            gameOverTime.enabled = false;
        }
        canvas[5].SetActive(false); //5: GameScreen

        referencer.soundtrack2.Stop();
        referencer.soundtrack1.Play();

        gameHasEnded = true;
        canvas[7].SetActive(true);//7: GameOverScreen
        StopTime(true);
    }

    public void RestartGame()
    {
        Debug.Log("*----Game Restarted--*");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetGameSpeed(float speed)
    {
        gameSpeed = speed;
        Time.timeScale = speed;
    }

    void StopTime(bool b)
    {
        if (b)
        {
            Time.timeScale = 0;
            //Time.fixedDeltaTime = 0;
        }
        else
        {
            Time.timeScale = gameSpeed;
            //Time.fixedDeltaTime = 1;
        }
    }

    public void SetStartingValues(GeneralSettingsPreset gen)
    {
        healthCount = gen.sHealth;
        hitsCount = gen.sHits;
        timeCount = gen.sTime;
        referencer.GameFlowFramework_PlayerCharacter.health = gen.sHealth;
        referencer.GameFlowFramework_PlayerCharacter.hits = gen.sHits;
        scoreCount = 0;

        UpdateUI();
    }

    public void Gamemode1GetValues()
    {
        int empty;
        if (int.TryParse(input1EmptyPriority.text, out empty))
        {
            if (empty < 0) { empty *= -1; input1EmptyPriority.text = empty.ToString(); }
            referencer.GameFlowFramework_Environment.emptyPatchSpawnPriority = empty;
        }

        int easy;
        if (int.TryParse(input1EasyPriority.text, out easy))
        {
            if (easy < 0) { easy *= -1; input1EasyPriority.text = easy.ToString(); }
            referencer.GameFlowFramework_Environment.easyPatchSpawnPriority = easy;
        }

        int medium;
        if (int.TryParse(input1MediumPriority.text, out medium))
        {
            if (medium < 0) { medium *= -1; input1MediumPriority.text = medium.ToString(); }
            referencer.GameFlowFramework_Environment.mediumPatchSpawnPriority = medium;
        }

        int hard;
        if (int.TryParse(input1HardPriority.text, out hard))
        {
            if (hard < 0) { hard *= -1; input1HardPriority.text = hard.ToString(); }
            referencer.GameFlowFramework_Environment.hardPatchSpawnPriority = hard;
        }

        int extreme;
        if (int.TryParse(input1ExtremePriority.text, out extreme))
        {
            if (extreme < 0) { extreme *= -1; input1ExtremePriority.text = extreme.ToString(); }
            referencer.GameFlowFramework_Environment.extremePatchSpawnPriority = extreme;
        }
    }

    public void Gamemode2GetValues()
    {
        int patchAmount;
        if (int.TryParse(input2PatchAmount.text, out patchAmount))
        {
            if (patchAmount < 0) { patchAmount *= -1; input2PatchAmount.text = patchAmount.ToString(); }
            referencer.GameFlowFramework_Environment.patchAmount = patchAmount;
        }

        float diff;
        if (float.TryParse(input2Difficulty.text, out diff))
        {
            if (diff < 0) { diff *= -1; input2Difficulty.text = diff.ToString(); }
            referencer.GameFlowFramework_Environment.totalPatchDifficulty = diff;
        }

        int iterations;
        if (int.TryParse(input2Iterations.text, out iterations))
        {
            if (iterations < 0) { iterations *= -1; input2Iterations.text = iterations.ToString(); }
            referencer.GameFlowFramework_Environment.iterations = iterations;
        }

        float easy;
        if (float.TryParse(input2EasyDiff.text, out easy))
        {
            if (easy < 0) { easy *= -1; input2EasyDiff.text = easy.ToString(); }
            referencer.GameFlowFramework_Environment.easyPatchDif = easy;
        }

        float medium;
        if (float.TryParse(input2MediumDiff.text, out medium))
        {
            if (medium < 0) { medium *= -1; input2MediumDiff.text = medium.ToString(); }
            referencer.GameFlowFramework_Environment.mediumPatchDif = medium;
        }

        float hard;
        if (float.TryParse(input2HardDiff.text, out hard))
        {
            if (hard < 0) { hard *= -1; input2HardDiff.text = hard.ToString(); }
            referencer.GameFlowFramework_Environment.hardPatchDif = hard;
        }

        float extreme;
        if (float.TryParse(input2ExtremeDiff.text, out extreme))
        {
            if (extreme < 0) { extreme *= -1; input2ExtremeDiff.text = extreme.ToString(); }
            referencer.GameFlowFramework_Environment.extremePatchDif = extreme;
        }
    }

    public void Gamemode3GetValues()
    {
        int patchAmount;
        if (int.TryParse(input3PatchAmount.text, out patchAmount))
        {
            if (patchAmount < 0) { patchAmount *= -1; input3PatchAmount.text = patchAmount.ToString(); }
            referencer.GameFlowFramework_Environment.patchAmount = patchAmount;
        }

        float diff;
        if (float.TryParse(input3Difficulty.text, out diff))
        {
            if (diff < 0) { diff *= -1; input3Difficulty.text = diff.ToString(); }
            referencer.GameFlowFramework_Environment.totalPatchDifficulty = diff;
        }

        int iterations;
        if (int.TryParse(input3Iterations.text, out iterations))
        {
            if (iterations < 0) { iterations *= -1; input3Iterations.text = iterations.ToString(); }
            referencer.GameFlowFramework_Environment.iterations = iterations;
        }

        float easy;
        if (float.TryParse(input3EasyDiff.text, out easy))
        {
            if (easy < 0) { easy *= -1; input3EasyDiff.text = easy.ToString(); }
            referencer.GameFlowFramework_Environment.easyPatchDif = easy;
        }

        float medium;
        if (float.TryParse(input3MediumDiff.text, out medium))
        {
            if (medium < 0) { medium *= -1; input3MediumDiff.text = medium.ToString(); }
            referencer.GameFlowFramework_Environment.mediumPatchDif = medium;
        }

        float hard;
        if (float.TryParse(input3HardDiff.text, out hard))
        {
            if (hard < 0) { hard *= -1; input3HardDiff.text = hard.ToString(); }
            referencer.GameFlowFramework_Environment.hardPatchDif = hard;
        }

        float extreme;
        if (float.TryParse(input3ExtremeDiff.text, out extreme))
        {
            if (extreme < 0) { extreme *= -1; input3ExtremeDiff.text = extreme.ToString(); }
            referencer.GameFlowFramework_Environment.extremePatchDif = extreme;
        }
    }

    public void Gamemode4GetValues()
    {
        /*
        string seed = input4Seed.text;

        string pureSeed = "";
        int r;

        for (int j = 0; j < seed.Length; j++)
        {
            if (seed[j]+"" == "0" || seed[j] + "" == "1" || seed[j] + "" == "2"
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
        input4Seed.text = pureSeed;
        */
        referencer.GameFlowFramework_Environment.patchSeed = input4Seed.text;

        int i;

        if (int.TryParse(input4Iterations.text, out i))
        {
            if (i == 0) { i = 1; }
            if (i < 0) { i *= -1; input4Iterations.text = i.ToString(); }
            referencer.GameFlowFramework_Environment.totalIterations = i;
        }
        float f;
        if (float.TryParse(input4Diff.text, out f))
        {
            referencer.GameFlowFramework_Environment.difficultyConstant = f;
        }

        referencer.GameFlowFramework_Environment.isEndless = input4Endless.isOn;
        referencer.GameFlowFramework_Environment.diffIsActive = input4DiffActive.isOn;
        referencer.GameFlowFramework_Environment.usePercentage = input4UsePercentage.isOn;
    }

    public void Gamemode1StartAlgorithm()
    {
        if (input1EasyPriority.text == "" || input1EasyPriority.text == null ||
            input1MediumPriority.text == "" || input1MediumPriority.text == null ||
            input1HardPriority.text == "" || input1HardPriority.text == null ||
            input1ExtremePriority.text == "" || input1ExtremePriority.text == null ||
            input1EmptyPriority.text == "" || input1EmptyPriority.text == null)
        {
            SwitchCanvas(8);//8: ErrorScreen
            return;
        }

        StartGame();
    }

    public void Gamemode2StartAlgorithm()
    {
        if (input2PatchAmount.text == "" || input2PatchAmount.text == null ||
            input2Difficulty.text == "" || input2Difficulty.text == null ||
            input2Iterations.text == "" || input2Iterations.text == null ||
            input2EasyDiff.text == "" || input2EasyDiff.text == null ||
            input2MediumDiff.text == "" || input2MediumDiff.text == null ||
            input2HardDiff.text == "" || input2HardDiff.text == null ||
            input2ExtremeDiff.text == "" || input2ExtremeDiff.text == null)
        {
            SwitchCanvas(8);//8: ErrorScreen
            return;
        }

        StartGame();
    }

    public void Gamemode3StartAlgorithm()
    {
        if (input3PatchAmount.text == "" || input3PatchAmount.text == null ||
            input3Difficulty.text == "" || input3Difficulty.text == null ||
            input3Iterations.text == "" || input3Iterations.text == null ||
            input3EasyDiff.text == "" || input3EasyDiff.text == null ||
            input3MediumDiff.text == "" || input3MediumDiff.text == null ||
            input3HardDiff.text == "" || input3HardDiff.text == null ||
            input3ExtremeDiff.text == "" || input3ExtremeDiff.text == null)
        {
            SwitchCanvas(8);//8: ErrorScreen
            return;
        }

        StartGame();
    }

    public void Gamemode4StartAlgorithm()
    {
        if ((input4Seed.text == "" || input4Seed.text == null)
            || 
            (input4DiffActive.isOn == true && (input4Diff.text == "" || input4Diff.text == null))
            ||
            (input4Endless.isOn == false && (input4Iterations.text == "" || input4Iterations.text == null))
            )
        {
            SwitchCanvas(8);//8: ErrorScreen
            return;
        }

        Gamemode4GetValues();
        StartGame();
    }

    public void FocusGroupStart(int i)
    {
        focusStudyMode = true;
        if (identification.text == null || identification.text == "")
        {
            SwitchCanvas(8);//8: ErrorScreen
            return;
        }
        
        referencer.GameFlowFramework_WinLoseCondition.focusMode = i;
        referencer.GameFlowFramework_Environment.SetGameMode(4);
        referencer.GameFlowFramework_Environment.gameMode_MakeMyOwnScript.uiIndex = i - 1;
        referencer.GameFlowFramework_Environment.gameMode_MakeMyOwnScript.SetPreset(i - 1);
        referencer.GeneralSettings.uiIndex = i - 1;
        referencer.GeneralSettings.SetPreset(i - 1);
        Gamemode4StartAlgorithm();
    }

    public void click()
    {
        referencer.effect_click.Play();
    }

    void StartGame()
    {

        referencer.GameFlowFramework_Questions.Init();
        referencer.GameFlowFramework_Environment.Init();

        click();
        referencer.soundtrack1.Stop();
        referencer.soundtrack2.Play();
        canvasCurrent.SetActive(false);
        canvas[5].SetActive(true); //5: GameScreen
        gameHasStarted = true;
        StopTime(false);
    }

    public void PrintCurrentStats(string header) //for debugging
    {
        Debug.Log("[START] <<<" + header + ">>>  [START]");
        Debug.Log("- Health: " + healthCount);
        Debug.Log("- Hits: " + hitsCount);
        Debug.Log("- Score: " + scoreCount);
        Debug.Log("- Time: " + gameTime);
        Debug.Log("- Real Time: " + realTimeCountInt);
        Debug.Log("- currentPatch: " + referencer.GameFlowFramework_Environment.flowObjects_current);
        Debug.Log("- totalEntered: " + referencer.GameFlowFramework_Environment.flowObjects_totalEntered);
        Debug.Log("- totalRegularEntered: " + referencer.GameFlowFramework_Environment.flowObjects_totalRegularEntered);
        Debug.Log("- totalSpawned: " + referencer.GameFlowFramework_Environment.flowObjects_totalSpawned);
        Debug.Log("- totalRegularSpawned: " + referencer.GameFlowFramework_Environment.flowObjects_totalRegularSpawned);
        Debug.Log("[END] <<<" + header + ">>> [END]");
    }

    public void PrintFlowObject(string header) //for debugging
    {
        Debug.Log("[START] <<<" + header + ">>>  [START]");
        Debug.Log("Total Size: " + referencer.GameFlowFramework_Environment.flowObjects.Count);
        for (int i = 0; i < referencer.GameFlowFramework_Environment.flowObjects.Count; i++)
        {
            Debug.Log("flowObjects[" + i + "]: " + referencer.GameFlowFramework_Environment.flowObjects[i].patch.name);
            Debug.Log("--- Located at " + referencer.GameFlowFramework_Environment.flowObjects[i].patch.transform.position);
            Debug.Log("--- Spawned at " + referencer.GameFlowFramework_Environment.flowObjects[i].spawnTime);
        }
        Debug.Log("[END] <<<" + header + ">>> [END]");
    }
}
