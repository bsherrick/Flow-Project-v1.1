
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class FlowCanvas : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ReportResults(string str);

    //Constants
    const int canvasIndex_Starting = 0;
    const int canvasIndex_Password = 1;
    const int canvasIndex_GameEnd = 2;
    const int canvasIndex_Setting = 3;
    const int canvasIndex_Pause = 4;
    const int canvasIndex_GameConfig = 5;
    const int canvasIndex_QuestionConfig = 6;
    const int canvasIndex_AlgoritmConfig = 7;
    const int patchA = 0;
    const int patchB = 1;
    const int patchC = 2;
    const int patchD = 3;
    const int patchE = 4;

    [HideInInspector] public FlowMain flow;

    [Tooltip("Display debugging messages?")] public bool showDebug;

    [Header("References:")]
    #region
    [Tooltip("The game camera.")] public Camera cam;
    #endregion

    [Header("Settings (Editor Only):")]
    #region
    [Tooltip("Camera default field of view.")] public float fovDefault = 60;
    [Tooltip("Camera field of view when zoomed.")] public float fovZoomed = 59;
    [Tooltip("Duration of zooming in seconds.")] public float fovDuration = 0.5f;
    #endregion

    [Header("Settings (To be configured by Game Settings):")]
    #region
    public bool ui_showHealth = false;
    public bool ui_showScore = false;
    public bool ui_showTime = false;
    public bool ui_showProgressBar = false;
    public bool ui_showObjective = false;
    public string ui_textHealth = "";
    public string ui_textScore = "";
    public string ui_textObjective = "";
    public string ui_textWinMessage = "";
    public string ui_textLoseMessage = "";
    #endregion

    [Header("Status (No Reset):")]
    #region
    public int configIndex;
    public int canvasIndex;
    #endregion

    #region CANVASES

    [Header("In-Game Classic:")]
    #region
    public GameObject inGameClassic; //[Tooltip("In-Game Classic mode canvas object.")] 
    public Slider inGameClassic_sliderProgress;
    public Text inGameClassic_textTimer;
    public Text inGameClassic_textHealth;
    public Text inGameClassic_textScore;
    public Text inGameClassic_textObjective;
    public float inGameClassic_FloatAmp = 10f;
    public float inGameClassic_FloatFreq = 15f;
    public float inGameClassic_FloatTime = 0.2f;
    #endregion

    [Header("In-Game Rhythm:")]
    #region
    [Tooltip("In-Game Rhythm mode canvas object.")]
    public GameObject inGameRhythm;
    public Slider inGameRhythm_sliderProgress;
    public Text inGameRhythm_textTimer;
    public Text inGameRhythm_textHit;
    public Text inGameRhythm_textAccuracy;
    public float inGameRhythm_textHitFloatAmp = 10f;
    public float inGameRhythm_textHitFloatFreq = 15f;
    public float inGameRhythm_textHitFloatTime = 0.2f;
    #endregion

    [Header("Loading Screen:")]
    #region
    [Tooltip("Loading screen canvas object.")]
    public GameObject loadingScreen;
    public Text loadingScreen_info;
    #endregion

    [Header("Starting Screen:")]
    #region
    [Tooltip("Starting screen canvas object.")]
    public GameObject startingScreen;
    public InputField startingScreen_inputFieldID;
    #endregion

    [Header("Password Screen:")]
    #region
    [Tooltip("Password screen canvas object.")]
    public GameObject passwordScreen;
    public InputField startingScreen_inputFieldPass;
    #endregion

    [Header("Pause Screen:")]
    #region
    [Tooltip("Pause screen canvas object.")]
    public GameObject pauseScreen;
    #endregion

    [Header("GameEnd Screen:")]
    #region
    [Tooltip("GameEnd screen canvas object.")]
    public GameObject gameEndScreen;
    public Text gameEndScreen_message;
    #endregion

    [Header("Pick Settings Screen:")]
    #region
    [Tooltip("Pick Settings screen canvas object.")]
    public GameObject pickSettingsScreen;
    #endregion

    [Header("Game Config Screen:")]
    #region
    [Tooltip("Game Config screen canvas object.")]
    public GameObject gameConfigScreen; 
    public Text gameConfig_TITLE;
    public Slider gameConfig_SLIDER;
    //general
    public InputField gameConfig_gameMode;
    public InputField gameConfig_delayStart;
    public InputField gameConfig_delayEnd;
    //map editor
    //[DEPRECIATED] public InputField gameConfig_mapping;
    public Text gameConfig_currentPatch;
    public InputField gameConfig_patchArrangement;
    public InputField[] gameConfig_lineUnitA; //12 rows * 5 units = 60 line units
    public InputField[] gameConfig_lineUnitB; //12 rows * 5 units = 60 line units
    public InputField[] gameConfig_lineUnitC; //12 rows * 5 units = 60 line units
    public InputField[] gameConfig_lineUnitD; //12 rows * 5 units = 60 line units
    public InputField[] gameConfig_lineUnitE; //12 rows * 5 units = 60 line units
    public GameObject gameConfig_panelPatchA;
    public GameObject gameConfig_panelPatchB;
    public GameObject gameConfig_panelPatchC;
    public GameObject gameConfig_panelPatchD;
    public GameObject gameConfig_panelPatchE;
    public InputField gameConfig_speed;
    public InputField gameConfig_repeat;
    public Toggle gameConfig_endless;
    //gameplay
    public InputField gameConfig_wHealth;
    public InputField gameConfig_wScore;
    public InputField gameConfig_wTime;
    public InputField gameConfig_lHealth;
    public InputField gameConfig_lScore;
    public InputField gameConfig_lTime;
    public Toggle gameConfig_wHealthOn;
    public Toggle gameConfig_wScoreOn;
    public Toggle gameConfig_wTimeOn;
    public Toggle gameConfig_lHealthOn;
    public Toggle gameConfig_lScoreOn;
    public Toggle gameConfig_lTimeOn;
    public InputField gameConfig_sHealth;
    public InputField gameConfig_sScore;
    public InputField gameConfig_sTime;
    public Toggle gameConfig_countdown;
    //user interface
    public Toggle gameConfig_showHealth;
    public Toggle gameConfig_showScore;
    public Toggle gameConfig_showTime;
    public Toggle gameConfig_showProgress;
    public Toggle gameConfig_showObjective;
    public InputField gameConfig_HealthText;
    public InputField gameConfig_ScoreText;
    public InputField gameConfig_ObjectiveText;
    public InputField gameConfig_WinText;
    public InputField gameConfig_LoseText;
    #endregion

    
    [Header("Algorithm Config Screen:")]
    #region
    [Tooltip("Game Config screen canvas object.")]
    public GameObject algorithmSettingsScreen;
    public Text algorithmConfig_TITLE;
    public Slider algorithmConfig_SLIDER;

    //general
    public InputField algorithmConfig_gameMode;
    public InputField algorithmConfig_delayStart;
    public InputField algorithmConfig_delayEnd;

    //algorithm editor
    public InputField algorithmConfig_speed;
    public InputField algorithmConfig_speedLines;
    public InputField algorithmConfig_lineCooldown;
    public InputField algorithmConfig_seed;
    public InputField algorithmConfig_difficultyLines;
    public InputField algorithmConfig_maxStarLines;
    public InputField algorithmConfig_minStarLines;
    public Toggle algorithmConfig_adaptive;
    public InputField algorithmConfig_adaptiveStars;
    public InputField algorithmConfig_adaptiveHits;

    //gameplay
    public InputField algorithmConfig_wHealth;
    public InputField algorithmConfig_wScore;
    public InputField algorithmConfig_wTime;
    public InputField algorithmConfig_lHealth;
    public InputField algorithmConfig_lScore;
    public InputField algorithmConfig_lTime;
    public Toggle algorithmConfig_wHealthOn;
    public Toggle algorithmConfig_wScoreOn;
    public Toggle algorithmConfig_wTimeOn;
    public Toggle algorithmConfig_lHealthOn;
    public Toggle algorithmConfig_lScoreOn;
    public Toggle algorithmConfig_lTimeOn;
    public InputField algorithmConfig_sHealth;
    public InputField algorithmConfig_sScore;
    public InputField algorithmConfig_sTime;
    public Toggle algorithmConfig_countdown;

    //user interface
    public Toggle algorithmConfig_showHealth;
    public Toggle algorithmConfig_showScore;
    public Toggle algorithmConfig_showTime;
    public Toggle algorithmConfig_showProgress;
    public Toggle algorithmConfig_showObjective;
    public InputField algorithmConfig_HealthText;
    public InputField algorithmConfig_ScoreText;
    public InputField algorithmConfig_ObjectiveText;
    public InputField algorithmConfig_WinText;
    public InputField algorithmConfig_LoseText;
    #endregion

    

    [Header("Questions Config Screen:")]
    #region
    [Tooltip("Questions Config screen canvas object.")]
    public GameObject questionsConfigScreen;
    public Text questionConfig_TITLE;
    public Slider questionConfig_SLIDER;
    //general
    public InputField questionConfig_question;
    public Toggle questionConfig_active;
    public InputField questionConfig_repeat;
    public Toggle questionConfig_noExpire;
    public InputField questionConfig_type;
    public InputField questionConfig_delayStart;
    public InputField questionConfig_delayEnd;
    public InputField questionConfig_speed;
    //trigger & rewards
    public InputField questionConfig_health;
    public InputField questionConfig_score;
    public InputField questionConfig_line;
    public InputField questionConfig_time;
    public Toggle questionConfig_healthOn;
    public Toggle questionConfig_scoreOn;
    public Toggle questionConfig_lineOn;
    public Toggle questionConfig_timeOn;
    public InputField questionConfig_addHealth;
    public InputField questionConfig_addScore;
    //question type: slot
    public InputField questionConfig_slot1;
    public InputField questionConfig_slot2;
    public InputField questionConfig_slot3;
    public InputField questionConfig_slot4;
    public InputField questionConfig_slot5;
    public Toggle questionConfig_slot1On;
    public Toggle questionConfig_slot2On;
    public Toggle questionConfig_slot3On;
    public Toggle questionConfig_slot4On;
    public Toggle questionConfig_slot5On;
    //quesiton type: spectrum
    public InputField questionConfig_min;
    public InputField questionConfig_max;
    public InputField questionConfig_round;
    #endregion

    #endregion

    int currentPatchIndex = 0; //min 0, max 4

    void Start()
    {
        //assign inputfields
        for (int i = 0; i < 60; i++)
        {
            gameConfig_lineUnitA[i] = gameConfig_panelPatchA.transform.GetChild(i).GetComponent<InputField>();
            gameConfig_lineUnitB[i] = gameConfig_panelPatchB.transform.GetChild(i).GetComponent<InputField>();
            gameConfig_lineUnitC[i] = gameConfig_panelPatchC.transform.GetChild(i).GetComponent<InputField>();
            gameConfig_lineUnitD[i] = gameConfig_panelPatchD.transform.GetChild(i).GetComponent<InputField>();
            gameConfig_lineUnitE[i] = gameConfig_panelPatchE.transform.GetChild(i).GetComponent<InputField>();
        }
    }

    public void Initialize()
    {
        configIndex = 0;
        canvasIndex = canvasIndex_Starting;
        GameplayClassicUI_Init();
        
    }

    /// <summary>
    /// Set the game UI (only for Classic gameplays)
    /// </summary>
    public void GameplayClassicUI_Init()
    {
        inGameClassic_textHealth.text = ui_textHealth;
        inGameClassic_textScore.text = ui_textScore;
        inGameClassic_textObjective.text = ui_textObjective;
        inGameClassic_textHealth.enabled = ui_showHealth;
        inGameClassic_textScore.enabled = ui_showScore;
        inGameClassic_textObjective.enabled = ui_showObjective;
        inGameClassic_sliderProgress.enabled = ui_showProgressBar;
    }

    void Update()
    {
        if (!flow.gameOn)
        {
            return;
        }

        #region Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            if (flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_Rhythm) //pause music in rhythm mode
            {
                flow.audioRhythm1.Pause();
            }
            canvasIndex = canvasIndex_Pause;
            pauseScreen.SetActive(true);
        }
        #endregion

        #region camera
        //camera zoom
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)
            || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2)
            || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4)
            || Input.GetKeyDown(KeyCode.Alpha5))
        {
            CameraZoom(true);
        }

        else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)
            || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D)
            || Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Alpha2)
            || Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Alpha4)
            || Input.GetKeyUp(KeyCode.Alpha5))
        {
            CameraZoom(false);
        }
        #endregion
    }

    public void CanvasButtons(int button)
    {
        switch (canvasIndex)
        {
            case canvasIndex_Starting:
                #region startingscreen
                switch (button)
                {
                    case -1: //ALGORITHM
                        //Debug.Log("Play Algorithm");
                        flow.StartGame(-1);
                        inGameClassic.SetActive(true);
                        startingScreen.SetActive(false);
                        break;
                    case 0: //001
                        flow.StartGame(0);
                        inGameClassic.SetActive(true);
                        startingScreen.SetActive(false);
                        break;
                    case 1: //002
                        flow.StartGame(1);
                        inGameClassic.SetActive(true);
                        startingScreen.SetActive(false);
                        break;
                    case 2: //003
                        flow.StartGame(2);
                        inGameClassic.SetActive(true);
                        startingScreen.SetActive(false);
                        break;
                    case 3: //bonus
                        flow.StartGame(3);
                        inGameRhythm.SetActive(true);
                        startingScreen.SetActive(false);
                        break;
                    case 4: //password
                        canvasIndex = canvasIndex_Password;
                        passwordScreen.SetActive(true);
                        startingScreen.SetActive(false);
                        break;
                }
                break;
            #endregion
            case canvasIndex_Password:
                #region password
                switch (button)
                {
                    case 0: //Back
                        canvasIndex = canvasIndex_Starting;
                        startingScreen.SetActive(true);
                        passwordScreen.SetActive(false); 
                        break;
                    case 1: //ENTER
                        if (startingScreen_inputFieldPass.text == "coconutpie")
                        {
                            canvasIndex = canvasIndex_Setting;
                            pickSettingsScreen.SetActive(true);
                            passwordScreen.SetActive(false);
                        }
                        else if (startingScreen_inputFieldPass.text == "AlgorithmConfig")
                        {
                            //Debug.Log("Config Algorithm");
                            canvasIndex = canvasIndex_AlgoritmConfig;
                            getAlgorithmVals();
                            algorithmSettingsScreen.SetActive(true);
                            passwordScreen.SetActive(false);
                        }
                        break;
                }
                break;
            #endregion
            case canvasIndex_GameEnd:
                #region gameEnd
                switch (button)
                {
                    case 0: //Back
                        BackToMenu(gameEndScreen);
                        break;
                }
                break;
            #endregion
            case canvasIndex_Setting:
                #region pickSettings
                switch (button)
                {
                    case 0: //Back
                        canvasIndex = canvasIndex_Password;
                        passwordScreen.SetActive(true);
                        pickSettingsScreen.SetActive(false);
                        break;
                    case 1: //001
                        CanvasGameMode(button);
                        break;
                    case 2: //002
                        CanvasGameMode(button);
                        break;
                    case 3: //003
                        CanvasGameMode(button);
                        break;
                    case 4: //1
                        CanvasQuestionMode(button);
                        break;
                    case 5: //2
                        CanvasQuestionMode(button);
                        break;
                    case 6: //3
                        CanvasQuestionMode(button);
                        break;
                    case 7: //4
                        CanvasQuestionMode(button);
                        break;
                    case 8: //5
                        CanvasQuestionMode(button);
                        break;
                    case 9: //6
                        CanvasQuestionMode(button);
                        break;
                    case 10: //7
                        CanvasQuestionMode(button);
                        break;
                    case 11: //8
                        CanvasQuestionMode(button);
                        break;
                    case 12: //9
                        CanvasQuestionMode(button);
                        break;
                    case 13: //10
                        CanvasQuestionMode(button);
                        break;
                }
                break;
            #endregion
            case canvasIndex_Pause:
                #region pauseScreen
                switch (button)
                {
                    case 0: //Back
                        pauseScreen.SetActive(false);
                        Time.timeScale = 1;
                        if (flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_Rhythm)
                        {
                            flow.audioRhythm1.Play();
                        }
                        break;
                    case 1: //Quit To Menu
                        BackToMenu(pauseScreen);
                        //Debug.Log("Quit to Menu");
                        break;
                }
                break;
            #endregion
            case canvasIndex_GameConfig:
                #region gameConfig
                switch (button)
                {
                    case 0: //Back
                        canvasIndex = canvasIndex_Setting;
                        gameConfig_currentPatch.text = "PATCH: A";
                        currentPatchIndex = 0;
                        gameConfig_panelPatchA.SetActive(true);
                        gameConfig_panelPatchB.SetActive(false);
                        gameConfig_panelPatchC.SetActive(false);
                        gameConfig_panelPatchD.SetActive(false);
                        gameConfig_panelPatchE.SetActive(false);
                        pickSettingsScreen.SetActive(true);
                        gameConfigScreen.SetActive(false);
                        break;
                    case 1: //Download
                        flow.FlowWebConfig.GetGameConfig(configIndex);
                        break;
                    case 2: //Upload
                        flow.FlowWebConfig.PostGameConfig(configIndex);
                        break;
                }
                break;
            #endregion
            case canvasIndex_QuestionConfig:
                #region questionConfig
                switch (button)
                {
                    case 0: //Back
                        canvasIndex = canvasIndex_Setting;
                        pickSettingsScreen.SetActive(true);
                        questionsConfigScreen.SetActive(false);
                        break;
                    case 1: //Download
                        flow.FlowWebConfig.GetQuestionConfig(configIndex);
                        break;
                    case 2: //Upload
                        flow.FlowWebConfig.PostQuestionConfig(configIndex);
                        break;
                }
                break;
            #endregion
            case canvasIndex_AlgoritmConfig:
                #region AlgoritmConfig
                switch (button)
                {
                    case 0: //Back
                        canvasIndex = canvasIndex_Password;
                        algorithmSettingsScreen.SetActive(false);
                        passwordScreen.SetActive(true);
                        break;
                    case 1: //Download
                        //flow.FlowWebConfig.GetQuestionConfig(configIndex);
                        break;
                    case 2: //Upload
                        setAlgorithmVals();
                        break;
                }
                break;
                #endregion
        }
    }

    /// <summary>
    /// Back to Menu from GameEnd and Pause Screens, resets the game status
    /// </summary>
    /// <param name="prevCanvas">The current canvas before returning to the Menu Screen</param>
    void BackToMenu(GameObject prevCanvas)
    {
        //remove all current lines in the game
        while (flow.FlowLineGenerator.linesInGame.Count > 0)
        {
            GameObject o = flow.FlowLineGenerator.linesInGame[0];
            flow.FlowLineGenerator.linesInGame.RemoveAt(0);
            Destroy(o);
        }

        //clear/reset targets (for rhythm mode)
        for (int i = 0; i < flow.Targets.Length; i++)
        {
            flow.FlowPlayerMovement.buttonPressed[i].SetActive(false);
            flow.Targets[i].lineObject.Clear();
        }

        //reset Questions
        flow.FlowWebConfig.ResetQuestions();

        //turn canvases on/off
        canvasIndex = canvasIndex_Starting;
        startingScreen.SetActive(true);
        prevCanvas.SetActive(false);
        inGameClassic.SetActive(false);
        inGameRhythm.SetActive(false);

        //reset game status
        flow.gameOn = false;
        flow.FlowQuestionHandler.Initialize();
        flow.FlowLineGenerator.Initialize();
        flow.FlowGameConfig.Initialize();

        //reset floating UI elements
        inGameClassic_textHealth.GetComponent<FloatAndRotate>().ForceReset();
        inGameClassic_textScore.GetComponent<FloatAndRotate>().ForceReset();
        inGameRhythm_textHit.GetComponent<FloatAndRotate>().ForceReset();
        inGameRhythm_textAccuracy.GetComponent<FloatAndRotate>().ForceReset();

        //reset player position to be in the middle
        flow.FlowPlayerMovement.playerCharacter.transform.position = new Vector3(flow.FlowPlayerMovement.playerCharacter.transform.position.x, flow.FlowPlayerMovement.playerCharacter.transform.position.y, flow.FlowPlayerMovement.targets[2].transform.position.z);
        flow.FlowPlayerMovement.playerCharacter.SetActive(true);

        //make target board visible
        for (int i = 0; i < flow.FlowPlayerMovement.targets.Length; i++)
        {
            flow.FlowPlayerMovement.targets[i].SetActive(true);
        }

        flow.FlowLineGenerator.ChangeLineSpeed(10f); //reset starWarp speed
        Time.timeScale = 1;
    }

    /// <summary>
    /// Turn on the Game Mode Configuration Screen
    /// </summary>
    /// <param name="button">Button index from the Settings Screen (refer to CanvasButtons())</param>
    void CanvasGameMode(int button)
    {
        canvasIndex = canvasIndex_GameConfig;
        configIndex = button - 1;
        gameConfig_TITLE.text = "GAME MODE 00" + (configIndex+1) + " SETTINGS";
        gameConfig_SLIDER.value = 100;
        GameConfig_SettingsToUI(configIndex);
        pickSettingsScreen.SetActive(false);
        gameConfigScreen.SetActive(true);
    }

    /// <summary>
    /// Turn on the Question Configuration Screen
    /// </summary>
    /// <param name="button">Button index from the Settings Screen (refer to CanvasButtons())</param>
    void CanvasQuestionMode(int button)
    {
        canvasIndex = canvasIndex_QuestionConfig;
        configIndex = button - 4;
        questionConfig_TITLE.text = "QUESTION No. " + (configIndex+1) + " SETTINGS";
        questionConfig_SLIDER.value = 100;
        QuestionsConfig_SettingsToUI(configIndex);
        pickSettingsScreen.SetActive(false);
        questionsConfigScreen.SetActive(true);
    }

    #region camera

    /// <summary>
    /// Camera zoom effect
    /// </summary>
    /// <param name="b">TRUE to zoom, FALSE to return to default</param>
    public void CameraZoom(bool b)
    {
        StopAllCoroutines();
        if (b)
        {
            StartCoroutine(CamZoom(cam.fieldOfView, fovZoomed, fovDuration));
        }
        else
        {
            StartCoroutine(CamZoom(cam.fieldOfView, fovDefault, fovDuration));
        }
    }

    IEnumerator CamZoom(float fromVal, float toVal, float duration)
    {
        float counter = 0f;

        while (counter < duration)
        {
            if (Time.timeScale == 0)
                counter += Time.unscaledDeltaTime;
            else
                counter += Time.deltaTime;

            float val = Mathf.Lerp(fromVal, toVal, counter / duration);
            cam.fieldOfView = val;
            yield return null;
        }
    }

    #endregion

    #region inGameClassic
    /// <summary>
    /// Update in-game UI (for Classic modes)
    /// </summary>
    /// <param name="select">
    /// 0: health,
    /// 1: score</param>
    /// <param name="text">The string to replace the selected UI element</param>
    public void inGameClassicTextChange(int select, string text)
    {
        switch (select)
        {
            case 0: //health
                inGameClassic_textHealth.text = text;
                inGameClassic_textHealth.GetComponent<FloatAndRotate>().Modify(-1, inGameClassic_FloatAmp,
                    inGameClassic_FloatFreq, inGameClassic_FloatTime);
                break;
            case 1: //score
                inGameClassic_textScore.text = text;
                inGameClassic_textScore.GetComponent<FloatAndRotate>().Modify(-1, inGameClassic_FloatAmp,
                    inGameClassic_FloatFreq, inGameClassic_FloatTime);
                break;
            case 2: //
                break;
        }
    }
    #endregion

    #region inGameRhythm

    /// <summary>
    /// Update in-game UI (for Rhythm mode)
    /// </summary>
    /// <param name="select">
    /// 0: accuracy,
    /// 1: hit,
    /// 2: timer
    /// </param>
    /// <param name="text">The string to replace the selected UI element</param>
    public void inGameRhythmTextChange(int select, string text)
    {
        switch (select)
        {
            case 0: //accuracy
                inGameRhythm_textAccuracy.text = text;
                break;
            case 1: //hit
                inGameRhythm_textHit.text = text;
                inGameRhythm_textHit.GetComponent<FloatAndRotate>().Modify(-1, inGameRhythm_textHitFloatAmp,
                    inGameRhythm_textHitFloatFreq, inGameRhythm_textHitFloatTime);
                break;
            case 2: //
                break;
        }
    }

    #endregion

    #region gameEndScreen
    /// <summary>
    /// Bring up the GameEnd Screen.
    /// </summary>
    /// <param name="b">TRUE if won, FALSE if lost.</param>
    public void GameEnd(bool b)
    {
        Time.timeScale = 0;
        canvasIndex = canvasIndex_GameEnd;
        //inGameClassic.SetActive(false);
        gameEndScreen.SetActive(true);
        if (b)
        {
            gameEndScreen_message.text = ui_textWinMessage;
        }
        else
        {
            gameEndScreen_message.text = ui_textLoseMessage;
        }
    }

    /// <summary>
    /// Shows the data collected from the play session.
    /// </summary>
    public void GameEndShowResults()
    {
        ReportResults(flow.FlowGameConfig.CurrentGameStatus());
    }

    [ContextMenu("GameEndShowResultsDebug()")]
    public void GameEndShowResultsDebug()
    {
        Debug.Log(flow.FlowGameConfig.CurrentGameStatus());
    }

    #endregion

    #region gameConfigScreen

    /// <summary>
    /// Set config UI to current settings.
    /// </summary>
    /// <param name="i">Game Setting number.</param>
    public void GameConfig_SettingsToUI(int i)
    {
        //general
        gameConfig_gameMode.text = flow.FlowWebConfig.gameSettings[i].g_gamePlay.ToString();
        gameConfig_delayStart.text = flow.FlowWebConfig.gameSettings[i].g_startDelay.ToString();
        gameConfig_delayEnd.text = flow.FlowWebConfig.gameSettings[i].g_endDelay.ToString();
        //map editor
        //gameConfig_mapping.text = SingleToMap(flow.FlowWebConfig.gameSettings[i].me_map);
        gameConfig_patchArrangement.text = flow.FlowWebConfig.gameSettings[i].me_patchArrangement;
        StringToPatch(0, flow.FlowWebConfig.gameSettings[i].me_patchA);
        StringToPatch(1, flow.FlowWebConfig.gameSettings[i].me_patchB);
        StringToPatch(2, flow.FlowWebConfig.gameSettings[i].me_patchC);
        StringToPatch(3, flow.FlowWebConfig.gameSettings[i].me_patchD);
        StringToPatch(4, flow.FlowWebConfig.gameSettings[i].me_patchE);
        gameConfig_speed.text = flow.FlowWebConfig.gameSettings[i].me_speed.ToString();
        gameConfig_repeat.text = flow.FlowWebConfig.gameSettings[i].me_repeat.ToString();
        gameConfig_endless.isOn = flow.FlowWebConfig.gameSettings[i].me_endless;
        //gameplay
        gameConfig_wHealth.text = flow.FlowWebConfig.gameSettings[i].gs_wHealth.ToString();
        gameConfig_wScore.text = flow.FlowWebConfig.gameSettings[i].gs_wScore.ToString();
        gameConfig_wTime.text = flow.FlowWebConfig.gameSettings[i].gs_wTime.ToString();
        gameConfig_lHealth.text = flow.FlowWebConfig.gameSettings[i].gs_lHealth.ToString();
        gameConfig_lScore.text = flow.FlowWebConfig.gameSettings[i].gs_lScore.ToString();
        gameConfig_lTime.text = flow.FlowWebConfig.gameSettings[i].gs_lTime.ToString();
        gameConfig_wHealthOn.isOn = flow.FlowWebConfig.gameSettings[i].gs_wHealthOn;
        gameConfig_wScoreOn.isOn = flow.FlowWebConfig.gameSettings[i].gs_wScoreOn;
        gameConfig_wTimeOn.isOn = flow.FlowWebConfig.gameSettings[i].gs_wTimeOn;
        gameConfig_lHealthOn.isOn = flow.FlowWebConfig.gameSettings[i].gs_lHealthOn;
        gameConfig_lScoreOn.isOn = flow.FlowWebConfig.gameSettings[i].gs_lScoreOn;
        gameConfig_lTimeOn.isOn = flow.FlowWebConfig.gameSettings[i].gs_lTimeOn;
        gameConfig_sHealth.text = flow.FlowWebConfig.gameSettings[i].gs_health.ToString();
        gameConfig_sScore.text = flow.FlowWebConfig.gameSettings[i].gs_score.ToString();
        gameConfig_sTime.text = flow.FlowWebConfig.gameSettings[i].gs_time.ToString();
        gameConfig_countdown.isOn = flow.FlowWebConfig.gameSettings[i].gs_timeCountdown;
        //user interface
        gameConfig_showHealth.isOn = flow.FlowWebConfig.gameSettings[i].ui_showHealth;
        gameConfig_showScore.isOn = flow.FlowWebConfig.gameSettings[i].ui_showScore;
        gameConfig_showTime.isOn = flow.FlowWebConfig.gameSettings[i].ui_showTime;
        gameConfig_showProgress.isOn = flow.FlowWebConfig.gameSettings[i].ui_showProgressBar;
        gameConfig_showObjective.isOn = flow.FlowWebConfig.gameSettings[i].ui_showObjective;
        gameConfig_HealthText.text = flow.FlowWebConfig.gameSettings[i].ui_textHealth;
        gameConfig_ScoreText.text = flow.FlowWebConfig.gameSettings[i].ui_textScore;
        gameConfig_ObjectiveText.text = flow.FlowWebConfig.gameSettings[i].ui_textObjective;
        gameConfig_WinText.text = flow.FlowWebConfig.gameSettings[i].ui_textWinMessage;
        gameConfig_LoseText.text = flow.FlowWebConfig.gameSettings[i].ui_textLoseMessage;
    }

    /// <summary>
    /// Set current settings to config UI.
    /// </summary>
    /// <param name="i">Game Mode number.</param>
    public void GameConfig_UIToSettings(int i)
    {
        //general
        flow.FlowWebConfig.gameSettings[i].g_gamePlay = IntCheck(gameConfig_gameMode.text, true, 0, 1);
        flow.FlowWebConfig.gameSettings[i].g_startDelay = FloatCheck(gameConfig_delayStart.text, true, 0, 100);
        flow.FlowWebConfig.gameSettings[i].g_endDelay = FloatCheck(gameConfig_delayEnd.text, true, 0, 100);
        //map editor
        /*
        if (gameConfig_mapping.text == "" || gameConfig_mapping.text == null)
        {
            gameConfig_mapping.text = flow.FlowWebConfig.lineMapSample;
        }
        flow.FlowWebConfig.gameSettings[i].me_map = CleanMapping(gameConfig_mapping.text);
        */
        flow.FlowWebConfig.gameSettings[i].me_patchArrangement = gameConfig_patchArrangement.text;
        flow.FlowWebConfig.gameSettings[i].me_patchA = PatchToString(0);
        flow.FlowWebConfig.gameSettings[i].me_patchB = PatchToString(1);
        flow.FlowWebConfig.gameSettings[i].me_patchC = PatchToString(2);
        flow.FlowWebConfig.gameSettings[i].me_patchD = PatchToString(3);
        flow.FlowWebConfig.gameSettings[i].me_patchE = PatchToString(4);
        flow.FlowWebConfig.gameSettings[i].me_speed = FloatCheck(gameConfig_speed.text, true, 0.1f, 100);
        flow.FlowWebConfig.gameSettings[i].me_repeat = IntCheck(gameConfig_repeat.text, true, 1, 100);
        flow.FlowWebConfig.gameSettings[i].me_endless = gameConfig_endless.isOn;
        //gameplay
        flow.FlowWebConfig.gameSettings[i].gs_wHealth = IntCheck(gameConfig_wHealth.text, false, 0, 0);
        flow.FlowWebConfig.gameSettings[i].gs_wScore = IntCheck(gameConfig_wScore.text, false, 0, 0);
        flow.FlowWebConfig.gameSettings[i].gs_wTime = IntCheck(gameConfig_wTime.text, false, 0, 0);
        flow.FlowWebConfig.gameSettings[i].gs_lHealth = IntCheck(gameConfig_lHealth.text, false, 0, 0);
        flow.FlowWebConfig.gameSettings[i].gs_lScore = IntCheck(gameConfig_lScore.text, false, 0, 0);
        flow.FlowWebConfig.gameSettings[i].gs_lTime = FloatCheck(gameConfig_lTime.text, false, 0, 0);
        flow.FlowWebConfig.gameSettings[i].gs_wHealthOn = gameConfig_wHealthOn.isOn;
        flow.FlowWebConfig.gameSettings[i].gs_wScoreOn = gameConfig_wScoreOn.isOn;
        flow.FlowWebConfig.gameSettings[i].gs_wTimeOn = gameConfig_wTimeOn.isOn;
        flow.FlowWebConfig.gameSettings[i].gs_lHealthOn = gameConfig_lHealthOn.isOn;
        flow.FlowWebConfig.gameSettings[i].gs_lScoreOn = gameConfig_lScoreOn.isOn;
        flow.FlowWebConfig.gameSettings[i].gs_lTimeOn = gameConfig_lTimeOn.isOn;
        flow.FlowWebConfig.gameSettings[i].gs_health = IntCheck(gameConfig_sHealth.text, false, 0, 0);
        flow.FlowWebConfig.gameSettings[i].gs_score = IntCheck(gameConfig_sScore.text, false, 0, 0);
        flow.FlowWebConfig.gameSettings[i].gs_time = IntCheck(gameConfig_sTime.text, false, 0, 0);
        flow.FlowWebConfig.gameSettings[i].gs_timeCountdown = gameConfig_countdown.isOn;
        //user interface
        flow.FlowWebConfig.gameSettings[i].ui_showHealth = gameConfig_showHealth.isOn;
        flow.FlowWebConfig.gameSettings[i].ui_showScore = gameConfig_showScore.isOn;
        flow.FlowWebConfig.gameSettings[i].ui_showTime = gameConfig_showTime.isOn;
        flow.FlowWebConfig.gameSettings[i].ui_showProgressBar = gameConfig_showProgress.isOn;
        flow.FlowWebConfig.gameSettings[i].ui_showObjective = gameConfig_showObjective.isOn;
        flow.FlowWebConfig.gameSettings[i].ui_textHealth = gameConfig_HealthText.text;
        flow.FlowWebConfig.gameSettings[i].ui_textScore = gameConfig_ScoreText.text;
        flow.FlowWebConfig.gameSettings[i].ui_textObjective = gameConfig_ObjectiveText.text;
        flow.FlowWebConfig.gameSettings[i].ui_textWinMessage = gameConfig_WinText.text;
        flow.FlowWebConfig.gameSettings[i].ui_textLoseMessage = gameConfig_LoseText.text;
    }

    public string GenerateGameConfig(int i)
    {
        string config = "";
        int n = 0;

        //general
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].g_gamePlay + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].g_startDelay + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].g_endDelay + '\t';
        //config += "[" + n++ + "]" + MapToSingle(flow.FlowWebConfig.gameSettings[i].me_map) + '\t';
        //config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].me_map + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].me_patchArrangement + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].me_patchA + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].me_patchB + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].me_patchC + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].me_patchD + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].me_patchE + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].me_speed + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].me_repeat + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].me_endless + '\t';
        //gameplay
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_wHealth + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_wScore + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_wTime + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_lHealth + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_lScore + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_lTime + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_wHealthOn + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_wScoreOn + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_wTimeOn + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_lHealthOn + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_lScoreOn + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_lTimeOn + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_health + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_score + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_time + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].gs_timeCountdown + '\t';
        //user interface
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].ui_showHealth + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].ui_showScore + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].ui_showTime + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].ui_showProgressBar + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].ui_showObjective + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].ui_textHealth + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].ui_textScore + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].ui_textObjective + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].ui_textWinMessage + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.gameSettings[i].ui_textLoseMessage + '\t';

        if (showDebug)
        {
            Debug.Log("START GAMECONFIG[" + i + "]");
            Debug.Log(config);
            Debug.Log("END GAMECONFIG[" + i + "]");
        }

        return config;
    }

    public void GameConfig_NextPatch()
    {
        gameConfig_panelPatchA.SetActive(false);
        gameConfig_panelPatchB.SetActive(false);
        gameConfig_panelPatchC.SetActive(false);
        gameConfig_panelPatchD.SetActive(false);
        gameConfig_panelPatchE.SetActive(false);
        if (currentPatchIndex == 4)
        {
            currentPatchIndex = 0;
        }
        else
        {
            currentPatchIndex++;
        }

        switch (currentPatchIndex)
        {
            case patchA: gameConfig_panelPatchA.SetActive(true); gameConfig_currentPatch.text = "PATCH: A"; break;
            case patchB: gameConfig_panelPatchB.SetActive(true); gameConfig_currentPatch.text = "PATCH: B"; break;
            case patchC: gameConfig_panelPatchC.SetActive(true); gameConfig_currentPatch.text = "PATCH: C"; break;
            case patchD: gameConfig_panelPatchD.SetActive(true); gameConfig_currentPatch.text = "PATCH: D"; break;
            case patchE: gameConfig_panelPatchE.SetActive(true); gameConfig_currentPatch.text = "PATCH: E"; break;
        }
    }

    #endregion

    #region questionsConfigScreen

    /// <summary>
    /// Set config UI to current settings.
    /// </summary>
    /// <param name="i">Question Number.</param>
    public void QuestionsConfig_SettingsToUI(int i)
    {
        //general
        questionConfig_question.text = flow.FlowWebConfig.questionSettings[i].g_question;
        questionConfig_active.isOn = flow.FlowWebConfig.questionSettings[i].g_active;
        questionConfig_repeat.text = flow.FlowWebConfig.questionSettings[i].g_repeatAmount.ToString();
        questionConfig_noExpire.isOn = flow.FlowWebConfig.questionSettings[i].g_neverExpire;
        questionConfig_type.text = flow.FlowWebConfig.questionSettings[i].qt_type.ToString();
        questionConfig_delayStart.text = flow.FlowWebConfig.questionSettings[i].e_delayStart.ToString();
        questionConfig_delayEnd.text = flow.FlowWebConfig.questionSettings[i].e_delayEnd.ToString();
        questionConfig_speed.text = flow.FlowWebConfig.questionSettings[i].e_lineSpeedSetTo.ToString();
        //trigger & rewards
        questionConfig_health.text = flow.FlowWebConfig.questionSettings[i].t_health.ToString();
        questionConfig_score.text = flow.FlowWebConfig.questionSettings[i].t_score.ToString();
        questionConfig_line.text = flow.FlowWebConfig.questionSettings[i].t_line.ToString();
        questionConfig_time.text = flow.FlowWebConfig.questionSettings[i].t_time.ToString();
        questionConfig_healthOn.isOn = flow.FlowWebConfig.questionSettings[i].t_healthOn;
        questionConfig_scoreOn.isOn = flow.FlowWebConfig.questionSettings[i].t_scoreOn;
        questionConfig_lineOn.isOn = flow.FlowWebConfig.questionSettings[i].t_lineOn;
        questionConfig_timeOn.isOn = flow.FlowWebConfig.questionSettings[i].t_timeOn; ;
        questionConfig_addHealth.text = flow.FlowWebConfig.questionSettings[i].rp_healthAdd.ToString();
        questionConfig_addScore.text = flow.FlowWebConfig.questionSettings[i].rp_scoreAdd.ToString();
        //question type: slot
        questionConfig_slot1.text = flow.FlowWebConfig.questionSettings[i].qt_ls1;
        questionConfig_slot2.text = flow.FlowWebConfig.questionSettings[i].qt_ls2;
        questionConfig_slot3.text = flow.FlowWebConfig.questionSettings[i].qt_ls3;
        questionConfig_slot4.text = flow.FlowWebConfig.questionSettings[i].qt_ls4;
        questionConfig_slot5.text = flow.FlowWebConfig.questionSettings[i].qt_ls5;
        questionConfig_slot1On.isOn = flow.FlowWebConfig.questionSettings[i].qt_ls1On;
        questionConfig_slot2On.isOn = flow.FlowWebConfig.questionSettings[i].qt_ls2On;
        questionConfig_slot3On.isOn = flow.FlowWebConfig.questionSettings[i].qt_ls3On;
        questionConfig_slot4On.isOn = flow.FlowWebConfig.questionSettings[i].qt_ls4On;
        questionConfig_slot5On.isOn = flow.FlowWebConfig.questionSettings[i].qt_ls5On;
        //quesiton type: spectrum
        questionConfig_min.text = flow.FlowWebConfig.questionSettings[i].qt_sMin.ToString();
        questionConfig_max.text = flow.FlowWebConfig.questionSettings[i].qt_sMax.ToString();
        questionConfig_round.text = flow.FlowWebConfig.questionSettings[i].qt_sDec.ToString();
    }

    /// <summary>
    /// Set current settings to config UI.
    /// </summary>
    /// <param name="i">Question Number.</param>
    public void QuestionsConfig_UIToSettings(int i)
    {
        //general
        flow.FlowWebConfig.questionSettings[i].g_question = questionConfig_question.text;
        flow.FlowWebConfig.questionSettings[i].g_active = questionConfig_active.isOn;
        flow.FlowWebConfig.questionSettings[i].g_repeatAmount = IntCheck(questionConfig_repeat.text, true, 0, 100);
        flow.FlowWebConfig.questionSettings[i].g_neverExpire = questionConfig_noExpire.isOn;
        flow.FlowWebConfig.questionSettings[i].qt_type = IntCheck(questionConfig_type.text, true, 1, 2);
        flow.FlowWebConfig.questionSettings[i].e_delayStart = FloatCheck(questionConfig_delayStart.text, true, 0, 100);
        flow.FlowWebConfig.questionSettings[i].e_delayEnd = FloatCheck(questionConfig_delayEnd.text, true, 0, 100);
        flow.FlowWebConfig.questionSettings[i].e_lineSpeedSetTo = FloatCheck(questionConfig_speed.text, true, 0.1f, 100);
        //trigger & rewards
        flow.FlowWebConfig.questionSettings[i].t_health = IntCheck(questionConfig_health.text, false, 0, 0);
        flow.FlowWebConfig.questionSettings[i].t_score = IntCheck(questionConfig_score.text, false, 0, 0);
        flow.FlowWebConfig.questionSettings[i].t_line = IntCheck(questionConfig_line.text, true, 1, 10000);
        flow.FlowWebConfig.questionSettings[i].t_time = FloatCheck(questionConfig_time.text, false, 0, 0);
        flow.FlowWebConfig.questionSettings[i].t_healthOn = questionConfig_healthOn.isOn;
        flow.FlowWebConfig.questionSettings[i].t_scoreOn = questionConfig_scoreOn.isOn;
        flow.FlowWebConfig.questionSettings[i].t_lineOn = questionConfig_lineOn.isOn;
        flow.FlowWebConfig.questionSettings[i].t_timeOn = questionConfig_timeOn.isOn;
        flow.FlowWebConfig.questionSettings[i].rp_healthAdd = IntCheck(questionConfig_addHealth.text, false, 0, 0);
        flow.FlowWebConfig.questionSettings[i].rp_scoreAdd = IntCheck(questionConfig_addScore.text, false, 0, 0);
        //question type: slot
        flow.FlowWebConfig.questionSettings[i].qt_ls1 = questionConfig_slot1.text;
        flow.FlowWebConfig.questionSettings[i].qt_ls2 = questionConfig_slot2.text;
        flow.FlowWebConfig.questionSettings[i].qt_ls3 = questionConfig_slot3.text;
        flow.FlowWebConfig.questionSettings[i].qt_ls4 = questionConfig_slot4.text;
        flow.FlowWebConfig.questionSettings[i].qt_ls5 = questionConfig_slot5.text;
        flow.FlowWebConfig.questionSettings[i].qt_ls1On = questionConfig_slot1On.isOn;
        flow.FlowWebConfig.questionSettings[i].qt_ls2On = questionConfig_slot2On.isOn;
        flow.FlowWebConfig.questionSettings[i].qt_ls3On = questionConfig_slot3On.isOn;
        flow.FlowWebConfig.questionSettings[i].qt_ls4On = questionConfig_slot4On.isOn;
        flow.FlowWebConfig.questionSettings[i].qt_ls5On = questionConfig_slot5On.isOn;
        //quesiton type: spectrum
        flow.FlowWebConfig.questionSettings[i].qt_sMin = FloatCheck(questionConfig_min.text, false, 0, 0);
        flow.FlowWebConfig.questionSettings[i].qt_sMax = FloatCheck(questionConfig_max.text, false, 0, 0);
        flow.FlowWebConfig.questionSettings[i].qt_sDec = IntCheck(questionConfig_round.text, true, 0, 10);
    }

    public string GenerateQuestionConfig(int i)
    {
        string config = "";
        int n = 0;

        //general
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].g_question + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].g_active + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].g_repeatAmount + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].g_neverExpire + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_type + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].e_delayStart + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].e_delayEnd + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].e_lineSpeedSetTo + '\t';
        //trigger & rewards
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].t_health + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].t_score + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].t_line + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].t_time + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].t_healthOn + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].t_scoreOn + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].t_lineOn + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].t_timeOn + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].rp_healthAdd + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].rp_scoreAdd + '\t';
        //question type: slot
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_ls1 + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_ls2 + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_ls3 + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_ls4 + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_ls5 + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_ls1On + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_ls2On + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_ls3On + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_ls4On + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_ls5On + '\t';
        //quesiton type: spectrum
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_sMin + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_sMax + '\t';
        config += "[" + n++ + "]" + flow.FlowWebConfig.questionSettings[i].qt_sDec + '\t';

        if (showDebug)
        {
            Debug.Log("START QUESTIONCONFIG[" + i + "]");
            Debug.Log(config);
            Debug.Log("END QUESTIONCONFIG[" + i + "]");
        }

        return config;

    }

    #endregion

    #region general

    /// <summary>
    /// Parses an int from a string.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="limit">TRUE to apply int bounds (min/max). FALSE for no bounds (ignore min/max).</param>
    /// <param name="min">Minimum value of the int.</param>
    /// <param name="max">Maximum value of the int.</param>
    /// <returns></returns>
    public int IntCheck(string s, bool limit, int min, int max)
    {
        int val;
        if (!int.TryParse(s, out val))
        {
            Debug.Log("PARSE FAILED! From " + s + " we got " + val);
        }
        if (limit && (val < min || val > max))
        {
            val = min;
        }
        return val;
    }

    /// <summary>
    /// Parses a float from a string.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="limit">TRUE to apply float bounds (min/max). FALSE for no bounds (ignore min/max).</param>
    /// <param name="min">Minimum value of the float.</param>
    /// <param name="max">Maximum value of the float.</param>
    /// <returns></returns>
    public float FloatCheck(string s, bool limit, float min, float max)
    {
        float val;
        if (!float.TryParse(s, out val))
        {
            Debug.Log("PARSE FAILED! From " + s + " we got " + val);
        }
        if (limit && (val < min || val > max))
        {
            val = min;
        }
        return val;
    }

    /// <summary>
    /// Converts a patch to a string.
    /// </summary>
    /// <param name="patch">Values 0,1,2,3,4 correspond to patches A,B,C,D,E respectively.</param>
    /// <returns></returns>
    public string PatchToString(int patch)
    {
        string result = "";
        switch (patch)
        {
            case patchA: for (int i = 0; i < 60; i++) { if (gameConfig_lineUnitA[i].text != ".") { result += gameConfig_lineUnitA[i].text; } }
                break;
            case patchB: for (int i = 0; i < 60; i++) { if (gameConfig_lineUnitB[i].text != ".") { result += gameConfig_lineUnitB[i].text; } }
                break;
            case patchC: for (int i = 0; i < 60; i++) { if (gameConfig_lineUnitC[i].text != ".") { result += gameConfig_lineUnitC[i].text; } }
                break;
            case patchD: for (int i = 0; i < 60; i++) { if (gameConfig_lineUnitD[i].text != ".") { result += gameConfig_lineUnitD[i].text; } }
                break;
            case patchE: for (int i = 0; i < 60; i++) { if (gameConfig_lineUnitE[i].text != ".") { result += gameConfig_lineUnitE[i].text; } }
                break;
        }
        return result;
    }

    /// <summary>
    /// Converts a string to a patch.
    /// </summary>
    /// <param name="patch">Values 0,1,2,3,4 correspond to patches A,B,C,D,E respectively.</param>
    /// <returns></returns>
    public void StringToPatch(int patch, string rawString)
    {
        int lineUnitIndex = 0;
        switch (patch)
        {
            case patchA:
                for (int i = 0; i < rawString.Length; i++) { gameConfig_lineUnitA[lineUnitIndex++].text = rawString[i].ToString(); }
                for (int i = lineUnitIndex; i < 60; i++) { gameConfig_lineUnitA[i].text = "."; }
                break;
            case patchB:
                for (int i = 0; i < rawString.Length; i++) { gameConfig_lineUnitB[lineUnitIndex++].text = rawString[i].ToString(); }
                for (int i = lineUnitIndex; i < 60; i++) { gameConfig_lineUnitB[i].text = "."; }
                break;
            case patchC:
                for (int i = 0; i < rawString.Length; i++) { gameConfig_lineUnitC[lineUnitIndex++].text = rawString[i].ToString(); }
                for (int i = lineUnitIndex; i < 60; i++) { gameConfig_lineUnitC[i].text = "."; }
                break;
            case patchD:
                for (int i = 0; i < rawString.Length; i++) { gameConfig_lineUnitD[lineUnitIndex++].text = rawString[i].ToString(); }
                for (int i = lineUnitIndex; i < 60; i++) { gameConfig_lineUnitD[i].text = "."; }
                break;
            case patchE:
                for (int i = 0; i < rawString.Length; i++) { gameConfig_lineUnitE[lineUnitIndex++].text = rawString[i].ToString(); }
                for (int i = lineUnitIndex; i < 60; i++) { gameConfig_lineUnitE[i].text = "."; }
                break;
        }
    }

    #endregion

    public void getAlgorithmVals()
    {
        //general
        if (flow.AlgorithmValues.gamePlay == FlowGameConfig.gamePlay_AlgorithmSmooth)
            algorithmConfig_gameMode.text = "0";
        if (flow.AlgorithmValues.gamePlay == FlowGameConfig.gamePlay_AlgorithmChop)
            algorithmConfig_gameMode.text = "1";
        algorithmConfig_delayStart.text = flow.AlgorithmValues.delayStart + "";
        algorithmConfig_delayEnd.text = flow.AlgorithmValues.delayEnd + "";

        //algorithm editor
        if (flow.AlgorithmValues.setSeed)
            algorithmConfig_seed.text = flow.AlgorithmValues.algorithmSeed + "";
        else
            algorithmConfig_seed.text = "-";

        algorithmConfig_speed.text = flow.AlgorithmValues.algorithmSpeed + "";
        algorithmConfig_speedLines.text = flow.AlgorithmValues.speedChangeLines + "";
        algorithmConfig_lineCooldown.text = flow.AlgorithmValues.maxLineCooldown + "";
        algorithmConfig_difficultyLines.text = flow.AlgorithmValues.difficultyChangeLines + "";
        algorithmConfig_maxStarLines.text = flow.AlgorithmValues.maxBetweenStars + "";
        algorithmConfig_minStarLines.text = flow.AlgorithmValues.minBetweenStars + "";
        algorithmConfig_adaptive.isOn = flow.AlgorithmValues.isAdaptiveDifficulty;
        algorithmConfig_adaptiveStars.text = flow.AlgorithmValues.adaptiveStars + "";
        algorithmConfig_adaptiveHits.text = flow.AlgorithmValues.adaptiveRocks + "";

        //gameplay
        algorithmConfig_wHealth.text = flow.AlgorithmValues.w_health + "";
        algorithmConfig_wScore.text = flow.AlgorithmValues.w_score + "";
        algorithmConfig_wTime.text = flow.AlgorithmValues.w_time + "";
        algorithmConfig_lHealth.text = flow.AlgorithmValues.l_health + "";
        algorithmConfig_lScore.text = flow.AlgorithmValues.l_score + "";
        algorithmConfig_lTime.text = flow.AlgorithmValues.l_time + "";
        algorithmConfig_wHealthOn.isOn = flow.AlgorithmValues.w_healthOn;
        algorithmConfig_wScoreOn.isOn = flow.AlgorithmValues.w_scoreOn;
        algorithmConfig_wTimeOn.isOn = flow.AlgorithmValues.w_timeOn;
        algorithmConfig_lHealthOn.isOn = flow.AlgorithmValues.l_healthOn;
        algorithmConfig_lScoreOn.isOn = flow.AlgorithmValues.l_scoreOn;
        algorithmConfig_lTimeOn.isOn = flow.AlgorithmValues.l_timeOn;
        algorithmConfig_sHealth.text = flow.AlgorithmValues.sHealth + "";
        algorithmConfig_sScore.text = flow.AlgorithmValues.sScore + "";
        algorithmConfig_sTime.text = flow.AlgorithmValues.sTime + "";
        algorithmConfig_countdown.isOn = flow.AlgorithmValues.timerCountdown;

        //user interface
        algorithmConfig_showHealth.isOn = flow.AlgorithmValues.ui_showHealth;
        algorithmConfig_showScore.isOn = flow.AlgorithmValues.ui_showScore;
        algorithmConfig_showTime.isOn = flow.AlgorithmValues.ui_showTime;
        algorithmConfig_showProgress.isOn = flow.AlgorithmValues.ui_showProgressBar;
        algorithmConfig_showObjective.isOn = flow.AlgorithmValues.ui_showObjective;
        algorithmConfig_HealthText.text = flow.AlgorithmValues.ui_textHealth;
        algorithmConfig_ScoreText.text = flow.AlgorithmValues.ui_textScore;
        algorithmConfig_ObjectiveText.text = flow.AlgorithmValues.ui_textObjective;
        algorithmConfig_WinText.text = flow.AlgorithmValues.ui_textWinMessage;
        algorithmConfig_LoseText.text = flow.AlgorithmValues.ui_textLoseMessage;
    }

    public void setAlgorithmVals()
    {
        //general
        if (algorithmConfig_gameMode.text.Equals("0"))
            flow.AlgorithmValues.gamePlay = FlowGameConfig.gamePlay_AlgorithmSmooth;
        if (algorithmConfig_gameMode.text.Equals("1"))
            flow.AlgorithmValues.gamePlay = FlowGameConfig.gamePlay_AlgorithmChop;
        flow.AlgorithmValues.delayStart = int.Parse(algorithmConfig_delayStart.text);
        flow.AlgorithmValues.delayEnd = int.Parse(algorithmConfig_delayEnd.text);

        //algorithm editor
        if (algorithmConfig_seed.text.Equals("-") || algorithmConfig_seed.text.Equals(""))
            flow.AlgorithmValues.setSeed = false;
        else
        {
            flow.AlgorithmValues.setSeed = true;
            flow.AlgorithmValues.algorithmSeed = int.Parse(algorithmConfig_seed.text);
        }

        flow.AlgorithmValues.algorithmSpeed = int.Parse(algorithmConfig_speed.text);
        flow.AlgorithmValues.speedChangeLines = int.Parse(algorithmConfig_speedLines.text);
        flow.AlgorithmValues.maxLineCooldown = int.Parse(algorithmConfig_lineCooldown.text);
        flow.AlgorithmValues.difficultyChangeLines = int.Parse(algorithmConfig_difficultyLines.text);
        flow.AlgorithmValues.maxBetweenStars = int.Parse(algorithmConfig_maxStarLines.text);
        flow.AlgorithmValues.minBetweenStars = int.Parse(algorithmConfig_minStarLines.text);
        flow.AlgorithmValues.isAdaptiveDifficulty = algorithmConfig_adaptive.isOn;
        flow.AlgorithmValues.adaptiveStars = int.Parse(algorithmConfig_adaptiveStars.text);
        flow.AlgorithmValues.adaptiveRocks = int.Parse(algorithmConfig_adaptiveHits.text);

        //gameplay
        flow.AlgorithmValues.w_health = int.Parse(algorithmConfig_wHealth.text);
        flow.AlgorithmValues.w_score = int.Parse(algorithmConfig_wScore.text);
        flow.AlgorithmValues.w_time = int.Parse(algorithmConfig_wTime.text);
        flow.AlgorithmValues.l_health = int.Parse(algorithmConfig_lHealth.text);
        flow.AlgorithmValues.l_score = int.Parse(algorithmConfig_lScore.text);
        flow.AlgorithmValues.l_time = int.Parse(algorithmConfig_lTime.text);
        flow.AlgorithmValues.w_healthOn = algorithmConfig_wHealthOn.isOn;
        flow.AlgorithmValues.w_scoreOn = algorithmConfig_wScoreOn.isOn;
        flow.AlgorithmValues.w_timeOn = algorithmConfig_wTimeOn.isOn;
        flow.AlgorithmValues.l_healthOn = algorithmConfig_lHealthOn.isOn;
        flow.AlgorithmValues.l_scoreOn = algorithmConfig_lScoreOn.isOn;
        flow.AlgorithmValues.l_timeOn = algorithmConfig_lTimeOn.isOn;
        flow.AlgorithmValues.sHealth = int.Parse(algorithmConfig_sHealth.text);
        flow.AlgorithmValues.sScore = int.Parse(algorithmConfig_sScore.text);
        flow.AlgorithmValues.sTime = int.Parse(algorithmConfig_sTime.text);
        flow.AlgorithmValues.timerCountdown = algorithmConfig_countdown.isOn;

        //user interface
        flow.AlgorithmValues.ui_showHealth = algorithmConfig_showHealth.isOn;
        flow.AlgorithmValues.ui_showScore = algorithmConfig_showScore.isOn;
        flow.AlgorithmValues.ui_showTime = algorithmConfig_showTime.isOn;
        flow.AlgorithmValues.ui_showProgressBar = algorithmConfig_showProgress.isOn;
        flow.AlgorithmValues.ui_showObjective = algorithmConfig_showObjective.isOn;
        flow.AlgorithmValues.ui_textHealth = algorithmConfig_HealthText.text;
        flow.AlgorithmValues.ui_textScore = algorithmConfig_ScoreText.text;
        flow.AlgorithmValues.ui_textObjective = algorithmConfig_ObjectiveText.text;
        flow.AlgorithmValues.ui_textWinMessage = algorithmConfig_WinText.text;
        flow.AlgorithmValues.ui_textLoseMessage = algorithmConfig_LoseText.text;

    }

}

