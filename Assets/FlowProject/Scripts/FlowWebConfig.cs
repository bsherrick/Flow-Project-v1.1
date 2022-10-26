using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlowWebConfig : MonoBehaviour
{
    [HideInInspector] public FlowMain flow;

    [Tooltip("Display debugging messages?")] public bool showDebug;
    [Tooltip("Allow upload?")] public bool noUpload;

    [Header("Web Addresses:")]
    #region
    public string webAddress = "https://game-flow-framework.herokuapp.com/";
    public string webGetQuestions = "getQuestionConfig.php"; //getQuestionConfig.php?questionNo=0 ... getQuestionConfig.php?questionNo=9
    public string webGetGameSettings = "getGameConfig.php"; //getGameConfig.php?gameNo=0 ... getGameConfig.php?gameNo=2
    public string webPostQuestions = "setQuestionConfig.php";
    public string webPostGameSettings = "setGameConfig.php";
    #endregion

    [Header("Settings:")]
    #region
    public int gameSettingsSize = 3;
    public int questionSettingsSize = 10;
    #endregion

    [Header("Status (To be configured by Gamemode/Questions):")]
    #region
    //questions
    public QuestionSettings[] questionSettings;
    public string[] questionsRaw;
    public bool[] questionsWebGetSuccess;
    //game settings
    public GameSettings[] gameSettings;
    public string[] gameSettingsRaw;
    public bool[] gameSettingsWebGetSuccess;
    #endregion

    public void Initialize()
    {
        //questions
        InitQuestions();
        questionsRaw = new string[questionSettingsSize];
        questionsWebGetSuccess = new bool[questionSettingsSize];

        //gamesettings
        InitGamemodes();
        gameSettingsRaw = new string[gameSettingsSize];
        gameSettingsWebGetSuccess = new bool[gameSettingsSize];

        //init question & game settings canvas with empty values
        flow.FlowCanvas.QuestionsConfig_SettingsToUI(0);
        flow.FlowCanvas.GameConfig_SettingsToUI(0);
    }

    public void InitQuestions()
    {
        questionSettings = new QuestionSettings[questionSettingsSize];
        for (int i = 0; i < questionSettingsSize; i++)
        {
            questionSettings[i] = new QuestionSettings();
            questionSettings[i].g_question = "[enter your question here]";
            questionSettings[i].qt_ls1 = "[enter answer here]";
            questionSettings[i].qt_ls2 = "[enter answer here]";
            questionSettings[i].qt_ls3 = "[enter answer here]";
            questionSettings[i].qt_ls4 = "[enter answer here]";
            questionSettings[i].qt_ls5 = "[enter answer here]";
            questionSettings[i].g_repeatAmount = 0;
            questionSettings[i].qt_type = 1;
            questionSettings[i].e_delayStart = 0;
            questionSettings[i].e_delayEnd = 0;
            questionSettings[i].e_lineSpeedSetTo = 10;
            questionSettings[i].t_line = 1;
            questionSettings[i].qt_sDec = 0;
        }
    }

    public void InitGamemodes()
    {
        gameSettings = new GameSettings[gameSettingsSize];
        for (int i = 0; i < gameSettingsSize; i++)
        {
            gameSettings[i] = new GameSettings();

            //init
            gameSettings[i].g_gamePlay = 0;
            gameSettings[i].g_startDelay = 0;
            gameSettings[i].g_endDelay = 0;
            gameSettings[i].me_speed = 15;
            gameSettings[i].me_repeat = 1;
            gameSettings[i].me_patchArrangement = "ABCDE";
            gameSettings[i].me_patchA = "............................................................"; //60 characters
            gameSettings[i].me_patchB = "............................................................"; //60 characters
            gameSettings[i].me_patchC = "............................................................"; //60 characters
            gameSettings[i].me_patchD = "............................................................"; //60 characters
            gameSettings[i].me_patchE = "............................................................"; //60 characters
            gameSettings[i].ui_textHealth = "HEALTH";
            gameSettings[i].ui_textScore = "SCORE";
            gameSettings[i].ui_textObjective = "[enter objective]";
            gameSettings[i].ui_textWinMessage = "[enter win message]";
            gameSettings[i].ui_textLoseMessage = "[enter lose message]";
        }
    }

    public void ResetQuestions()
    {
        for (int i = 0; i < questionSettingsSize; i++)
        {
            ExtractQuestion(i);
        }
    }

    public void GetAllQuestionAndGameConfigs()
    {
        flow.FlowCanvas.loadingScreen.SetActive(true);
        StartCoroutine(DownloadAndExtract(true, true, 0, true, true, 0));
    }

    /// <summary>
    /// Extract Question Configuration from raw web data
    /// </summary>
    /// <param name="i">Question number</param>
    void ExtractQuestion(int i)
    {
        string[] split;

        split = questionsRaw[i].Split('\t');

        if (showDebug) { Debug.Log("---SPLIT RESULT START---"); }
        for (int j = 0; j < split.Length; j++)
        {
            split[j] = split[j].Substring(split[j].IndexOf(']') + 1);
            if (showDebug) { Debug.Log("[" + j + "] - " + split[j]); }
        }
        if (showDebug) { Debug.Log("---SPLIT RESULT END---"); }

        //general
        questionSettings[i].g_question = split[0];
        questionSettings[i].g_active = StringToBool(split[1]);
        questionSettings[i].g_repeatAmount = flow.FlowCanvas.IntCheck(split[2], false, 0, 0);
        questionSettings[i].g_neverExpire = StringToBool(split[3]);
        questionSettings[i].qt_type = flow.FlowCanvas.IntCheck(split[4], false, 0, 0);
        questionSettings[i].e_delayStart = flow.FlowCanvas.FloatCheck(split[5], false, 0, 0);
        questionSettings[i].e_delayEnd = flow.FlowCanvas.FloatCheck(split[6], false, 0, 0);
        questionSettings[i].e_lineSpeedSetTo = flow.FlowCanvas.FloatCheck(split[7], false, 0, 0);
        //trigger & rewards
        questionSettings[i].t_health = flow.FlowCanvas.IntCheck(split[8], false, 0, 0);
        questionSettings[i].t_score = flow.FlowCanvas.IntCheck(split[9], false, 0, 0);
        questionSettings[i].t_line = flow.FlowCanvas.IntCheck(split[10], false, 0, 0);
        questionSettings[i].t_time = flow.FlowCanvas.IntCheck(split[11], false, 0, 0);
        questionSettings[i].t_timeTotal = questionSettings[i].t_time;
        questionSettings[i].t_healthOn = StringToBool(split[12]);
        questionSettings[i].t_scoreOn = StringToBool(split[13]);
        questionSettings[i].t_lineOn = StringToBool(split[14]);
        questionSettings[i].t_timeOn = StringToBool(split[15]);
        questionSettings[i].rp_healthAdd = flow.FlowCanvas.IntCheck(split[16], false, 0, 0);
        questionSettings[i].rp_scoreAdd = flow.FlowCanvas.IntCheck(split[17], false, 0, 0);
        //question type: slot
        questionSettings[i].qt_ls1 = split[18];
        questionSettings[i].qt_ls2 = split[19];
        questionSettings[i].qt_ls3 = split[20];
        questionSettings[i].qt_ls4 = split[21];
        questionSettings[i].qt_ls5 = split[22];
        questionSettings[i].qt_ls1On = StringToBool(split[23]);
        questionSettings[i].qt_ls2On = StringToBool(split[24]);
        questionSettings[i].qt_ls3On = StringToBool(split[25]);
        questionSettings[i].qt_ls4On = StringToBool(split[26]);
        questionSettings[i].qt_ls5On = StringToBool(split[27]);
        //quesiton type: spectrum
        questionSettings[i].qt_sMin = flow.FlowCanvas.FloatCheck(split[28], false, 0, 0);
        questionSettings[i].qt_sMax = flow.FlowCanvas.FloatCheck(split[29], false, 0, 0);
        questionSettings[i].qt_sDec = flow.FlowCanvas.IntCheck(split[30], false, 0, 0);

        flow.FlowCanvas.QuestionsConfig_SettingsToUI(i); //update ui

        if (showDebug)
        {
            Debug.Log("QUESTIONS[" + i + "] DEBUG:");
            questionSettings[i].PrintVariables();
        }
    }

    /// <summary>
    /// Extract Game Configuration from raw web data
    /// </summary>
    /// <param name="i">Gamemode number</param>
    void ExtractGame(int i)
    {
        string[] split;

        split = gameSettingsRaw[i].Split('\t');

        if (showDebug) { Debug.Log("---SPLIT RESULT START---"); }
        for (int j = 0; j < split.Length; j++)
        {
            split[j] = split[j].Substring(split[j].IndexOf(']') + 1);
            if (showDebug) { Debug.Log("[" + j + "] - " + split[j]); }
        }
        if (showDebug) { Debug.Log("---SPLIT RESULT END---"); }

        gameSettings[i].g_gamePlay = flow.FlowCanvas.IntCheck(split[0], false, 0, 0);
        gameSettings[i].g_startDelay = flow.FlowCanvas.FloatCheck(split[1], false, 0, 0);
        gameSettings[i].g_endDelay = flow.FlowCanvas.FloatCheck(split[2], false, 0, 0);

        gameSettings[i].me_patchArrangement = split[3];
        gameSettings[i].me_patchA = split[4];
        gameSettings[i].me_patchB = split[5];
        gameSettings[i].me_patchC = split[6];
        gameSettings[i].me_patchD = split[7];
        gameSettings[i].me_patchE = split[8];
        gameSettings[i].me_speed = flow.FlowCanvas.FloatCheck(split[9], false, 0, 0);
        gameSettings[i].me_repeat = flow.FlowCanvas.IntCheck(split[10], false, 0, 0);
        gameSettings[i].me_endless = StringToBool(split[11]);

        gameSettings[i].gs_wHealth = flow.FlowCanvas.IntCheck(split[12], false, 0, 0);
        gameSettings[i].gs_wScore = flow.FlowCanvas.IntCheck(split[13], false, 0, 0);
        gameSettings[i].gs_wTime = flow.FlowCanvas.IntCheck(split[14], false, 0, 0);
        gameSettings[i].gs_lHealth = flow.FlowCanvas.IntCheck(split[15], false, 0, 0);
        gameSettings[i].gs_lScore = flow.FlowCanvas.IntCheck(split[16], false, 0, 0);
        gameSettings[i].gs_lTime = flow.FlowCanvas.IntCheck(split[17], false, 0, 0);
        gameSettings[i].gs_wHealthOn = StringToBool(split[18]);
        gameSettings[i].gs_wScoreOn = StringToBool(split[19]);
        gameSettings[i].gs_wTimeOn = StringToBool(split[20]);
        gameSettings[i].gs_lHealthOn = StringToBool(split[21]);
        gameSettings[i].gs_lScoreOn = StringToBool(split[22]);
        gameSettings[i].gs_lTimeOn = StringToBool(split[23]);
        gameSettings[i].gs_health = flow.FlowCanvas.IntCheck(split[24], false, 0, 0);
        gameSettings[i].gs_score = flow.FlowCanvas.IntCheck(split[25], false, 0, 0);
        gameSettings[i].gs_time = flow.FlowCanvas.IntCheck(split[26], false, 0, 0);
        gameSettings[i].gs_timeCountdown = StringToBool(split[27]);

        gameSettings[i].ui_showHealth = StringToBool(split[28]);
        gameSettings[i].ui_showScore = StringToBool(split[29]);
        gameSettings[i].ui_showTime = StringToBool(split[30]);
        gameSettings[i].ui_showProgressBar = StringToBool(split[31]);
        gameSettings[i].ui_showObjective = StringToBool(split[32]);
        gameSettings[i].ui_textHealth = split[33];
        gameSettings[i].ui_textScore = split[34];
        gameSettings[i].ui_textObjective = split[35];
        gameSettings[i].ui_textWinMessage = split[36];
        gameSettings[i].ui_textLoseMessage = split[37];

        flow.FlowCanvas.GameConfig_SettingsToUI(i); //update ui

        if (showDebug)
        {
            Debug.Log("GAME SETTINGS[" + i + "] DEBUG:");
            gameSettings[i].PrintVariables();
        }
    }

    /// <summary>
    /// Converts a string to a boolean variable.
    /// </summary>
    /// <param name="s">The string to be converted.</param>
    /// <returns>The boolean variable.</returns>
    bool StringToBool(string s)
    {
        if (s == "True") { return true; } else { return false; }
    }

    #region web requests

    public void GetQuestionConfig(int i)
    {
        flow.FlowCanvas.loadingScreen.SetActive(true);
        StartCoroutine(DownloadAndExtract(true, false, i, false, false, 0));
    }

    public void GetGameConfig(int i)
    {
        flow.FlowCanvas.loadingScreen.SetActive(true);
        StartCoroutine(DownloadAndExtract(false, false, 0, true, false, i));
    }

    public void PostQuestionConfig(int i)
    {
        if (noUpload)
        {
            Debug.Log(flow.FlowCanvas.GenerateQuestionConfig(i));
            return;
        }
        flow.FlowCanvas.loadingScreen.SetActive(true);
        StartCoroutine(UploadAndExtract(true, i, false, 0));
    }

    public void PostGameConfig(int i)
    {
        if (noUpload)
        {
            Debug.Log(flow.FlowCanvas.GenerateGameConfig(i));
            return;
        }
        flow.FlowCanvas.loadingScreen.SetActive(true);
        StartCoroutine(UploadAndExtract(false, 0, true, i));
    }

    /// <summary>
    /// Retrieves data from the web.
    /// </summary>
    /// <param name="questionDownload">TRUE to download Question(s).</param>
    /// <param name="questionAll">TRUE to download all available Questions.</param>
    /// <param name="questionIndex">Question number.</param>
    /// <param name="gameDownload">TRUE to download Gamemode(s).</param>
    /// <param name="gameAll">TRUE to download all available Gamemodes.</param>
    /// <param name="gameIndex">Gamemode number.</param>
    /// <returns></returns>
    IEnumerator DownloadAndExtract(bool questionDownload, bool questionAll, int questionIndex,
                                    bool gameDownload, bool gameAll, int gameIndex)
    {
        UnityWebRequest www;

        if (questionDownload)
        {
            if (questionAll)
            {
                for (int i = 0; i < questionSettings.Length; i++)
                {
                    flow.FlowCanvas.loadingScreen_info.text = "Downloading question number " + (i + 1) + "...";
                    #region download Questions
                    yield return new WaitForSecondsRealtime(0.1f);
                    www = UnityWebRequest.Get(webAddress + webGetQuestions + "?questionNo=" + i);
                    yield return www.SendWebRequest();
                    if (www.isNetworkError || www.isHttpError)
                    {
                        flow.FlowCanvas.loadingScreen_info.text = "ERROR DOWNLOADING. Please report this problem.";
                        yield break;
                        //Debug.Log("WebGet Question[" + i + "] ERROR: " + www.error);
                        //questionsRaw[i] = "NULL";
                        //questionsWebGetSuccess[i] = false;
                    }
                    else
                    {
                        if (showDebug)
                        {
                            Debug.Log("WebGet Question[" + i + "] SUCCESS: " + www.downloadHandler.text);
                        }
                        questionsRaw[i] = www.downloadHandler.text;
                        questionsWebGetSuccess[i] = true;
                    }
                    #endregion
                    ExtractQuestion(i);
                }
            }
            else
            {
                int i = questionIndex;
                flow.FlowCanvas.loadingScreen_info.text = "Downloading question number " + (i + 1) + "...";
                #region download Questions
                yield return new WaitForSecondsRealtime(0.1f);
                www = UnityWebRequest.Get(webAddress + webGetQuestions + "?questionNo=" + i);
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    flow.FlowCanvas.loadingScreen_info.text = "ERROR DOWNLOADING. Please report this problem.";
                    yield break;
                    //Debug.Log("WebGet Question[" + i + "] ERROR: " + www.error);
                    //questionsRaw[i] = "NULL";
                    //questionsWebGetSuccess[i] = false;
                }
                else
                {
                    if (showDebug)
                    {
                        Debug.Log("WebGet Question[" + i + "] SUCCESS: " + www.downloadHandler.text);
                    }
                    questionsRaw[i] = www.downloadHandler.text;
                    questionsWebGetSuccess[i] = true;
                }
                #endregion
                ExtractQuestion(i);
            }
        }

        if (gameDownload)
        {
            if (gameAll)
            {
                for (int i = 0; i < gameSettings.Length; i++)
                {
                    flow.FlowCanvas.loadingScreen_info.text = "Downloading game mode number " + (i + 1) + "...";
                    #region download GameSettings
                    yield return new WaitForSecondsRealtime(0.1f);
                    www = UnityWebRequest.Get(webAddress + webGetGameSettings + "?gameNo=" + i);
                    yield return www.SendWebRequest();
                    if (www.isNetworkError || www.isHttpError)
                    {
                        flow.FlowCanvas.loadingScreen_info.text = "ERROR DOWNLOADING. Please report this problem.";
                        yield break;
                        //Debug.Log("WebGet GameSetting[" + i + "] ERROR: " + www.error);
                        //gameSettingsRaw[i] = "NULL";
                        //gameSettingsWebGetSuccess[i] = false;
                    }
                    else
                    {
                        if (showDebug)
                        {
                            Debug.Log("WebGet GameSetting[" + i + "] SUCCESS: " + www.downloadHandler.text);
                        }
                        gameSettingsRaw[i] = www.downloadHandler.text;
                        gameSettingsWebGetSuccess[i] = true;
                    }
                    #endregion
                    ExtractGame(i);
                }
            }
            else
            {
                int i = gameIndex;
                flow.FlowCanvas.loadingScreen_info.text = "Downloading game mode number " + (i + 1) + "...";
                #region download GameSettings
                yield return new WaitForSecondsRealtime(0.1f);
                www = UnityWebRequest.Get(webAddress + webGetGameSettings + "?gameNo=" + i);
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    flow.FlowCanvas.loadingScreen_info.text = "ERROR DOWNLOADING. Please report this problem.";
                    yield break;
                    //Debug.Log("WebGet GameSetting[" + i + "] ERROR: " + www.error);
                    //gameSettingsRaw[i] = "NULL";
                    //gameSettingsWebGetSuccess[i] = false;
                }
                else
                {
                    if (showDebug)
                    {
                        Debug.Log("WebGet GameSetting[" + i + "] SUCCESS: " + www.downloadHandler.text);
                    }
                    gameSettingsRaw[i] = www.downloadHandler.text;
                    gameSettingsWebGetSuccess[i] = true;
                }
                #endregion
                ExtractGame(i);
            }
        }
        flow.FlowCanvas.loadingScreen.SetActive(false);
    }

    /// <summary>
    /// Uploads data onto the web.
    /// </summary>
    /// <param name="questionUpload">TRUE to upload Question.</param>
    /// <param name="questionIndex">Question number.</param>
    /// <param name="gameUpload">TRUE to upload Gamemode.</param>
    /// <param name="gameIndex">Gamemode number.</param>
    /// <returns></returns>
    IEnumerator UploadAndExtract(bool questionUpload, int questionIndex, bool gameUpload, int gameIndex)
    {
        UnityWebRequest www;
        WWWForm form;
        int i;

        if (questionUpload)
        {
            flow.FlowCanvas.QuestionsConfig_UIToSettings(questionIndex);
            flow.FlowCanvas.loadingScreen_info.text = "Uploading question number " + (questionIndex + 1) + "...";

            form = new WWWForm();
            i = 0;
            #region form
            form.AddField("questionNo", questionIndex);
            //general
            form.AddField("qc0", "[" + i++ + "]" + questionSettings[questionIndex].g_question);
            form.AddField("qc1", "[" + i++ + "]" + questionSettings[questionIndex].g_active);
            form.AddField("qc2", "[" + i++ + "]" + questionSettings[questionIndex].g_repeatAmount);
            form.AddField("qc3", "[" + i++ + "]" + questionSettings[questionIndex].g_neverExpire);
            form.AddField("qc4", "[" + i++ + "]" + questionSettings[questionIndex].qt_type);
            form.AddField("qc5", "[" + i++ + "]" + questionSettings[questionIndex].e_delayStart);
            form.AddField("qc6", "[" + i++ + "]" + questionSettings[questionIndex].e_delayEnd);
            form.AddField("qc7", "[" + i++ + "]" + questionSettings[questionIndex].e_lineSpeedSetTo);
            //trigger & rewards
            form.AddField("qc8", "[" + i++ + "]" + questionSettings[questionIndex].t_health);
            form.AddField("qc9", "[" + i++ + "]" + questionSettings[questionIndex].t_score);
            form.AddField("qc10", "[" + i++ + "]" + questionSettings[questionIndex].t_line);
            form.AddField("qc11", "[" + i++ + "]" + questionSettings[questionIndex].t_time);
            form.AddField("qc12", "[" + i++ + "]" + questionSettings[questionIndex].t_healthOn);
            form.AddField("qc13", "[" + i++ + "]" + questionSettings[questionIndex].t_scoreOn);
            form.AddField("qc14", "[" + i++ + "]" + questionSettings[questionIndex].t_lineOn);
            form.AddField("qc15", "[" + i++ + "]" + questionSettings[questionIndex].t_timeOn);
            form.AddField("qc16", "[" + i++ + "]" + questionSettings[questionIndex].rp_healthAdd);
            form.AddField("qc17", "[" + i++ + "]" + questionSettings[questionIndex].rp_scoreAdd);
            //question type: slot
            form.AddField("qc18", "[" + i++ + "]" + questionSettings[questionIndex].qt_ls1);
            form.AddField("qc19", "[" + i++ + "]" + questionSettings[questionIndex].qt_ls2);
            form.AddField("qc20", "[" + i++ + "]" + questionSettings[questionIndex].qt_ls3);
            form.AddField("qc21", "[" + i++ + "]" + questionSettings[questionIndex].qt_ls4);
            form.AddField("qc22", "[" + i++ + "]" + questionSettings[questionIndex].qt_ls5);
            form.AddField("qc23", "[" + i++ + "]" + questionSettings[questionIndex].qt_ls1On);
            form.AddField("qc24", "[" + i++ + "]" + questionSettings[questionIndex].qt_ls2On);
            form.AddField("qc25", "[" + i++ + "]" + questionSettings[questionIndex].qt_ls3On);
            form.AddField("qc26", "[" + i++ + "]" + questionSettings[questionIndex].qt_ls4On);
            form.AddField("qc27", "[" + i++ + "]" + questionSettings[questionIndex].qt_ls5On);
            //quesiton type: spectrum
            form.AddField("qc28", "[" + i++ + "]" + questionSettings[questionIndex].qt_sMin);
            form.AddField("qc29", "[" + i++ + "]" + questionSettings[questionIndex].qt_sMax);
            form.AddField("qc30", "[" + i++ + "]" + questionSettings[questionIndex].qt_sDec);
            #endregion

            if (showDebug)
            {
                i = 0;
                #region debugForm
                Debug.Log("questionNo: " + questionIndex);
                //general
                Debug.Log("qc0: [" + i++ + "]" + questionSettings[questionIndex].g_question);
                Debug.Log("qc1: [" + i++ + "]" + questionSettings[questionIndex].g_active);
                Debug.Log("qc2: [" + i++ + "]" + questionSettings[questionIndex].g_repeatAmount);
                Debug.Log("qc3: [" + i++ + "]" + questionSettings[questionIndex].g_neverExpire);
                Debug.Log("qc4: [" + i++ + "]" + questionSettings[questionIndex].qt_type);
                Debug.Log("qc5: [" + i++ + "]" + questionSettings[questionIndex].e_delayStart);
                Debug.Log("qc6: [" + i++ + "]" + questionSettings[questionIndex].e_delayEnd);
                Debug.Log("qc7: [" + i++ + "]" + questionSettings[questionIndex].e_lineSpeedSetTo);
                //trigger & rewards
                Debug.Log("qc8: [" + i++ + "]" + questionSettings[questionIndex].t_health);
                Debug.Log("qc9: [" + i++ + "]" + questionSettings[questionIndex].t_score);
                Debug.Log("qc10: [" + i++ + "]" + questionSettings[questionIndex].t_line);
                Debug.Log("qc11: [" + i++ + "]" + questionSettings[questionIndex].t_time);
                Debug.Log("qc12: [" + i++ + "]" + questionSettings[questionIndex].t_healthOn);
                Debug.Log("qc13: [" + i++ + "]" + questionSettings[questionIndex].t_scoreOn);
                Debug.Log("qc14: [" + i++ + "]" + questionSettings[questionIndex].t_lineOn);
                Debug.Log("qc15: [" + i++ + "]" + questionSettings[questionIndex].t_timeOn);
                Debug.Log("qc16: [" + i++ + "]" + questionSettings[questionIndex].rp_healthAdd);
                Debug.Log("qc17: [" + i++ + "]" + questionSettings[questionIndex].rp_scoreAdd);
                //question type: slot
                Debug.Log("qc18: [" + i++ + "]" + questionSettings[questionIndex].qt_ls1);
                Debug.Log("qc19: [" + i++ + "]" + questionSettings[questionIndex].qt_ls2);
                Debug.Log("qc20: [" + i++ + "]" + questionSettings[questionIndex].qt_ls3);
                Debug.Log("qc21: [" + i++ + "]" + questionSettings[questionIndex].qt_ls4);
                Debug.Log("qc22: [" + i++ + "]" + questionSettings[questionIndex].qt_ls5);
                Debug.Log("qc23: [" + i++ + "]" + questionSettings[questionIndex].qt_ls1On);
                Debug.Log("qc24: [" + i++ + "]" + questionSettings[questionIndex].qt_ls2On);
                Debug.Log("qc25: [" + i++ + "]" + questionSettings[questionIndex].qt_ls3On);
                Debug.Log("qc26: [" + i++ + "]" + questionSettings[questionIndex].qt_ls4On);
                Debug.Log("qc27: [" + i++ + "]" + questionSettings[questionIndex].qt_ls5On);
                //quesiton type: spectrum
                Debug.Log("qc28: [" + i++ + "]" + questionSettings[questionIndex].qt_sMin);
                Debug.Log("qc29: [" + i++ + "]" + questionSettings[questionIndex].qt_sMax);
                Debug.Log("qc30: [" + i++ + "]" + questionSettings[questionIndex].qt_sDec);
                #endregion
            }

            www = UnityWebRequest.Post(webAddress + webPostQuestions, form);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                flow.FlowCanvas.loadingScreen_info.text = "ERROR DOWNLOADING. Please report this problem.";
                yield break;
                //Debug.Log("WEB POST ERROR: " + www.error);
            }
            else
            {
                //Debug.Log("WEB POST SUCCESS: " + www.downloadHandler.text);
                questionsRaw[questionIndex] = flow.FlowCanvas.GenerateQuestionConfig(questionIndex);
                ExtractQuestion(questionIndex);
            }
        }

        if (gameUpload)
        {
            flow.FlowCanvas.GameConfig_UIToSettings(gameIndex);     
            flow.FlowCanvas.loadingScreen_info.text = "Uploading game mode number " + (gameIndex + 1) + "...";

            form = new WWWForm();
            i = 0;
            #region form
            form.AddField("gameNo", gameIndex);
            //general
            form.AddField("gc0", "[" + i++ + "]" + gameSettings[gameIndex].g_gamePlay);
            form.AddField("gc1", "[" + i++ + "]" + gameSettings[gameIndex].g_startDelay);
            form.AddField("gc2", "[" + i++ + "]" + gameSettings[gameIndex].g_endDelay);
            //form.AddField("gc3", "[" + i++ + "]" + flow.FlowCanvas.MapToSingle(gameSettings[gameIndex].me_map));
            //form.AddField("gc3", "[" + i++ + "]" + gameSettings[gameIndex].me_map);
            form.AddField("gc3", "[" + i++ + "]" + gameSettings[gameIndex].me_patchArrangement);
            form.AddField("gc4", "[" + i++ + "]" + gameSettings[gameIndex].me_patchA);
            form.AddField("gc5", "[" + i++ + "]" + gameSettings[gameIndex].me_patchB);
            form.AddField("gc6", "[" + i++ + "]" + gameSettings[gameIndex].me_patchC);
            form.AddField("gc7", "[" + i++ + "]" + gameSettings[gameIndex].me_patchD);
            form.AddField("gc8", "[" + i++ + "]" + gameSettings[gameIndex].me_patchE);
            form.AddField("gc9", "[" + i++ + "]" + gameSettings[gameIndex].me_speed);
            form.AddField("gc10", "[" + i++ + "]" + gameSettings[gameIndex].me_repeat);
            form.AddField("gc11", "[" + i++ + "]" + gameSettings[gameIndex].me_endless);
            //gameplay
            form.AddField("gc12", "[" + i++ + "]" + gameSettings[gameIndex].gs_wHealth);
            form.AddField("gc13", "[" + i++ + "]" + gameSettings[gameIndex].gs_wScore);
            form.AddField("gc14", "[" + i++ + "]" + gameSettings[gameIndex].gs_wTime);
            form.AddField("gc15", "[" + i++ + "]" + gameSettings[gameIndex].gs_lHealth);
            form.AddField("gc16", "[" + i++ + "]" + gameSettings[gameIndex].gs_lScore);
            form.AddField("gc17", "[" + i++ + "]" + gameSettings[gameIndex].gs_lTime);
            form.AddField("gc18", "[" + i++ + "]" + gameSettings[gameIndex].gs_wHealthOn);
            form.AddField("gc19", "[" + i++ + "]" + gameSettings[gameIndex].gs_wScoreOn);
            form.AddField("gc20", "[" + i++ + "]" + gameSettings[gameIndex].gs_wTimeOn);
            form.AddField("gc21", "[" + i++ + "]" + gameSettings[gameIndex].gs_lHealthOn);
            form.AddField("gc22", "[" + i++ + "]" + gameSettings[gameIndex].gs_lScoreOn);
            form.AddField("gc23", "[" + i++ + "]" + gameSettings[gameIndex].gs_lTimeOn);
            form.AddField("gc24", "[" + i++ + "]" + gameSettings[gameIndex].gs_health);
            form.AddField("gc25", "[" + i++ + "]" + gameSettings[gameIndex].gs_score);
            form.AddField("gc26", "[" + i++ + "]" + gameSettings[gameIndex].gs_time);
            form.AddField("gc27", "[" + i++ + "]" + gameSettings[gameIndex].gs_timeCountdown);
            //user interface
            form.AddField("gc28", "[" + i++ + "]" + gameSettings[gameIndex].ui_showHealth);
            form.AddField("gc29", "[" + i++ + "]" + gameSettings[gameIndex].ui_showScore);
            form.AddField("gc30", "[" + i++ + "]" + gameSettings[gameIndex].ui_showTime);
            form.AddField("gc31", "[" + i++ + "]" + gameSettings[gameIndex].ui_showProgressBar);
            form.AddField("gc32", "[" + i++ + "]" + gameSettings[gameIndex].ui_showObjective);
            form.AddField("gc33", "[" + i++ + "]" + gameSettings[gameIndex].ui_textHealth);
            form.AddField("gc34", "[" + i++ + "]" + gameSettings[gameIndex].ui_textScore);
            form.AddField("gc35", "[" + i++ + "]" + gameSettings[gameIndex].ui_textObjective);
            form.AddField("gc36", "[" + i++ + "]" + gameSettings[gameIndex].ui_textWinMessage);
            form.AddField("gc37", "[" + i++ + "]" + gameSettings[gameIndex].ui_textLoseMessage);
            #endregion

            if (showDebug)
            {
                i = 0;
                #region debugForm
                Debug.Log("gameNo: " + gameIndex);
                //general
                Debug.Log("gc0: [" + i++ + "]" + gameSettings[gameIndex].g_gamePlay);
                Debug.Log("gc1: [" + i++ + "]" + gameSettings[gameIndex].g_startDelay);
                Debug.Log("gc2: [" + i++ + "]" + gameSettings[gameIndex].g_endDelay);
                //Debug.Log("gc3: [" + i++ + "]" + flow.FlowCanvas.MapToSingle(gameSettings[gameIndex].me_map));
                //Debug.Log("gc3: [" + i++ + "]" + gameSettings[gameIndex].me_map);
                Debug.Log("gc3: [" + i++ + "]" + gameSettings[gameIndex].me_patchArrangement);
                Debug.Log("gc4: [" + i++ + "]" + gameSettings[gameIndex].me_patchA);
                Debug.Log("gc5: [" + i++ + "]" + gameSettings[gameIndex].me_patchB);
                Debug.Log("gc6: [" + i++ + "]" + gameSettings[gameIndex].me_patchC);
                Debug.Log("gc7: [" + i++ + "]" + gameSettings[gameIndex].me_patchD);
                Debug.Log("gc8: [" + i++ + "]" + gameSettings[gameIndex].me_patchE);
                Debug.Log("gc9: [" + i++ + "]" + gameSettings[gameIndex].me_speed);
                Debug.Log("gc10: [" + i++ + "]" + gameSettings[gameIndex].me_repeat);
                Debug.Log("gc11: [" + i++ + "]" + gameSettings[gameIndex].me_endless);
                //gameplay
                Debug.Log("gc12: [" + i++ + "]" + gameSettings[gameIndex].gs_wHealth);
                Debug.Log("gc13: [" + i++ + "]" + gameSettings[gameIndex].gs_wScore);
                Debug.Log("gc14: [" + i++ + "]" + gameSettings[gameIndex].gs_wTime);
                Debug.Log("gc15: [" + i++ + "]" + gameSettings[gameIndex].gs_lHealth);
                Debug.Log("gc16: [" + i++ + "]" + gameSettings[gameIndex].gs_lScore);
                Debug.Log("gc17: [" + i++ + "]" + gameSettings[gameIndex].gs_lTime);
                Debug.Log("gc18: [" + i++ + "]" + gameSettings[gameIndex].gs_wHealthOn);
                Debug.Log("gc19: [" + i++ + "]" + gameSettings[gameIndex].gs_wScoreOn);
                Debug.Log("gc20: [" + i++ + "]" + gameSettings[gameIndex].gs_wTimeOn);
                Debug.Log("gc21: [" + i++ + "]" + gameSettings[gameIndex].gs_lHealthOn);
                Debug.Log("gc22: [" + i++ + "]" + gameSettings[gameIndex].gs_lScoreOn);
                Debug.Log("gc23: [" + i++ + "]" + gameSettings[gameIndex].gs_lTimeOn);
                Debug.Log("gc24: [" + i++ + "]" + gameSettings[gameIndex].gs_health);
                Debug.Log("gc25: [" + i++ + "]" + gameSettings[gameIndex].gs_score);
                Debug.Log("gc26: [" + i++ + "]" + gameSettings[gameIndex].gs_time);
                Debug.Log("gc27: [" + i++ + "]" + gameSettings[gameIndex].gs_timeCountdown);
                //user interface
                Debug.Log("gc28: [" + i++ + "]" + gameSettings[gameIndex].ui_showHealth);
                Debug.Log("gc29: [" + i++ + "]" + gameSettings[gameIndex].ui_showScore);
                Debug.Log("gc30: [" + i++ + "]" + gameSettings[gameIndex].ui_showTime);
                Debug.Log("gc31: [" + i++ + "]" + gameSettings[gameIndex].ui_showProgressBar);
                Debug.Log("gc32: [" + i++ + "]" + gameSettings[gameIndex].ui_showObjective);
                Debug.Log("gc33: [" + i++ + "]" + gameSettings[gameIndex].ui_textHealth);
                Debug.Log("gc34: [" + i++ + "]" + gameSettings[gameIndex].ui_textScore);
                Debug.Log("gc35: [" + i++ + "]" + gameSettings[gameIndex].ui_textObjective);
                Debug.Log("gc36: [" + i++ + "]" + gameSettings[gameIndex].ui_textWinMessage);
                Debug.Log("gc37: [" + i++ + "]" + gameSettings[gameIndex].ui_textLoseMessage);
                #endregion
            }

            www = UnityWebRequest.Post(webAddress + webPostGameSettings, form);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                flow.FlowCanvas.loadingScreen_info.text = "ERROR DOWNLOADING. Please report this problem.";
                yield break;
                //Debug.Log("WEB POST ERROR: " + www.error);
            }
            else
            {
                //Debug.Log("WEB POST SUCCESS: " + www.downloadHandler.text);
                gameSettingsRaw[gameIndex] = flow.FlowCanvas.GenerateGameConfig(gameIndex);
                ExtractGame(gameIndex);
            }
        }

        flow.FlowCanvas.loadingScreen.SetActive(false);
    }

    #endregion
}

