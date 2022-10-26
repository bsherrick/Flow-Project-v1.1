using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowMain : MonoBehaviour
{
    [Header("DEBUG:")]
    [Tooltip("TRUE if the game should start right away.")] public bool startGameRightAway;
    [Tooltip("TRUE if the game should not download gamemodes/questions from the web.")] public bool noWebDownload;

    [Header("Global Settings:")]
    public bool gameOn = false;

    [Header("Main Scripts:")]
    public FlowCanvas FlowCanvas;
    public FlowWebConfig FlowWebConfig;
    public FlowGameConfig FlowGameConfig;
    public FlowLineGenerator FlowLineGenerator;
    public FlowPlayerMovement FlowPlayerMovement;
    public FlowQuestionHandler FlowQuestionHandler;
    public AlgorithmValues AlgorithmValues;
    
    [Header("Other Scripts:")]
    public Target[] Targets;

    [Header("Audio References:")]
    public AudioSource audioRhythm1;
    public AudioSource audioSoundtrack1;
    public AudioSource audioSoundtrack2;
    public AudioSource audioExplode;
    public AudioSource audioVictory;
    public AudioSource audioClick;
    public AudioSource audioLaserShoot;

    [Header("Other References")]
    public Renderer starWarp;
    //[TextArea] public string finalData = "";

    void Start()
    {
        //get reference
        FlowCanvas = GetComponent<FlowCanvas>();
        FlowWebConfig = GetComponent<FlowWebConfig>();
        FlowGameConfig = GetComponent<FlowGameConfig>();
        FlowLineGenerator = GetComponent<FlowLineGenerator>();
        FlowPlayerMovement = GetComponent<FlowPlayerMovement>();
        FlowQuestionHandler = GetComponent<FlowQuestionHandler>();
        AlgorithmValues = GetComponent<AlgorithmValues>();

        //send reference
        FlowCanvas.flow = this;
        FlowWebConfig.flow = this;
        FlowGameConfig.flow = this;
        FlowLineGenerator.flow = this;
        FlowPlayerMovement.flow = this;
        FlowQuestionHandler.flow = this;

        //send reference to the "targets"
        for (int i = 0; i < Targets.Length; i++)
        {
            Targets[i].flow = this;
        }

        //prepare to handle information from the web
        FlowWebConfig.Initialize();

        //download question and game configurations from the web
        if (!noWebDownload){FlowWebConfig.GetAllQuestionAndGameConfigs();}

        //set the background starWarp speed
        FlowLineGenerator.ChangeLineSpeed(10f);

        //start the game right away (used for debugging)
        if (startGameRightAway){StartGame(0);}
    }

    /// <summary>
    /// Apply game configuration retrieved from the web to current game
    /// </summary>
    /// <param name="i">the game mode configuration to be applied</param>
    public void ApplyGameSettings(int i)
    {
        //GAMEMODE
        FlowGameConfig.gameMode = i;
        //general
        FlowGameConfig.gamePlay = FlowWebConfig.gameSettings[i].g_gamePlay;
        FlowGameConfig.delayStart = FlowWebConfig.gameSettings[i].g_startDelay;
        FlowGameConfig.delayEnd = FlowWebConfig.gameSettings[i].g_endDelay;
        //map editor
        FlowLineGenerator.me_patchArrangement = FlowWebConfig.gameSettings[i].me_patchArrangement;
        FlowLineGenerator.me_patchA = FlowWebConfig.gameSettings[i].me_patchA;
        FlowLineGenerator.me_patchB = FlowWebConfig.gameSettings[i].me_patchB;
        FlowLineGenerator.me_patchC = FlowWebConfig.gameSettings[i].me_patchC;
        FlowLineGenerator.me_patchD = FlowWebConfig.gameSettings[i].me_patchD;
        FlowLineGenerator.me_patchE = FlowWebConfig.gameSettings[i].me_patchE;
        FlowLineGenerator.me_speed = FlowWebConfig.gameSettings[i].me_speed;
        FlowLineGenerator.me_repeat = FlowWebConfig.gameSettings[i].me_repeat;
        FlowLineGenerator.me_endless = FlowWebConfig.gameSettings[i].me_endless;
        //gameplay
        FlowGameConfig.w_health = FlowWebConfig.gameSettings[i].gs_wHealth;
        FlowGameConfig.w_score = FlowWebConfig.gameSettings[i].gs_wScore;
        FlowGameConfig.w_time = FlowWebConfig.gameSettings[i].gs_wTime;
        FlowGameConfig.l_health = FlowWebConfig.gameSettings[i].gs_lHealth;
        FlowGameConfig.l_score = FlowWebConfig.gameSettings[i].gs_lScore;
        FlowGameConfig.l_time = FlowWebConfig.gameSettings[i].gs_lTime;
        FlowGameConfig.w_healthOn = FlowWebConfig.gameSettings[i].gs_wHealthOn;
        FlowGameConfig.w_scoreOn = FlowWebConfig.gameSettings[i].gs_wScoreOn;
        FlowGameConfig.w_timeOn = FlowWebConfig.gameSettings[i].gs_wTimeOn;
        FlowGameConfig.l_healthOn = FlowWebConfig.gameSettings[i].gs_lHealthOn;
        FlowGameConfig.l_scoreOn = FlowWebConfig.gameSettings[i].gs_lScoreOn;
        FlowGameConfig.l_timeOn = FlowWebConfig.gameSettings[i].gs_lTimeOn;
        FlowGameConfig.sHealth = FlowWebConfig.gameSettings[i].gs_health;
        FlowGameConfig.sScore = FlowWebConfig.gameSettings[i].gs_score;
        FlowGameConfig.sTime = FlowWebConfig.gameSettings[i].gs_time;
        FlowGameConfig.timerCountdown= FlowWebConfig.gameSettings[i].gs_timeCountdown;
        //user interface
        FlowCanvas.ui_showHealth = FlowWebConfig.gameSettings[i].ui_showHealth;
        FlowCanvas.ui_showScore = FlowWebConfig.gameSettings[i].ui_showScore;
        FlowCanvas.ui_showTime = FlowWebConfig.gameSettings[i].ui_showTime;
        FlowCanvas.ui_showProgressBar = FlowWebConfig.gameSettings[i].ui_showProgressBar;
        FlowCanvas.ui_showObjective = FlowWebConfig.gameSettings[i].ui_showObjective;
        FlowCanvas.ui_textHealth = FlowWebConfig.gameSettings[i].ui_textHealth;
        FlowCanvas.ui_textScore = FlowWebConfig.gameSettings[i].ui_textScore;
        FlowCanvas.ui_textObjective = FlowWebConfig.gameSettings[i].ui_textObjective;
        FlowCanvas.ui_textWinMessage = FlowWebConfig.gameSettings[i].ui_textWinMessage;
        FlowCanvas.ui_textLoseMessage = FlowWebConfig.gameSettings[i].ui_textLoseMessage;
    }

    /// <summary>
    /// Apply game configuration for the Algorithm mode
    /// </summary>
    /// <param name="i">the game mode configuration to be applied</param>
    public void ApplyAlgorithmSettings()
    {

        //GAMEMODE
        FlowGameConfig.gameMode = -1;
        //general
        FlowGameConfig.gamePlay = AlgorithmValues.gamePlay;
        FlowGameConfig.delayStart = AlgorithmValues.delayStart;
        FlowGameConfig.delayEnd = AlgorithmValues.delayEnd;
        //algorithm editor
        FlowLineGenerator.setSeed = AlgorithmValues.setSeed;
        FlowLineGenerator.algorithmSeed = AlgorithmValues.algorithmSeed;
        FlowLineGenerator.me_speed = AlgorithmValues.algorithmSpeed;
        FlowLineGenerator.speedChangeLines = AlgorithmValues.speedChangeLines;
        FlowLineGenerator.maxLineCooldown = AlgorithmValues.maxLineCooldown;
        FlowLineGenerator.difficultyChangeLines = AlgorithmValues.difficultyChangeLines;
        FlowLineGenerator.maxBetweenStars = AlgorithmValues.maxBetweenStars;
        FlowLineGenerator.minBetweenStars = AlgorithmValues.minBetweenStars;
        FlowLineGenerator.isAdaptiveDifficulty = AlgorithmValues.isAdaptiveDifficulty;
        FlowLineGenerator.adaptiveRocks = AlgorithmValues.adaptiveRocks;
        FlowLineGenerator.adaptiveStars = AlgorithmValues.adaptiveStars;
        //gameplay
        FlowGameConfig.w_health = AlgorithmValues.w_health;
        FlowGameConfig.w_score = AlgorithmValues.w_score;
        FlowGameConfig.w_time = AlgorithmValues.w_time;
        FlowGameConfig.l_health = AlgorithmValues.l_health;
        FlowGameConfig.l_score = AlgorithmValues.l_score;
        FlowGameConfig.l_time = AlgorithmValues.l_time;
        FlowGameConfig.w_healthOn = AlgorithmValues.w_healthOn;
        FlowGameConfig.w_scoreOn = AlgorithmValues.w_scoreOn;
        FlowGameConfig.w_timeOn = AlgorithmValues.w_timeOn;
        FlowGameConfig.l_healthOn = AlgorithmValues.l_healthOn;
        FlowGameConfig.l_scoreOn = AlgorithmValues.l_scoreOn;
        FlowGameConfig.l_timeOn = AlgorithmValues.l_timeOn;
        FlowGameConfig.sHealth = AlgorithmValues.sHealth;
        FlowGameConfig.sScore = AlgorithmValues.sScore;
        FlowGameConfig.sTime = AlgorithmValues.sTime;
        FlowGameConfig.timerCountdown = AlgorithmValues.timerCountdown;

        //user interface
        FlowCanvas.ui_showHealth = AlgorithmValues.ui_showHealth;
        FlowCanvas.ui_showScore = AlgorithmValues.ui_showScore;
        FlowCanvas.ui_showTime = AlgorithmValues.ui_showTime;
        FlowCanvas.ui_showProgressBar = AlgorithmValues.ui_showProgressBar;
        FlowCanvas.ui_showObjective = AlgorithmValues.ui_showObjective;
        FlowCanvas.ui_textHealth = AlgorithmValues.ui_textHealth;
        FlowCanvas.ui_textScore = AlgorithmValues.ui_textScore;
        FlowCanvas.ui_textObjective = AlgorithmValues.ui_textObjective;
        FlowCanvas.ui_textWinMessage = AlgorithmValues.ui_textWinMessage;
        FlowCanvas.ui_textLoseMessage = AlgorithmValues.ui_textLoseMessage;
    }


    /// <summary>
    /// Start the game
    /// </summary>
    /// <param name="mode">Gamemode to be played, set to 3 for Bonus Rhythm mode</param>
    public void StartGame(int mode)
    {
        if (mode == 3) //BONUS: Rhythm mode. Manually set configuration.
        {
            FlowGameConfig.gamePlay = FlowGameConfig.gamePlay_Rhythm;
            FlowLineGenerator.me_speed = 20;
            FlowCanvas.ui_textWinMessage = "Thank you for playing!";
            FlowGameConfig.delayEnd = 5;
        }
        else if (mode == -1) //Algorithm Mode
        {
            ApplyAlgorithmSettings();
        }
        else //all the other gamemodes
        {
            ApplyGameSettings(mode);
        }

        //Scripts should be initialized in this order!
        FlowCanvas.Initialize();
        FlowQuestionHandler.Initialize();
        FlowLineGenerator.Initialize();
        FlowGameConfig.Initialize();
        FlowPlayerMovement.Initialize();

        gameOn = true;
        FlowLineGenerator.StartGame();
    }
}
