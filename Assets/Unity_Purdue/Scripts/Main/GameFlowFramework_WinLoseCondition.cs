using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class GameFlowFramework_WinLoseCondition : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ReportResults(string str);

    //Default Values
    /****************************************************************************/
    //ACTIVE THRESHOLD VALUES:
    static bool healthThresholdIsActive_Default;
    static bool hitsThresholdIsActive_Default;
    static bool scoreThresholdIsActive_Default;
    static bool timerThresholdIsActive_Default;
    //THRESHOLD VALUES:
    static int healthThreshold_Default;
    static int hitsThreshold_Default;
    static int scoreThreshold_Default;
    static int timerThreshold_Default;
    /****************************************************************************/

    [Header("ACTIVE THRESHOLD VALUES:")]
    [Header("__________________________________________________________________")]
    public bool healthThresholdIsActive = healthThresholdIsActive_Default;
    public bool hitsThresholdIsActive = hitsThresholdIsActive_Default;
    public bool scoreThresholdIsActive = scoreThresholdIsActive_Default;
    public bool timerThresholdIsActive = timerThresholdIsActive_Default;

    [Header("THRESHOLD VALUES:")]
    [Header("__________________________________________________________________")]
    public int healthThreshold = healthThreshold_Default;
    public int hitsThreshold = hitsThreshold_Default;
    public int scoreThreshold = scoreThreshold_Default;
    public int timerThreshold = timerThreshold_Default;

    [Header("OTHER:")]
    [Header("__________________________________________________________________")]
    public List<string> questionsAnswered;
    public int focusMode = 0;

    GameFlowFramework_ScriptReferencer referencer;

    void Start()
    {
        Debug.Log("*----Game Started----*");
        referencer = GetComponent<GameFlowFramework_ScriptReferencer>();
        questionsAnswered = new List<string>();
    }

    public void ToggleThresholds(GeneralSettingsPreset gen)
    {
        healthThresholdIsActive = gen.tHealthToggle;
        hitsThresholdIsActive = gen.tHitsToggle;
        scoreThresholdIsActive = false;
        timerThresholdIsActive = gen.tTimeToggle;
    }

    public void SetThresholds(GeneralSettingsPreset gen)
    {
        healthThreshold = gen.tHealth;
        hitsThreshold = gen.tHits;
        scoreThreshold = 0;
        timerThreshold = gen.tTime;
    }

    public void CheckConditions(int health, int hit, int score, int time)
    {
        if (!referencer.CanvasScript.gameHasStarted)
        {
            return;
        }

        if (healthThresholdIsActive && healthThreshold == health)
        {
            referencer.CanvasScript.GameOver();
        }

        if (hitsThresholdIsActive && hitsThreshold == hit)
        {
            referencer.CanvasScript.GameOver();
        }

        if (scoreThresholdIsActive && scoreThreshold == score)
        {
            referencer.CanvasScript.GameOver();
        }

        if (timerThresholdIsActive && timerThreshold == time)
        {
            referencer.CanvasScript.GameOver();
        }
    }

    public void SetPlayerPrefs()
    {
        /*
        int i;
        if (automaticRestart)
        {
            i = 1;
            Debug.Log("Auto Restart Set to TRUE");
        }
        else
        {
            i = 0;
            Debug.Log("Auto Restart Set to FALSE");
        }
        PlayerPrefs.SetInt("skipStartingScreen", i);
        */
    }

    public void GetResults()
    {
        string result = "ID=" + referencer.CanvasScript.identification.text + "\t"
            + "Mode=00" + focusMode + "\t"
            + "UnscaledTime=" + Time.unscaledTime + "\t"
            + "Time=" + Time.time + "\t"
            + "GeneralSettings=" + referencer.GameFlowFramework_Web.rawGeneralSettings[focusMode - 1] + "\t"
            + "Preset=" + referencer.GameFlowFramework_Web.rawPresets[focusMode - 1] + "\t"
            + "HealthEnd=" + referencer.CanvasScript.healthCount + "\t"
            + "HitsEnd=" + referencer.CanvasScript.hitsCount + "\t"
            + "TimeEnd=" + referencer.CanvasScript.timeCountFormated;
        for (int i = 0; i < questionsAnswered.Count; i++)
        {
            result += ("\t" + questionsAnswered[i]);
        }
        ReportResults(result);

        /*
         * List of things to include:
         * 
         * total time the application is on
         * 
         * general settings
         * 
         * preset
         * 
         * ending health
         * ending hits
         * ending time
         * 
         * questions asnwered[]
         * 
         * 
         */
    }

    public void ResetValues()
    {  
        healthThresholdIsActive = healthThresholdIsActive_Default;
        hitsThresholdIsActive = hitsThresholdIsActive_Default;
        scoreThresholdIsActive = scoreThresholdIsActive_Default;
        timerThresholdIsActive = timerThresholdIsActive_Default;   

        healthThreshold = healthThreshold_Default;
        hitsThreshold = hitsThreshold_Default;
        scoreThreshold = scoreThreshold_Default;
        timerThreshold = timerThreshold_Default;
    }
}