public class GameSettings
{
    //General
    public int g_gamePlay; //1,2,(3)
    public float g_startDelay; //0 =< x
    public float g_endDelay;//0 =< x    

    //Map Editor
    //public string me_map; //only allow lines of "xxxxx" where x = {0,1,2}
    public string me_patchArrangement;
    public string me_patchA;
    public string me_patchB;
    public string me_patchC;
    public string me_patchD;
    public string me_patchE;
    public float me_speed; //0 < x <= 100
    public int me_repeat; //0 < x
    public bool me_endless;

    //Game Settings
    //--win conditions
    public int gs_wHealth;
    public int gs_wScore;
    public float gs_wTime;
    public bool gs_wHealthOn;
    public bool gs_wScoreOn;
    public bool gs_wTimeOn;
    //--lose conditions
    public int gs_lHealth;
    public int gs_lScore;
    public float gs_lTime;
    public bool gs_lHealthOn;
    public bool gs_lScoreOn;
    public bool gs_lTimeOn;
    //--starting conditions
    public int gs_health;
    public int gs_score;
    public float gs_time;   
    public bool gs_timeCountdown;

    //UI
    public bool ui_showHealth;
    public bool ui_showScore;
    public bool ui_showTime;
    public bool ui_showProgressBar;
    public bool ui_showObjective;
    public string ui_textHealth;
    public string ui_textScore;
    public string ui_textObjective;
    public string ui_textWinMessage;
    public string ui_textLoseMessage;

