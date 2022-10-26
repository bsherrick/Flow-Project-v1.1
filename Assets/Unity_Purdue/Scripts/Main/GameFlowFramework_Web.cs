using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameFlowFramework_Web : MonoBehaviour
{

    [Header("General:")]
    [Header("__________________________________________________________________")]
    public bool debugQuestions;
    public bool debugPresets;
    public bool debugGeneralSettings;

    [Header("Web Details:")]
    [Header("__________________________________________________________________")]
    public string webAddress;
    public string[] getQ; //e.g. getQuestions.php?question=1
    public string postQ; //e.g. setQuestions.php
    public string[] getP; //e.g. getPresets.php?presetNo=1
    public string postP; //e.g. setPresets.php
    public string[] getG;
    public string postG;

    [Header("Question Details seen in UI:")]
    [Header("__________________________________________________________________")]
    public Toggle barShowAllAnswers;
    public Toggle reuse;
    public InputField spawns;
    public InputField prePatch;
    public InputField type;
    public InputField bars;
    public InputField fontSize;
    public Toggle patchNumber;
    public Toggle time;
    public Toggle health;
    public Toggle hits;
    public InputField triggerValue;
    public InputField question;
    public InputField bar1;
    public InputField bar2;
    public InputField bar3;
    public InputField bar4;
    public InputField bar5;
    public InputField bar6;
    public InputField spectrumMin;
    public InputField spectrumMax;
    public Text showIndex;
    public Text uploadQMessage;
    public Text uploadPMessage;
    public Text uploadGMessage;

    GameFlowFramework_ScriptReferencer referencer;
    GameFlowFramework_Questions q;
    GameFlowFramework_Environment env;
    GeneralSettings g;

    string[] extractedQuestions;
    string[] extractedPresets;
    string[] extractedGeneralSettings;

    bool successQ;
    bool successP;
    bool successG;

    [Header("Raw Data:")]
    [Header("__________________________________________________________________")]
    public string[] rawQuestions;
    public string[] rawPresets;
    public string[] rawGeneralSettings;

    void Start()
    {
        referencer = GetComponent<GameFlowFramework_ScriptReferencer>();
        q = referencer.GameFlowFramework_Questions;
        env = referencer.GameFlowFramework_Environment;
        g = referencer.GeneralSettings;
        extractedQuestions = new string[getQ.Length];
        extractedPresets = new string[getP.Length];
        extractedGeneralSettings = new string[getG.Length];
        rawQuestions = new string[getQ.Length];
        rawPresets = new string[getP.Length];
        rawGeneralSettings = new string[getG.Length];
        successQ = true;
        successP = true;
        successG = true;
    }

    public void All()
    {
        StartCoroutine(DownloadALL());
    }

    IEnumerator DownloadALL()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        //QUESTIONS--------------------------------------------------------------------------

        UnityWebRequest www; //= UnityWebRequest.Get(webAddress + get);

        for (int i = 0; i < getQ.Length; i++)
        {
            www = UnityWebRequest.Get(webAddress + getQ[i]);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("WEB GET[" + i + "] ERROR: " + www.error);
                extractedQuestions[i] = "NULL";
                successQ = false;
            }
            else
            {
                if (debugQuestions)
                {
                    Debug.Log("WEB GET[" + i + "] SUCCESS: " + www.downloadHandler.text);
                }
                extractedQuestions[i] = www.downloadHandler.text;
                rawQuestions[i] = www.downloadHandler.text;
            }
        }

        if (successQ)
        {
            ExtractedQuestions();
            referencer.CanvasScript.loadingMessage.text = "LOADING PRESETS...";
        }
        else
        {
            referencer.CanvasScript.loadingMessage.text = "FAILED!";
            yield break;
        }

        //PRESETS--------------------------------------------------------------------------

        for (int i = 0; i < getP.Length; i++)
        {
            www = UnityWebRequest.Get(webAddress + getP[i]);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("WEB GET[" + i + "] ERROR: " + www.error);
                extractedPresets[i] = "NULL";
                successP = false;
            }
            else
            {
                if (debugPresets)
                {
                    Debug.Log("WEB GET[" + i + "] SUCCESS: " + www.downloadHandler.text);
                }
                extractedPresets[i] = www.downloadHandler.text;
                rawPresets[i] = www.downloadHandler.text;
            }
        }

        if (successP)
        {
            ExtractedPresets();
            referencer.CanvasScript.loadingMessage.text = "LOADING SETTINGS...";
        }
        else
        {
            referencer.CanvasScript.loadingMessage.text = "FAILED!";
            yield break;
        }

        //GENERAL SETTINGS--------------------------------------------------------------------------

        for (int i = 0; i < getG.Length; i++)
        {
            www = UnityWebRequest.Get(webAddress + getG[i]);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("WEB GET[" + i + "] ERROR: " + www.error);
                extractedGeneralSettings[i] = "NULL";
                successG = false;
            }
            else
            {
                if (debugGeneralSettings)
                {
                    Debug.Log("WEB GET[" + i + "] SUCCESS: " + www.downloadHandler.text);
                }
                extractedGeneralSettings[i] = www.downloadHandler.text;
                rawGeneralSettings[i] = www.downloadHandler.text;
            }
        }

        if (successG)
        {
            ExtractedGeneralSettings();
            referencer.CanvasScript.loadingMessage.text = "SUCCESS!";
        }
        else
        {
            referencer.CanvasScript.loadingMessage.text = "FAILED!";
            yield break;
        }
        referencer.CanvasScript.Start2();
    }

    public void UploadQuestions()
    {
        uploadQMessage.text = "LOADING";
        StartCoroutine(UploadQ());
    }

    /*
    public bool DownloadQuestions()
    {
        //downloadMessage.text = "LOADING";
        StartCoroutine(DownloadQ());
        referencer.QuestionEditor.SetQuestions(referencer.QuestionEditor.currentIndex);
        return successQ;
    }
    */

    public void UploadPresets()
    {
        uploadPMessage.text = "LOADING";
        StartCoroutine(UploadP());
    }

    public void UploadGenSettings()
    {
        uploadGMessage.text = "LOADING";
        StartCoroutine(UploadG());
    }

    /*
    public bool DownloadPresets()
    {
        StartCoroutine(DownloadP());
        env.gameMode_MakeMyOwnScript.SetPreset(env.gameMode_MakeMyOwnScript.uiIndex);
        return successP;
    }
    */

    IEnumerator UploadQ()
    {
        WWWForm form = new WWWForm();

        for (int i = 0; i < q.questions.Length; i++)
        {
            form.AddField("Q" + i + "-0", "1theQuestion~" + q.questions[i].theQuestion);
            form.AddField("Q" + i + "-1", "2fontSize~" + q.questions[i].fontSize);
            form.AddField("Q" + i + "-2", "3isActive~" + q.questions[i].isActive);
            form.AddField("Q" + i + "-3", "4prePatch~" + q.questions[i].prePatch);
            form.AddField("Q" + i + "-4", "5questionType~" + q.questions[i].questionType);
            form.AddField("Q" + i + "-5", "6barQuestionAmount~" + q.questions[i].barQuestionAmount);
            form.AddField("Q" + i + "-6", "7barQuestionAnswers1~" + q.questions[i].barQuestionAnswers[0]);
            form.AddField("Q" + i + "-7", "8barQuestionAnswers2~" + q.questions[i].barQuestionAnswers[1]);
            form.AddField("Q" + i + "-8", "9barQuestionAnswers3~" + q.questions[i].barQuestionAnswers[2]);
            form.AddField("Q" + i + "-9", "10barQuestionAnswers4~" + q.questions[i].barQuestionAnswers[3]);
            form.AddField("Q" + i + "-10", "11barQuestionAnswers5~" + q.questions[i].barQuestionAnswers[4]);
            form.AddField("Q" + i + "-11", "12barQuestionAnswers6~" + q.questions[i].barQuestionAnswers[5]);
            form.AddField("Q" + i + "-12", "13barShowAllAnswers~" + q.questions[i].barShowAllAnswers);
            form.AddField("Q" + i + "-13", "14spectrumQuestionMin~" + q.questions[i].spectrumQuestionMin);
            form.AddField("Q" + i + "-14", "15spectrumQuestionMax~" + q.questions[i].spectrumQuestionMax);
            form.AddField("Q" + i + "-15", "16spectrumDecimalPlace~" + q.questions[i].spectrumDecimalPlace);
            form.AddField("Q" + i + "-16", "17triggerPatch~" + q.questions[i].triggerPatch);
            form.AddField("Q" + i + "-17", "18triggerTime~" + q.questions[i].triggerTime);
            form.AddField("Q" + i + "-18", "19triggerHealth~" + q.questions[i].triggerHealth);
            form.AddField("Q" + i + "-19", "20triggerHits~" + q.questions[i].triggerHits);
            form.AddField("Q" + i + "-20", "21triggerScore~" + q.questions[i].triggerScore);
            form.AddField("Q" + i + "-21", "22reuseQuestion~" + q.questions[i].reuseQuestion);
            form.AddField("Q" + i + "-22", "23spawnAmount~" + q.questions[i].spawnAmount);
            form.AddField("Q" + i + "-23", "24valuePatch~" + q.questions[i].valuePatch);
            form.AddField("Q" + i + "-24", "25valueTime~" + q.questions[i].valueTime);
            form.AddField("Q" + i + "-25", "26valueHealth~" + q.questions[i].valueHealth);
            form.AddField("Q" + i + "-26", "27valueHits~" + q.questions[i].valueHits);
            form.AddField("Q" + i + "-27", "28valueScore~" + q.questions[i].valueScore);
        }

        UnityWebRequest www = UnityWebRequest.Post(webAddress + postQ, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("WEB POST ERROR: " + www.error);
            uploadQMessage.text = "FAILED";
        }
        else
        {
            Debug.Log("WEB POST SUCCESS: " + www.downloadHandler.text);
            uploadQMessage.text = "SUCCESS";
        }
        yield return new WaitForSecondsRealtime(2);
        uploadQMessage.text = "";
    }

    /*
    IEnumerator DownloadQ()
    {
        UnityWebRequest www; //= UnityWebRequest.Get(webAddress + get);

        for (int i = 0; i < getQ.Length; i++)
        {
            www = UnityWebRequest.Get(webAddress + getQ[i]);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("WEB GET[" + i + "] ERROR: " + www.error);
                extractedQuestions[i] = "NULL";
                successQ = false;
            }
            else
            {
                if (enableDebugMode)
                {
                    Debug.Log("WEB GET[" + i + "] SUCCESS: " + www.downloadHandler.text);
                }
                extractedQuestions[i] = www.downloadHandler.text;
            }
        }

        if (successQ)
        {
            ExtractedQuestions();
            //downloadMessage.text = "SUCCESS";
        }
        else
        {
            //downloadMessage.text = "FAILED";
        }

        //yield return new WaitForSecondsRealtime(2);
        //downloadMessage.text = "";
    }
    */

    IEnumerator UploadP()
    {
        WWWForm form = new WWWForm();

        for (int i = 0; i < env.presets.Length; i++)
        {
            form.AddField("P" + i + "-0", "1name~" + env.presets[i].name);
            form.AddField("P" + i + "-1", "2seed~" + env.presets[i].seed);
            form.AddField("P" + i + "-2", "3iterations~" + env.presets[i].iterations);
            form.AddField("P" + i + "-3", "4isEndless~" + env.presets[i].isEndless);
            form.AddField("P" + i + "-4", "5difficulty~" + env.presets[i].difficulty);
            form.AddField("P" + i + "-5", "6diffIsActive~" + env.presets[i].diffIsActive);
            form.AddField("P" + i + "-6", "7usePercentage~" + env.presets[i].usePercentage);

            if (debugPresets)
            {
                Debug.Log("P" + i + "-0 + " + "1name~" + env.presets[i].name);
                Debug.Log("P" + i + "-1 + " + "2seed~" + env.presets[i].seed);
                Debug.Log("P" + i + "-2 + " + "3iterations~" + env.presets[i].iterations);
                Debug.Log("P" + i + "-3 + " + "4isEndless~" + env.presets[i].isEndless);
                Debug.Log("P" + i + "-4 + " + "5difficulty~" + env.presets[i].difficulty);
                Debug.Log("P" + i + "-5 + " + "6diffIsActive~" + env.presets[i].diffIsActive);
                Debug.Log("P" + i + "-6 + " + "7usePercentage~" + env.presets[i].usePercentage);
            }
        }

        UnityWebRequest www = UnityWebRequest.Post(webAddress + postP, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("WEB POST ERROR: " + www.error);
            uploadPMessage.text = "FAILED";
        }
        else
        {
            Debug.Log("WEB POST SUCCESS: " + www.downloadHandler.text);
            uploadPMessage.text = "SUCCESS";
        }
        yield return new WaitForSecondsRealtime(2);
        uploadPMessage.text = "";
    }

    /*
    IEnumerator DownloadP()
    {
        UnityWebRequest www; //= UnityWebRequest.Get(webAddress + get);

        for (int i = 0; i < getP.Length; i++)
        {
            www = UnityWebRequest.Get(webAddress + getP[i]);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("WEB GET[" + i + "] ERROR: " + www.error);
                extractedPresets[i] = "NULL";
                successP = false;
            }
            else
            {
                if (enableDebugMode)
                {
                    Debug.Log("WEB GET[" + i + "] SUCCESS: " + www.downloadHandler.text);
                }
                extractedPresets[i] = www.downloadHandler.text;
            }
        }

        if (successP)
        {
            ExtractedPresets();
        }
    }
    */

    IEnumerator UploadG()
    {
        WWWForm form = new WWWForm();

        for (int i = 0; i < g.genPreset.Length; i++)
        {
            form.AddField("G" + i + "-0", "1sHealth~" + g.genPreset[i].sHealth);
            form.AddField("G" + i + "-1", "2sHits~" + g.genPreset[i].sHits);
            form.AddField("G" + i + "-2", "3sTime~" + g.genPreset[i].sTime);
            form.AddField("G" + i + "-3", "4tHealth~" + g.genPreset[i].tHealth);
            form.AddField("G" + i + "-4", "5tHits~" + g.genPreset[i].tHits);
            form.AddField("G" + i + "-5", "6tTime~" + g.genPreset[i].tTime);
            form.AddField("G" + i + "-6", "7showHealth~" + g.genPreset[i].showHealth);
            form.AddField("G" + i + "-7", "8showHits~" + g.genPreset[i].showHits);
            form.AddField("G" + i + "-8", "9showTimer~" + g.genPreset[i].showTimer);
            form.AddField("G" + i + "-9", "10timerCountdown~" + g.genPreset[i].timerCountdown);
            form.AddField("G" + i + "-10", "11tHealthToggle~" + g.genPreset[i].tHealthToggle);
            form.AddField("G" + i + "-11", "12tHitsToggle~" + g.genPreset[i].tHitsToggle);
            form.AddField("G" + i + "-12", "13tTimeToggle~" + g.genPreset[i].tTimeToggle);            
        }

        if (debugGeneralSettings)
        {
            Debug.Log("-----GENERAL SETTINGS POST FORM [START]-----");
            for (int i = 0; i < g.genPreset.Length; i++)
            {
                Debug.Log("G" + i + "-0: 1sHealth~" + g.genPreset[i].sHealth);
                Debug.Log("G" + i + "-1: 2sHits~" + g.genPreset[i].sHits);
                Debug.Log("G" + i + "-2: 3sTime~" + g.genPreset[i].sTime);
                Debug.Log("G" + i + "-3: 4tHealth~" + g.genPreset[i].tHealth);
                Debug.Log("G" + i + "-4: 5tHits~" + g.genPreset[i].tHits);
                Debug.Log("G" + i + "-5: 6tTime~" + g.genPreset[i].tTime);
                Debug.Log("G" + i + "-6: 7showHealth~" + g.genPreset[i].showHealth);
                Debug.Log("G" + i + "-7: 8showHits~" + g.genPreset[i].showHits);
                Debug.Log("G" + i + "-8: 9showTimer~" + g.genPreset[i].showTimer);
                Debug.Log("G" + i + "-9: 10timerCountdown~" + g.genPreset[i].timerCountdown);
                Debug.Log("G" + i + "-10: 11tHealthToggle~" + g.genPreset[i].tHealthToggle);
                Debug.Log("G" + i + "-11: 12tHitsToggle~" + g.genPreset[i].tHitsToggle);
                Debug.Log("G" + i + "-12: 13tTimeToggle~" + g.genPreset[i].tTimeToggle);
            }
            Debug.Log("-----GENERAL SETTINGS POST FORM [END]-----");
        }

        UnityWebRequest www = UnityWebRequest.Post(webAddress + postG, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("WEB POST ERROR: " + www.error);
            uploadGMessage.text = "FAILED";
        }
        else
        {
            Debug.Log("WEB POST SUCCESS: " + www.downloadHandler.text);
            uploadGMessage.text = "SUCCESS";
        }
        yield return new WaitForSecondsRealtime(2);
        uploadGMessage.text = "";
    }

    void ExtractedQuestions()
    {
        for (int i = 0; i < extractedQuestions.Length; i++)
        {
            if (extractedQuestions[i] == "NULL")
            {
                if (debugQuestions)
                {
                    Debug.Log("-----QUESTION " + i + " [START]-----");
                    Debug.Log("NULL");
                    Debug.Log("-----QUESTION " + i + " [END]-----");
                }
            }
            else
            {
                string[] data = extractedQuestions[i].Split('\'');

                if (debugQuestions)
                {
                    Debug.Log("-----QUESTION " + i + " [START]-----");
                    for (int j = 0; j < data.Length; j++)
                    {
                        Debug.Log("data[" + j + "]: " + data[j]);
                    }
                    Debug.Log("-----QUESTION " + i + " [END]-----");
                }
                for (int j = 0; j < data.Length; j++)
                {
                    data[j] = data[j].Substring(data[j].IndexOf('~') + 1);
                    if (debugQuestions)
                    {
                        if (j != data.Length - 1)
                        {
                            Debug.Log("CONCAT[" + i + "]: " + data[j]);
                        }
                        else
                        {
                            data[j] = data[j].Substring(0, data[j].Length - 1);
                            Debug.Log("CONCAT LAST[" + i + "]: " + data[j] + "|||");
                        }
                    }
                }

                string theQuestion = data[0];
                int fontSize = int.Parse(data[1]);

                bool isActive;
                if (data[2].ToLower() == "true") { isActive = true; } else { isActive = false; }

                int prePatch = int.Parse(data[3]);
                int questionType = int.Parse(data[4]);
                int barQuestionAmount = int.Parse(data[5]);

                string barQuestionAnswers1 = data[6];
                string barQuestionAnswers2 = data[7];
                string barQuestionAnswers3 = data[8];
                string barQuestionAnswers4 = data[9];
                string barQuestionAnswers5 = data[10];
                string barQuestionAnswers6 = data[11];

                bool barShowAllAnswers = true;
                if (data[12].ToLower() == "true") { barShowAllAnswers = true; } else { barShowAllAnswers = false; }

                float spectrumQuestionMin = float.Parse(data[13]);
                float spectrumQuestionMax = float.Parse(data[14]);
                int spectrumDecimalPlace = int.Parse(data[15]);

                bool triggerPatch;
                bool triggerTime;
                bool triggerHealth;
                bool triggerHits;
                bool triggerScore;
                bool reuseQuestion;
                if (data[16].ToLower() == "true") { triggerPatch = true; } else { triggerPatch = false; }
                if (data[17].ToLower() == "true") { triggerTime = true; } else { triggerTime = false; }
                if (data[18].ToLower() == "true") { triggerHealth = true; } else { triggerHealth = false; }
                if (data[19].ToLower() == "true") { triggerHits = true; } else { triggerHits = false; }
                if (data[20].ToLower() == "true") { triggerScore = true; } else { triggerScore = false; }
                if (data[21].ToLower() == "true") { reuseQuestion = true; } else { reuseQuestion = false; }

                int spawnAmount = int.Parse(data[22]);
                int valuePatch = int.Parse(data[23]);
                int valueTime = int.Parse(data[24]);
                int valueHealth = int.Parse(data[25]);
                int valueHits = int.Parse(data[26]);
                int valueScore = int.Parse(data[27]);

                if (debugQuestions)
                {
                    Debug.Log("-----QUESTION " + i + " [START]-----");

                    Debug.Log("- 1. theQuestion: " + theQuestion);
                    Debug.Log("- 2. fontSize: " + fontSize);
                    Debug.Log("- 3. isActive: " + isActive);
                    Debug.Log("- 4. prePatch: " + prePatch);
                    Debug.Log("- 5. questionType: " + questionType);
                    Debug.Log("- 6. barQuestionAmount: " + barQuestionAmount);
                    Debug.Log("- 7. barQuestionAnswers1: " + barQuestionAnswers1);
                    Debug.Log("- 8. barQuestionAnswers2: " + barQuestionAnswers2);
                    Debug.Log("- 9. barQuestionAnswers3: " + barQuestionAnswers3);
                    Debug.Log("- 10. barQuestionAnswers4: " + barQuestionAnswers4);
                    Debug.Log("- 11. barQuestionAnswers5: " + barQuestionAnswers5);
                    Debug.Log("- 12. barQuestionAnswers6: " + barQuestionAnswers6);
                    Debug.Log("- 13. barShowAllAnswers: "  + barShowAllAnswers);
                    Debug.Log("- 14. spectrumQuestionMin: " + spectrumQuestionMin);
                    Debug.Log("- 15. spectrumQuestionMax: " + spectrumQuestionMax);
                    Debug.Log("- 16. spectrumDecimalPlace: " + spectrumDecimalPlace);
                    Debug.Log("- 17. triggerPatch: " + triggerPatch);
                    Debug.Log("- 18. triggerTime: " + triggerTime);
                    Debug.Log("- 19. triggerHealth: " + triggerHealth);
                    Debug.Log("- 20. triggerHits: " + triggerHits);
                    Debug.Log("- 21. triggerScore: " + triggerScore);
                    Debug.Log("- 22. reuseQuestion: " + reuseQuestion);
                    Debug.Log("- 23. spawnAmount: " + spawnAmount);
                    Debug.Log("- 24. valuePatch: " + valuePatch);
                    Debug.Log("- 25. valueTime: " + valueTime);
                    Debug.Log("- 26. valueHealth: " + valueHealth);
                    Debug.Log("- 27. valueHits: " + valueHits);
                    Debug.Log("- 28. valueScore: " + valueScore);

                    Debug.Log("-----QUESTION " + i + " [END]-----");
                }

                q.questions[i].theQuestion = theQuestion; //1
                q.questions[i].fontSize = fontSize; //2
                q.questions[i].isActive = isActive; //3
                q.questions[i].prePatch = prePatch; //4
                q.questions[i].questionType = questionType; //5
                q.questions[i].barQuestionAmount = barQuestionAmount; //6
                q.questions[i].barQuestionAnswers[0] = barQuestionAnswers1; //7
                q.questions[i].barQuestionAnswers[1] = barQuestionAnswers2; //8
                q.questions[i].barQuestionAnswers[2] = barQuestionAnswers3; //9
                q.questions[i].barQuestionAnswers[3] = barQuestionAnswers4; //10
                q.questions[i].barQuestionAnswers[4] = barQuestionAnswers5; //11
                q.questions[i].barQuestionAnswers[5] = barQuestionAnswers6; //12
                q.questions[i].barShowAllAnswers = barShowAllAnswers; //13
                q.questions[i].spectrumQuestionMin = spectrumQuestionMin; //14
                q.questions[i].spectrumQuestionMax = spectrumQuestionMax; //15
                q.questions[i].spectrumDecimalPlace = spectrumDecimalPlace; //16
                q.questions[i].triggerPatch = triggerPatch; //17
                q.questions[i].triggerTime = triggerTime; //18
                q.questions[i].triggerHealth = triggerHealth; //19
                q.questions[i].triggerHits = triggerHits; //20
                q.questions[i].triggerScore = triggerScore; //21
                q.questions[i].reuseQuestion = reuseQuestion; //22
                q.questions[i].spawnAmount = spawnAmount; //23
                q.questions[i].valuePatch = valuePatch; //24
                q.questions[i].valueTime = valueTime; //25
                q.questions[i].valueHealth = valueHealth; //26
                q.questions[i].valueHits = valueHits; //27
                q.questions[i].valueScore = valueScore; //28
            }
        }
    }

    void ExtractedPresets()
    {
        for (int i = 0; i < extractedPresets.Length; i++)
        {
            if(extractedPresets[i] == "NULL")
            {
                if (debugPresets)
                {
                    Debug.Log("-----PRESET " + i + " [START]-----");
                    Debug.Log("NULL");
                    Debug.Log("-----PRESET " + i + " [END]-----");
                }
            }
            else
            {
                string[] data = extractedPresets[i].Split('\'');

                if (debugPresets)
                {
                    Debug.Log("-----PRESET " + i + " [START]-----");
                    for (int j = 0; j < data.Length; j++)
                    {
                        Debug.Log("data[" + j + "]: " + data[j]);
                    }
                    Debug.Log("-----PRESET " + i + " [END]-----");
                }
                for (int j = 0; j < data.Length; j++)
                {
                    data[j] = data[j].Substring(data[j].IndexOf('~') + 1);
                    if (debugPresets)
                    {
                        if (j != data.Length - 1)
                        {
                            Debug.Log("CONCAT[" + i + "]: " + data[j]);
                        }
                        else
                        {
                            data[j] = data[j].Substring(0, data[j].Length - 1);
                            Debug.Log("CONCAT LAST[" + i + "]: " + data[j] + "|||");
                        }
                    }
                }

                string name = data[0];
                string seed = data[1];
                int iterations = int.Parse(data[2]);

                bool isEndless;
                if (data[3].ToLower() == "true") { isEndless = true; } else { isEndless = false; }

                float difficulty = float.Parse(data[4]);

                bool diffIsActive;
                if (data[5].ToLower() == "true") { diffIsActive = true; } else { diffIsActive = false; }

                bool usePercentage;
                if (data[6].ToLower() == "true") { usePercentage = true; } else { usePercentage = false; }

               
                if (debugPresets)
                {
                    Debug.Log("-----PRESET " + i + " [START]-----");

                    Debug.Log("- 1. name: " + name);
                    Debug.Log("- 2. seed: " + seed);
                    Debug.Log("- 3. iterations: " + iterations);
                    Debug.Log("- 4. isEndless: " + isEndless);
                    Debug.Log("- 5. difficulty: " + difficulty);
                    Debug.Log("- 6. diffIsActive: " + diffIsActive);
                    Debug.Log("- 7. usePercentage: " + usePercentage);

                    Debug.Log("-----PRESET " + i + " [END]-----");
                }

                env.presets[i].name = name; //1
                env.presets[i].seed = seed; //2
                env.presets[i].iterations = iterations; //3
                env.presets[i].isEndless = isEndless; //4
                env.presets[i].difficulty = difficulty; //5
                env.presets[i].diffIsActive = diffIsActive; //6
                env.presets[i].usePercentage = usePercentage; //7
            }
        }
    }

    void ExtractedGeneralSettings()
    {
        for (int i = 0; i < extractedGeneralSettings.Length; i++)
        {
            if (extractedGeneralSettings[i] == "NULL")
            {
                if (debugGeneralSettings)
                {
                    Debug.Log("-----GENERAL SETTINGS " + i + " [START]-----");
                    Debug.Log("NULL");
                    Debug.Log("-----GENERAL SETTINGS " + i + " [END]-----");
                }
            }
            else
            {
                string[] data = extractedGeneralSettings[i].Split('\'');

                if (debugGeneralSettings)
                {
                    Debug.Log("-----GENERAL SETTINGS " + i + " [START]-----");
                    for (int j = 0; j < data.Length; j++)
                    {
                        Debug.Log("data[" + j + "]: " + data[j]);
                    }
                    Debug.Log("-----GENERAL SETTINGS " + i + " [END]-----");
                }
                for (int j = 0; j < data.Length; j++)
                {
                    data[j] = data[j].Substring(data[j].IndexOf('~') + 1);
                    if (debugGeneralSettings)
                    {
                        if (j != data.Length - 1)
                        {
                            Debug.Log("CONCAT[" + i + "]: " + data[j]);
                        }
                        else
                        {
                            data[j] = data[j].Substring(0, data[j].Length - 1);
                            Debug.Log("CONCAT LAST[" + i + "]: " + data[j] + "|||");
                        }
                    }
                }

                int sHealth = int.Parse(data[0]);
                int sHits = int.Parse(data[1]);
                int sTime = int.Parse(data[2]);
                int tHealth = int.Parse(data[3]);
                int tHits = int.Parse(data[4]);
                int tTime = int.Parse(data[5]);

                bool showHealth;
                bool showHits;
                bool showTimer;
                bool timerCountdown;
                bool tHealthToggle;
                bool tHitsToggle;
                bool tTimeToggle;

                if (data[6].ToLower() == "true") { showHealth = true; } else { showHealth = false; }
                if (data[7].ToLower() == "true") { showHits = true; } else { showHits = false; }
                if (data[8].ToLower() == "true") { showTimer = true; } else { showTimer = false; }
                if (data[9].ToLower() == "true") { timerCountdown = true; } else { timerCountdown = false; }
                if (data[10].ToLower() == "true") { tHealthToggle = true; } else { tHealthToggle = false; }
                if (data[11].ToLower() == "true") { tHitsToggle = true; } else { tHitsToggle = false; }
                if (data[12].ToLower() == "true") { tTimeToggle = true; } else { tTimeToggle = false; }

                
                if (debugGeneralSettings)
                {
                    Debug.Log("-----GENERAL SETTINGS " + i + " [START]-----");

                    Debug.Log("- 1. sHealth: " + sHealth);
                    Debug.Log("- 2. sHits: " + sHits);
                    Debug.Log("- 3. sTime: " + sTime);
                    Debug.Log("- 4. tHealth: " + tHealth);
                    Debug.Log("- 5. tHits: " + tHits);
                    Debug.Log("- 6. tTime: " + tTime);
                    Debug.Log("- 7. showHealth: " + showHealth);
                    Debug.Log("- 8. showHits: " + showHits);
                    Debug.Log("- 9. showTimer: " + showTimer);
                    Debug.Log("- 10. timerCountdown: " + timerCountdown);
                    Debug.Log("- 11. tHealthToggle: " + tHealthToggle);
                    Debug.Log("- 12. tHitsToggle: " + tHitsToggle);
                    Debug.Log("- 13. tTimeToggle: " + tTimeToggle);

                    Debug.Log("-----GENERAL SETTINGS " + i + " [END]-----");
                }

                g.genPreset[i].sHealth = sHealth; //1
                g.genPreset[i].sHits = sHits; //2
                g.genPreset[i].sTime = sTime; //3
                g.genPreset[i].tHealth = tHealth; //4
                g.genPreset[i].tHits = tHits; //5
                g.genPreset[i].tTime = tTime; //6
                g.genPreset[i].showHealth = showHealth; //7
                g.genPreset[i].showHits = showHits; //8
                g.genPreset[i].showTimer = showTimer; //9
                g.genPreset[i].timerCountdown = timerCountdown; //10
                g.genPreset[i].tHealthToggle = tHealthToggle; //11
                g.genPreset[i].tHitsToggle = tHitsToggle; //12
                g.genPreset[i].tTimeToggle = tTimeToggle; //13
            }
        }
    }

    public void ResetValues()
    {
        //
    }
}
