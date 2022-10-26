using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmValues : MonoBehaviour
{
    //general
    [Tooltip("Choppy or Smooth Movement")] public int gamePlay; //AlgorithmSmooth or AlgorithmChop
    [Tooltip("Lines to delay game start")] public int delayStart;
    [Tooltip("Lines to delay game end")] public int delayEnd;

    //algorithm editor
    [Tooltip("Whether the algorithm uses a seed or is completely Random")] public bool setSeed;
    [Tooltip("Seed the algorithm is based on")] public int algorithmSeed;
    [Tooltip("The starting speed of lines.")] public int algorithmSpeed;
    [Tooltip("Amount of lines til ship speed increases")] public int speedChangeLines;
    [Tooltip("Max line cooldown amount between lines")] public int maxLineCooldown;
    [Tooltip("Whether the algorithm uses adaptive difficulty")] public bool isAdaptiveDifficulty;
    [Tooltip("Amount of Rocks hit till difficulty adapts")] public int adaptiveRocks;
    [Tooltip("Amount of Stars hit till difficulty adapts")] public int adaptiveStars;
    [Tooltip("Amount of lines til difficulty increases")] public int difficultyChangeLines;
    [Tooltip("Min lines between star spawns")] public int minBetweenStars;
    [Tooltip("Max lines between star spawns")] public int maxBetweenStars;

    //general
    [Tooltip("Health Reqired to Win")] public int w_health;
    [Tooltip("Score Reqired to Win")] public int w_score;
    [Tooltip("Time Reqired to Win")] public int w_time;
    [Tooltip("Health Reqired to Lose")] public int l_health;
    [Tooltip("Score Reqired to Lose")] public int l_score;
    [Tooltip("Time Reqired to Lose")] public int l_time;
    [Tooltip("Health Reqired to Win Activated")] public bool w_healthOn;
    [Tooltip("Score Reqired to Win Activated")] public bool w_scoreOn;
    [Tooltip("Time Reqired to Win Activated")] public bool w_timeOn;
    [Tooltip("Health Reqired to Lose Activated")] public bool l_healthOn;
    [Tooltip("Score Reqired to Lose Activated")] public bool l_scoreOn;
    [Tooltip("Time Reqired to Lose Activated")] public bool l_timeOn;
    [Tooltip("Starting Health")] public int sHealth;
    [Tooltip("Starting Score")] public int sScore;
    [Tooltip("Starting Time")] public int sTime;
    [Tooltip("Timer Countdown Activated")] public bool timerCountdown;

    //user interface
    [Tooltip("Show Health in UI")] public bool ui_showHealth;
    [Tooltip("Show Score in UI")] public bool ui_showScore;
    [Tooltip("Show Time in UI")] public bool ui_showTime;
    [Tooltip("Show Progress Bar in UI")] public bool ui_showProgressBar;
    [Tooltip("Show Objective in UI")] public bool ui_showObjective;
    [Tooltip("Name of Health")] public string ui_textHealth;
    [Tooltip("Name of Score")] public string ui_textScore;
    [Tooltip("Name of Objective")] public string ui_textObjective;
    [Tooltip("Name of Win Message")] public string ui_textWinMessage;
    [Tooltip("Name of Lose Message")] public string ui_textLoseMessage;

    private void Start(){

        //general
        gamePlay = FlowGameConfig.gamePlay_AlgorithmChop;
        delayStart = 0;
        delayEnd = 0;

        //algorithm editor
        setSeed = false;
        algorithmSeed = 0;
        algorithmSpeed = 4;
        speedChangeLines = 11;
        maxLineCooldown = 6;
        difficultyChangeLines = 20;
        maxBetweenStars = 8;
        minBetweenStars = 4;  
        isAdaptiveDifficulty = true;
        adaptiveRocks = 4;
        adaptiveStars = 4;

        //general
        w_health = 0;
        w_score = 25;
        w_time = 0;
        l_health = 0;
        l_score = 0;
        l_time = 0;
        w_healthOn = false;
        w_scoreOn = true;
        w_timeOn = false;
        l_healthOn = true;
        l_scoreOn = false;
        l_timeOn = false;
        sHealth = 25;
        sScore = 0;
        sTime = 0;
        timerCountdown = false;

        //user interface
        ui_showHealth = true;
        ui_showScore = true;
        ui_showTime = true;
        ui_showProgressBar = true;
        ui_showObjective = true;
        ui_textHealth = "ARMOR";
        ui_textScore = "STARS";
        ui_textObjective = "COLLECT 25 STARS";
        ui_textWinMessage = "YOU WON!";
        ui_textLoseMessage = "YOU LOST!";
    }
}