    public void PrintVariables()
    {
        Debug.Log("|***************|START GAME SETTINGS|***************|");
        Debug.Log("1:" + g_gamePlay);
        Debug.Log("2:" + g_startDelay);
        Debug.Log("3:" + g_endDelay);
        //Debug.Log("4:" + me_map);
        Debug.Log("4:" + me_patchArrangement);
        Debug.Log("5:" + me_patchA);
        Debug.Log("6:" + me_patchB);
        Debug.Log("7:" + me_patchC);
        Debug.Log("8:" + me_patchD);
        Debug.Log("9:" + me_patchE);
        Debug.Log("10:" + me_speed);
        Debug.Log("11:" + me_repeat);
        Debug.Log("12:" + me_endless);
        Debug.Log("13:" + gs_wHealth);
        Debug.Log("14:" + gs_wScore);
        Debug.Log("15:" + gs_wTime);
        Debug.Log("16:" + gs_wHealthOn);
        Debug.Log("17:" + gs_wScoreOn);
        Debug.Log("18:" + gs_wTimeOn);
        Debug.Log("19:" + gs_lHealth);
        Debug.Log("20:" + gs_lScore);
        Debug.Log("21:" + gs_lTime);
        Debug.Log("22:" + gs_lHealthOn);
        Debug.Log("23:" + gs_lScoreOn);
        Debug.Log("24:" + gs_lTimeOn);
        Debug.Log("25:" + gs_health);
        Debug.Log("26:" + gs_score);
        Debug.Log("27:" + gs_time);
        Debug.Log("28:" + gs_timeCountdown);
        Debug.Log("29:" + ui_showHealth);
        Debug.Log("30:" + ui_showScore);
        Debug.Log("31:" + ui_showTime);
        Debug.Log("32:" + ui_showProgressBar);
        Debug.Log("32:" + ui_showObjective);
        Debug.Log("33:" + ui_textHealth);
        Debug.Log("34:" + ui_textScore);
        Debug.Log("35:" + ui_textObjective);
        Debug.Log("36:" + ui_textWinMessage);
        Debug.Log("37:" + ui_textLoseMessage);
        Debug.Log("|**************|END GAME SETTINGS|***************|");
    }
}

