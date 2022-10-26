using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class QuestionEditor : MonoBehaviour
{
    //General
    public GameFlowFramework_ScriptReferencer referencer;
    GameFlowFramework_Questions que;
    public int currentIndex;

    //Question Details
    public Text questionNo;
    public InputField question;
    public Slider font;
    public Text fontText;
    public Dropdown prePatch;
    public Dropdown type;
    public Toggle active;
    public Toggle reuse;
    public InputField spawnAmount;
    public Toggle triggerPatch;
    public Toggle triggerTime;
    public Toggle triggerHealth;
    public Toggle triggerHits;
    public InputField valuePatch;
    public InputField valueTime;
    public InputField valueHealth;
    public InputField valueHits;

    //Bar
    public GameObject[] allBarObjects;
    public Slider barAmount;
    public InputField bar1;
    public InputField bar2;
    public InputField bar3;
    public InputField bar4;
    public InputField bar5;
    public InputField bar6;
    public Text barAmountText;
    public Toggle barShowAll;

    //Spectrum
    public GameObject[] allSpectrumObjects;
    public InputField spectrumMin;
    public InputField spectrumMax;
    public Slider spectrumDecimal;
    public Text spectrumDecimalText;

    //Other
    public Text uiIndexText;
    public InternetResultTextChange download;
    public InternetResultTextChange upload;

    void Start()
    {
        currentIndex = 0;
    }

    void Update()
    {
        
    }

    public void Init()
    {
        currentIndex = 0;
        que = referencer.GameFlowFramework_Questions;
        SetQuestions(currentIndex);
    }

    public void Next(bool b)
    {
        if (b)
        {
            //next
            if (currentIndex >= 9){currentIndex = 0;}else{currentIndex += 1;}
        }
        else
        {
            //prev
            if (currentIndex <= 0){currentIndex = 9;}else{currentIndex -= 1;}
        }

        SetQuestions(currentIndex);
    }

    public void ToggleTrigger(int t)
    {
        que.questions[currentIndex].triggerPatch = false;
        que.questions[currentIndex].triggerTime = false;
        que.questions[currentIndex].triggerHealth = false;
        que.questions[currentIndex].triggerHits = false;
        que.questions[currentIndex].triggerScore = false;

        switch (t)
        {
            case 0://patch
                que.questions[currentIndex].triggerPatch = true;
                break;
            case 1://time
                que.questions[currentIndex].triggerTime = true;
                break;
            case 2://health
                que.questions[currentIndex].triggerHealth = true;
                break;
            case 3://hits
                que.questions[currentIndex].triggerHits = true;
                break;
            case 4://score
                que.questions[currentIndex].triggerScore = true;
                break;
        }

        SetQuestions(currentIndex);
    }

    public void ChangedValue()
    {
        que.questions[currentIndex].theQuestion = question.text;
        que.questions[currentIndex].fontSize = int.Parse(font.value.ToString());
        que.questions[currentIndex].prePatch = prePatch.value;
        que.questions[currentIndex].questionType = type.value;
        que.questions[currentIndex].isActive = active.isOn;
        que.questions[currentIndex].reuseQuestion = reuse.isOn;
        que.questions[currentIndex].spawnAmount = int.Parse(spawnAmount.text.ToString());
        //que.questions[currentIndex].triggerPatch = triggerPatch.isOn;
        //que.questions[currentIndex].triggerTime = triggerTime.isOn;
        //que.questions[currentIndex].triggerHealth = triggerHealth.isOn;
        //que.questions[currentIndex].triggerHits = triggerHits.isOn;
        que.questions[currentIndex].valuePatch = int.Parse(valuePatch.text.ToString());
        que.questions[currentIndex].valueTime = int.Parse(valueTime.text.ToString());
        que.questions[currentIndex].valueHealth = int.Parse(valueHealth.text.ToString());
        que.questions[currentIndex].valueHits = int.Parse(valueHits.text.ToString());

        que.questions[currentIndex].barQuestionAmount = int.Parse(barAmount.value.ToString());
        que.questions[currentIndex].barQuestionAnswers[0] = bar1.text;
        que.questions[currentIndex].barQuestionAnswers[1] = bar2.text;
        que.questions[currentIndex].barQuestionAnswers[2] = bar3.text;
        que.questions[currentIndex].barQuestionAnswers[3] = bar4.text;  
        que.questions[currentIndex].barQuestionAnswers[4] = bar5.text;
        que.questions[currentIndex].barQuestionAnswers[5] = bar6.text;
        que.questions[currentIndex].barShowAllAnswers = barShowAll.isOn;

        que.questions[currentIndex].spectrumQuestionMin = float.Parse(spectrumMin.text.ToString());
        que.questions[currentIndex].spectrumQuestionMax = float.Parse(spectrumMax.text.ToString());
        que.questions[currentIndex].spectrumDecimalPlace = int.Parse(spectrumDecimal.value.ToString());

        SetQuestions(currentIndex);
    }

    public void SetQuestions(int index)
    {
        uiIndexText.text = (index + 1) + " of 10";
        questionNo.text = "QUESTION NO. " + (index + 1);
        question.SetTextWithoutNotify(que.questions[index].theQuestion);
        font.SetValueWithoutNotify(que.questions[index].fontSize);
        fontText.text = font.value.ToString();
        prePatch.SetValueWithoutNotify(que.questions[index].prePatch);
        type.SetValueWithoutNotify(que.questions[index].questionType);
        active.SetIsOnWithoutNotify(que.questions[index].isActive);
        reuse.SetIsOnWithoutNotify(que.questions[index].reuseQuestion);
        spawnAmount.SetTextWithoutNotify(que.questions[index].spawnAmount.ToString());
        triggerPatch.SetIsOnWithoutNotify(que.questions[index].triggerPatch);
        triggerTime.SetIsOnWithoutNotify(que.questions[index].triggerTime); 
        triggerHealth.SetIsOnWithoutNotify(que.questions[index].triggerHealth);
        triggerHits.SetIsOnWithoutNotify(que.questions[index].triggerHits);
        valuePatch.SetTextWithoutNotify(que.questions[index].valuePatch.ToString());
        valueTime.SetTextWithoutNotify(que.questions[index].valueTime.ToString());
        valueHealth.SetTextWithoutNotify(que.questions[index].valueHealth.ToString());
        valueHits.SetTextWithoutNotify(que.questions[index].valueHits.ToString());

        barAmount.SetValueWithoutNotify(que.questions[index].barQuestionAmount);
        bar1.SetTextWithoutNotify(que.questions[index].barQuestionAnswers[0]); 
        bar2.SetTextWithoutNotify(que.questions[index].barQuestionAnswers[1]); 
        bar3.SetTextWithoutNotify(que.questions[index].barQuestionAnswers[2]); 
        bar4.SetTextWithoutNotify(que.questions[index].barQuestionAnswers[3]); 
        bar5.SetTextWithoutNotify(que.questions[index].barQuestionAnswers[4]); 
        bar6.SetTextWithoutNotify(que.questions[index].barQuestionAnswers[5]); 
        barAmountText.text = barAmount.value.ToString();
        barShowAll.SetIsOnWithoutNotify(que.questions[index].barShowAllAnswers);

        spectrumMin.SetTextWithoutNotify(que.questions[index].spectrumQuestionMin.ToString());
        spectrumMax.SetTextWithoutNotify(que.questions[index].spectrumQuestionMax.ToString());
        spectrumDecimal.SetValueWithoutNotify(que.questions[index].spectrumDecimalPlace);
        spectrumDecimalText.text = spectrumDecimal.value.ToString();

        if (que.questions[index].questionType == 0) //Bar Question
        {
            for (int i = 0; i < allBarObjects.Length; i++)
            {
                allBarObjects[i].SetActive(false);
            }
            allBarObjects[0].SetActive(true);
            allBarObjects[7].SetActive(true);
            allBarObjects[8].SetActive(true);
            if (que.questions[index].barQuestionAmount >= 1) { allBarObjects[1].SetActive(true); }
            if (que.questions[index].barQuestionAmount >= 2) { allBarObjects[2].SetActive(true); }
            if (que.questions[index].barQuestionAmount >= 3) { allBarObjects[3].SetActive(true); }
            if (que.questions[index].barQuestionAmount >= 4) { allBarObjects[4].SetActive(true); }
            if (que.questions[index].barQuestionAmount >= 5) { allBarObjects[5].SetActive(true); }
            if (que.questions[index].barQuestionAmount >= 6) { allBarObjects[6].SetActive(true); }
            for (int i = 0; i < allSpectrumObjects.Length; i++)
            {
                allSpectrumObjects[i].SetActive(false);
            }
        }
        else if (que.questions[index].questionType == 1) //Spectrum Question
        {
            for (int i = 0; i < allBarObjects.Length; i++)
            {
                allBarObjects[i].SetActive(false);
            }
            for (int i = 0; i < allSpectrumObjects.Length; i++)
            {
                allSpectrumObjects[i].SetActive(true);
            }
        }

        CheckActive(index, que.questions[index].isActive);
    }

    void CheckActive(int index, bool active)
    {
        question.interactable = active;
        font.interactable = active;
        prePatch.interactable = active;
        type.interactable = active;
        reuse.interactable = active;
        spawnAmount.interactable = active;
        triggerPatch.interactable = active;
        triggerTime.interactable = active;
        triggerHealth.interactable = active;
        triggerHits.interactable = active;
        valuePatch.interactable = active;
        valueTime.interactable = active;
        valueHealth.interactable = active;
        valueHits.interactable = active;

        barAmount.interactable = active;
        bar1.interactable = active;
        bar2.interactable = active;
        bar3.interactable = active;
        bar4.interactable = active;
        bar5.interactable = active;
        bar6.interactable = active;
        barShowAll.interactable = active;

        spectrumMin.interactable = active;
        spectrumMax.interactable = active;
        spectrumDecimal.interactable = active;
    }


    public void CheckValues()
    {
        //Question Details
    //public Text questionNo;
    //public InputField question;
    //public Slider font;
    //public Dropdown prePatch;
    //public Dropdown type;
    //public Toggle active;
    //public Toggle reuse;
    //public InputField spawnAmount;
    //public Toggle triggerPatch;
    //public Toggle triggerTime;
   // public Toggle triggerHealth;
   // public Toggle triggerHits;
    //public InputField valuePatch;
   // public InputField valueTime;
   // public InputField valueHealth;
   // public InputField valueHits;

    //Bar
   // public GameObject[] allBarObjects;
    //public Slider barAmount;
    //public InputField bar1;
   // public InputField bar2;
   // public InputField bar3;
   // public InputField bar4;
   // public InputField bar5;
   // public InputField bar6;

    //Spectrum
  //  public GameObject allSpectrumObjects;
   // public InputField spectrumMin;
   // public InputField spectrumMax;
   // public Slider spectrumDecimal;

    //Other
   // public Text index;
   // public InternetResultTextChange download;
    //public InternetResultTextChange upload;
    }
}
