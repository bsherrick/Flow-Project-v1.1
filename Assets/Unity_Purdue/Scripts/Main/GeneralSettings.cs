using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSettings : MonoBehaviour
{
    [Header("GENERAL")]
    public GameFlowFramework_ScriptReferencer referencer;

    [Header("STARTING VALUES")]
    public InputField sHealth;
    public InputField sHits;
    public InputField sTime;

    [Header("THRESHOLD VALUES")]
    public InputField tHealth;
    public InputField tHits;
    public InputField tTime;

    [Header("TOGGLES")]
    public Toggle tHealthToggle;
    public Toggle tHitsToggle;
    public Toggle tTimeToggle;
    public Toggle showHealth;
    public Toggle showHits;
    public Toggle showTimer;
    public Toggle timerCountdown;

    //other
    public int uiIndex;
    public GeneralSettingsPreset[] genPreset;
    public Text indexName;

    void Start()
    {
        genPreset = new GeneralSettingsPreset[3];
        for (int i = 0; i < 3; i++)
        {
            genPreset[i] = new GeneralSettingsPreset();
            genPreset[i].sHealth = 30;
            genPreset[i].sHits = 0;
            genPreset[i].sTime = 0;
            genPreset[i].tHealth = 0;
            genPreset[i].tHits = 0;
            genPreset[i].tTime = 0;
            genPreset[i].showHealth = true;
            genPreset[i].showHits = true;
            genPreset[i].showTimer = true;
            genPreset[i].timerCountdown = false;
            genPreset[i].tHealthToggle = true;
            genPreset[i].tHitsToggle = false;
            genPreset[i].tTimeToggle = false;
        }
        uiIndex = 0;
        SetPreset(0);
    }
    
    void Update()
    {

    }

    public void SetPreset(int i)
    {
        indexName.text = (i + 1) + " of " + genPreset.Length;

        sHealth.SetTextWithoutNotify(genPreset[i].sHealth.ToString());
        sHits.SetTextWithoutNotify(genPreset[i].sHits.ToString());
        sTime.SetTextWithoutNotify(genPreset[i].sTime.ToString());

        tHealth.SetTextWithoutNotify(genPreset[i].tHealth.ToString());
        tHits.SetTextWithoutNotify(genPreset[i].tHits.ToString());
        tTime.SetTextWithoutNotify(genPreset[i].tTime.ToString());

        tHealthToggle.SetIsOnWithoutNotify(genPreset[i].tHealthToggle);
        tHitsToggle.SetIsOnWithoutNotify(genPreset[i].tHitsToggle);
        tTimeToggle.SetIsOnWithoutNotify(genPreset[i].tTimeToggle);
        showHealth.SetIsOnWithoutNotify(genPreset[i].showHealth);
        showHits.SetIsOnWithoutNotify(genPreset[i].showHits);
        showTimer.SetIsOnWithoutNotify(genPreset[i].showTimer);
        timerCountdown.SetIsOnWithoutNotify(genPreset[i].timerCountdown);

        referencer.CanvasScript.SetStartingValues(genPreset[i]);
        referencer.GameFlowFramework_WinLoseCondition.SetThresholds(genPreset[i]);
        referencer.GameFlowFramework_WinLoseCondition.ToggleThresholds(genPreset[i]);
        referencer.GameFlowFramework_Environment.ToggleUI(genPreset[i]);
    }

    public void SetValue()
    {
        int i = uiIndex;

        genPreset[i].sHealth = int.Parse(sHealth.text);
        genPreset[i].sHits = int.Parse(sHits.text);
        genPreset[i].sTime = int.Parse(sTime.text);
        genPreset[i].tHealth = int.Parse(tHealth.text);
        genPreset[i].tHits = int.Parse(tHits.text);
        genPreset[i].tTime = int.Parse(tTime.text);
        genPreset[i].tHealthToggle = tHealthToggle.isOn;
        genPreset[i].tHitsToggle = tHitsToggle.isOn;
        genPreset[i].tTimeToggle = tTimeToggle.isOn;
        genPreset[i].showHealth = showHealth.isOn;
        genPreset[i].showHits = showHits.isOn;
        genPreset[i].showTimer = showTimer.isOn;
        genPreset[i].timerCountdown = timerCountdown.isOn;

        SetPreset(i);
    }
    public void NextQuestion(bool b)
    {
        if (b)
        {
            //next
            if (uiIndex >= genPreset.Length - 1) { uiIndex = 0; } else { uiIndex += 1; }

        }
        else
        {
            //prev
            if (uiIndex <= 0) { uiIndex = genPreset.Length - 1; } else { uiIndex -= 1; }
        }

        SetPreset(uiIndex);
    }

}

public class GeneralSettingsPreset
{
    [Header("STARTING VALUES")]
    public int sHealth = 30;
    public int sHits = 0;
    public int sTime = 0;

    [Header("THRESHOLD VALUES")]
    public int tHealth = 0;
    public int tHits = 0;
    public int tTime = 0;

    [Header("TOGGLES")]
    public bool tHealthToggle = true;
    public bool tHitsToggle = false;
    public bool tTimeToggle = false;
    public bool showHealth = true;
    public bool showHits = true;
    public bool showTimer = true;
    public bool timerCountdown = false;
}