public class QuestionSettings
{
    //General
    public string g_question;
    public bool g_active;
    public int g_repeatAmount; //0 <= x
    public bool g_neverExpire;

    //Trigger
    public int t_health;
    public int t_score;
    public int t_line; //0 < x
    public float t_time;
    public float t_timeTotal;
    public bool t_healthOn;
    public bool t_scoreOn;
    public bool t_lineOn;
    public bool t_timeOn;

    //Effects
    public float e_delayStart; //0 <= x
    public float e_delayEnd; //0 <= x
    public float e_lineSpeedSetTo; // 0 < x <= 100

    //Rewards/Punishment
    public int rp_healthAdd;
    public int rp_scoreAdd;

    //Question Type
    public int qt_type;  //1,2
    //--line slot (1)
    public string qt_ls1;
    public string qt_ls2;
    public string qt_ls3;
    public string qt_ls4;
    public string qt_ls5;
    public bool qt_ls1On;
    public bool qt_ls2On;
    public bool qt_ls3On;
    public bool qt_ls4On;
    public bool qt_ls5On;
    //--spectrum (2)
    public float qt_sMin;
    public float qt_sMax;
    public int qt_sDec; //0 <= x <= 10

    public void PrintVariables()
    {
        Debug.Log("|***************|START QUESTION|***************|");
        Debug.Log("1:" + g_question);
        Debug.Log("2:" + g_active);
        Debug.Log("3:" + g_repeatAmount);
        Debug.Log("4:" + g_neverExpire);
        Debug.Log("5:" + t_health);
        Debug.Log("6:" + t_score);
        Debug.Log("7:" + t_line);
        Debug.Log("8:" + t_time);
        Debug.Log("9:" + t_timeTotal);
        Debug.Log("10:" + t_healthOn);
        Debug.Log("11:" + t_scoreOn);
        Debug.Log("12:" + t_lineOn);
        Debug.Log("13:" + t_timeOn);
        Debug.Log("14:" + e_delayStart);
        Debug.Log("15:" + e_delayEnd);
        Debug.Log("16:" + e_lineSpeedSetTo);
        Debug.Log("17:" + rp_healthAdd);
        Debug.Log("18:" + rp_scoreAdd);
        Debug.Log("19:" + qt_type);
        Debug.Log("20:" + qt_ls1);
        Debug.Log("21:" + qt_ls2);
        Debug.Log("22:" + qt_ls3);
        Debug.Log("23:" + qt_ls4);
        Debug.Log("24:" + qt_ls5);
        Debug.Log("25:" + qt_ls1On);
        Debug.Log("26:" + qt_ls2On);
        Debug.Log("27:" + qt_ls3On);
        Debug.Log("28:" + qt_ls4On);
        Debug.Log("29:" + qt_ls5On);
        Debug.Log("30:" + qt_sMin);
        Debug.Log("31:" + qt_sMax);
        Debug.Log("32:" + qt_sDec);
        Debug.Log("|***************|END QUESTION|***************|");
    }
}
