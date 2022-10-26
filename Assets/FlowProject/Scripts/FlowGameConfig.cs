using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FlowGameConfig : MonoBehaviour
{
    //Gameplay Settings
    public const int gamePlay_ClassicSmooth = 0;
    public const int gamePlay_ClassicChop = 1;
    public const int gamePlay_Rhythm = 2;
    public const int gamePlay_AlgorithmSmooth = -1;
    public const int gamePlay_AlgorithmChop = -2;

    [HideInInspector] public FlowMain flow;

    public bool showDebug; //Display debugging messages?
    
    [Header("Settings (Editor Only):")]
    #region settings
    public ParticleSystem[] hitEffects; //[Tooltip("List of particle effects to play when the player gets hit.")] 
    public ParticleSystem[] scoreEffects; //[Tooltip("List of particle effects to play when the player scores.")]
    #endregion

    [Header("Settings (To be configured by Gamemode):")]
    #region settings
    //GAMEMODE
    public int gameMode = 0;
    //GENERAL
    [Tooltip("Gameplay settings.")]
    public int gamePlay = 0;
    public float delayStart = 0;
    public float delayEnd = 0;
    //(For map settings see FlowLineGenerator)
    //GAMEPLAY
    [Tooltip("Win health.")] public int w_health = 0;
    [Tooltip("Win score.")] public int w_score = 0;
    [Tooltip("Win time.")] public float w_time = 0;
    [Tooltip("Win health condition enabled?")] public bool w_healthOn = false;
    [Tooltip("Win score condition enabled?")] public bool w_scoreOn = false;
    [Tooltip("Win time condition enabled?")] public bool w_timeOn = false;
    [Tooltip("Lose health.")] public int l_health = 0;
    [Tooltip("Lose score.")] public int l_score = 0;
    [Tooltip("Lose time.")] public float l_time = 0;
    [Tooltip("Lose health condition enabled?")] public bool l_healthOn = false;
    [Tooltip("Lose score condition enabled?")] public bool l_scoreOn = false;
    [Tooltip("Lose time condition enabled?")] public bool l_timeOn = false;
    [Tooltip("Starting health.")] public int sHealth = 0;
    [Tooltip("Starting score.")] public int sScore = 0;
    [Tooltip("Starting time in seconds.")] public float sTime = 0;
    [Tooltip("Does the timer countdown?")] public bool timerCountdown = false;
    //(For UI settings see FlowCanvas)
    #endregion

    [Header("Status (Per game session):")]
    #region
    //Rhythm Mode
    [Tooltip("=hit/hitAmount or =(hitAmount-miss)/hitAmount in percentage (Rhythm Mode).")] public float accuracy = 100;
    [Tooltip("player hits (Rhythm Mode).")] public int hit = 0;
    [Tooltip("Player misses (Rhythm Mode).")] public int miss = 0;
    [Tooltip("Total available beats to hit.")] public int hitAmount = 0;
    //Classic Mode
    [Tooltip("Current health.")] public int health = 0;
    [Tooltip("Total times rocks have been hit.")] public int rockHits = 0;
    [Tooltip("Current score.")] public int score = 0;
    [Tooltip("The formatted string to be displayed in the UI.")] public string timer = "00:00 (m:s)";
    [Tooltip("Game Progress.")] public float progress = 0;
    [Tooltip("Time in-game (factors in starting time and countdown condition)")] public float timeInGame = 0;
    [Tooltip("Time since start of current game session.")] public float timeSinceStartOfGame = 0;

    #endregion

    [Header("Status (No Reset):")]
    #region
    [Tooltip("Time since start of the application.")] public float timeSinceStartOfApplication = 0;
    #endregion

    int timerInt;
    string minutes;
    string seconds;

    public void Initialize()
    {
        StopAllCoroutines();

        //Rhythm Mode
        accuracy = 100;
        hit = 0;
        miss = 0;
        hitAmount = 0;
        //Classic Mode
        health = 0;
        score = 0;
        timer = "00:00 (m:s)";
        progress = 0;
        timeInGame = 0;

        #region general

        timerInt = 0;
        TimeConvertToString();
        progress = 0;

        #endregion

        #region set values

        //Classic Mode
        health = sHealth;
        score = sScore;
        timeInGame = sTime;
        flow.FlowCanvas.inGameClassic_textHealth.text = flow.FlowCanvas.ui_textHealth + ": " + health;
        flow.FlowCanvas.inGameClassic_textScore.text = flow.FlowCanvas.ui_textScore + ": " + score;

        //Rhythm Mode
        accuracy = 100;
        hit = 0;
        miss = 0;
        //hitAmount = 0;
        flow.FlowCanvas.inGameRhythm_textAccuracy.text = "100%";

        #endregion
    }

    public void TimeConvertToString()
    {
        timerInt = Mathf.RoundToInt(timeInGame); //round to int
        if (timerInt < 0)
        {
            timerInt *= -1;
            minutes = "-";
        }
        else
        {
            minutes = "";
        }
        minutes = minutes + (timerInt / 60).ToString("D2");
        seconds = (timerInt % 60).ToString("D2");
        timer = minutes + ":" + seconds; //set timer to be displayed in UI
        if (gamePlay != gamePlay_Rhythm) {timer += " (m:s)";}
    }

    void Update()
    {
        timeSinceStartOfApplication += Time.deltaTime; //update time since the application was run

        if (!flow.gameOn)
        {
            return;
        }

        #region time
        //TIMER
        timeSinceStartOfGame += Time.deltaTime;
        if (timerCountdown && gamePlay != gamePlay_Rhythm)
        {
            timeInGame -= Time.deltaTime; //timer will decrement
        }
        else
        {
            timeInGame += Time.deltaTime; //timer will increment
        }
        #endregion

        #region win/lose conditions
        //game won
        if (w_timeOn && Mathf.RoundToInt(timeInGame) == w_time)
        {
            flow.gameOn = false;
            EndGame(true);
        }

        //game lost
        if (l_timeOn && Mathf.RoundToInt(timeInGame) == l_time)
        {
            flow.gameOn = false;
            EndGame(false);
        }
        #endregion

        TimeConvertToString(); //Convert time to string
        flow.FlowCanvas.inGameClassic_textTimer.text = timer; //Update UI
        flow.FlowCanvas.inGameRhythm_textTimer.text = timer; //Update UI
        flow.FlowQuestionHandler.TriggerTime(Mathf.RoundToInt(timeInGame)); //trigger questions
    }

    /// <summary>
    /// When the character is hit by a line object.
    /// </summary>
    /// <param name="type">
    /// 0: empty
    /// 1: obstacle
    /// 2: collectible
    /// </param>
    public void PlayerHit(int type)
    {
        if (showDebug) { Debug.Log("HitPlayer() type: " + type); }

        switch (type)
        {
            case 0: //empty
                break;
            case 1: //obstacle
                health--;
                rockHits++;
                flow.FlowCanvas.inGameClassicTextChange(0, flow.FlowCanvas.ui_textHealth + ": " + health);
                flow.FlowPlayerMovement.ShakeModel(); //make the player shake
                flow.audioExplode.Play(); //play hit sound
                flow.FlowQuestionHandler.TriggerHealth(health); //trigger questions
                if (flow.FlowLineGenerator.isAdaptiveDifficulty && ((rockHits + 1) % flow.FlowLineGenerator.adaptiveRocks == 0))
                    flow.FlowLineGenerator.AdaptiveDifficultyChange(1);
                for (int i = 0; i < hitEffects.Length; i++) {
                    hitEffects[i].Play(); //play effects
                } 
                CheckHealth();
                break;
            case 2: //collectible
                score++;
                flow.FlowCanvas.inGameClassicTextChange(1, flow.FlowCanvas.ui_textScore + ": " + score);
                flow.audioLaserShoot.Play(); //play score sound
                flow.FlowQuestionHandler.TriggerScore(score); //trigger questions
                if (flow.FlowLineGenerator.isAdaptiveDifficulty && ((score + 1) % flow.FlowLineGenerator.adaptiveStars == 0))
                    flow.FlowLineGenerator.AdaptiveDifficultyChange(2);
                for (int i = 0; i < scoreEffects.Length; i++) { scoreEffects[i].Play(); } //play effects
                CheckScore();
                break;
            default: //error
                Debug.Log("Error! Invalid object type!");
                break;
        }
    }

    public void PlayerHitQuestion(int addHealth, int addScore)
    {
        health += addHealth;
        score += addScore;
        flow.audioLaserShoot.Play();
        for (int i = 0; i < scoreEffects.Length; i++) { scoreEffects[i].Play(); } //play effects

        flow.FlowCanvas.inGameClassicTextChange(0, flow.FlowCanvas.ui_textHealth + ": " + health);
        flow.FlowCanvas.inGameClassicTextChange(1, flow.FlowCanvas.ui_textScore + ": " + score);

        flow.FlowQuestionHandler.QuestionAnswered();
    }

    /// <summary>
    /// (Rhythm Mode) When the target hits a beat.
    /// </summary>
    /// <param name="type">
    /// 0: miss
    /// 1: hit
    /// </param>
    public void TargetHit(int type)
    {
        switch (type)
        {
            case 0: //miss
                miss++;
                accuracy = (int)((((float)hitAmount - (float)miss) / (float)hitAmount) * 100);
                flow.FlowCanvas.inGameRhythmTextChange(0, accuracy + "%");
                break;
            case 1: //hit
                hit++;
                flow.FlowCanvas.inGameRhythmTextChange(1, "HITS: " + hit + "/" + hitAmount);
                break;
        }
        CheckRhythmDone();
    }

    #region win/lose/end conditions

    void CheckHealth()
    {
        if (!flow.gameOn)
        {
            return;
        }

        //game won
        if (w_healthOn && health == w_health)
        {
            flow.gameOn = false;
            EndGame(true);
        }

        //game lost
        if (l_healthOn && health == l_health)
        {
            flow.gameOn = false;
            EndGame(false);
        }
    }

    void CheckScore()
    {
        if (!flow.gameOn)
        {
            return;
        }

        //game won
        if (w_scoreOn && score == w_score)
        {
            flow.gameOn = false;
            EndGame(true);
        }

        //game lost
        if (l_scoreOn && score == l_score)
        {
            flow.gameOn = false;
            EndGame(false);
        }
    }

    void CheckRhythmDone()
    {
        if (!flow.gameOn)
        {
            return;
        }

        if (hitAmount == hit + miss)
        {
            flow.gameOn = false;
            EndGame(true); //end game
        }
    }

    #endregion

    public void EndGame(bool b)
    {
        StartCoroutine(EndGameWait(b));
    }

    IEnumerator EndGameWait(bool b)
    {
        yield return new WaitForSeconds(delayEnd);
        flow.FlowCanvas.GameEnd(b);
    }

    public string CurrentGameStatus()
    {
        string status = "";

        //Identification
        status += "ID=[" + flow.FlowCanvas.startingScreen_inputFieldID.text + "]" + "\t";

        //Gamemode
        switch (gameMode)
        {
            case -1:
                status += "GAMEMODE=ALGORITHM" + "\t";
                break;
            case 0:
                status += "GAMEMODE=001" + "\t";
                break;
            case 1:
                status += "GAMEMODE=002" + "\t";
                break;
            case 2:
                status += "GAMEMODE=003" + "\t";
                break;
        }
        switch (gamePlay)
        {
            case gamePlay_ClassicSmooth:
                status += "GAMEPLAY=ClassicSmooth" + "\t";
                break;
            case gamePlay_ClassicChop:
                status += "GAMEPLAY=ClassicChop" + "\t";
                break;
            case gamePlay_Rhythm:
                status += "GAMEPLAY=Rhythm" + "\t";
                break;
        }

        //GameStats
        status += "HEALTH=" + health + "\t";
        status += "SCORE=" + score + "\t";
        status += "TIMER_VALUE_RAW=" + timeInGame + "\t";

        //Questions
        for (int i = 0; i < flow.FlowQuestionHandler.qtAnswers.Count; i++)
        {
            status += flow.FlowQuestionHandler.qtAnswers[i] + "\t";
        }

        return status;
    }
}