using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionParent : MonoBehaviour
{
    public string question;
    public int fontSize;

    Text uiQuestion;
    Text uiAnswer;

    GameFlowFramework_ScriptReferencer reference;
    public CanvasScript canvas;

    //For Bar Questions
    public bool showAllAnswers;

    //For Spectrum Questions
    public float spectrumMin;
    public float spectrumMax;
    public int spectrumRounded;

    void Start()
    {
        //uiQuestion = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        //uiQuestion.text = question;
        //uiQuestion.fontSize = fontSize;
        uiAnswer = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        uiAnswer.text = "";
        uiAnswer.fontSize = fontSize;
        reference = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameFlowFramework_ScriptReferencer>();
        canvas = reference.CanvasScript;

    }

    void Update()
    {

    }

    /// <summary>
    /// For when the user has picked an answer to the question.
    /// </summary>
    /// <param name="answer">The answer that the player has picked.</param>
    /// <param name="questionType">0 for Bar Question, 1 for Spectrum Question.</param>
    public void Answered(string answer, int questionType)
    {
        string result = "QUESTION[" + question + ": " + answer + "]";
        switch (questionType)
        {
            case 0: //Bar Question
                Debug.Log("QUESTION ANSWERED (bar): " + question + ": " + answer);
                break;
            case 1: //Spectrum Question
                Debug.Log("QUESTION ANSWERED (spectrum): " + question + ": " + answer);
                break;
            default:
                Debug.Log("ERROR: QuestionParent answered() invalid questionType!");
                break;
        }
        reference.GameFlowFramework_WinLoseCondition.questionsAnswered.Add(result);

        //Destroy(gameObject);
        //canvas.SetGameSpeed(1); //revert game speed to normal
        canvas.question.gameObject.SetActive(false); //hide the question (because it has been answered already)
        //canvas.answer.gameObject.SetActive(false); //hide the answer (because it has been answered already)
        transform.GetChild(1).gameObject.SetActive(false); //destroy the answer options
        canvas.referencer.GameFlowFramework_PlayerCharacter.Slowed(false);
        reference.effect_laserShoot.Play();
    }

    /// <summary>
    /// Displays to the user what is currently selected (by updating the UI)
    /// </summary>
    /// <param name="currentAnswer"></param>
    public void ChangeCurrentAnswer(string currentAnswer)
    {
        uiAnswer.text = currentAnswer;
        //canvas.answer.text = currentAnswer;
        //canvas.answer.gameObject.SetActive(true);
    }

    public void QuestionBar_Init(int bars, string[] barAnswers, bool show)
    {
        showAllAnswers = show;
        GameObject barGroup = transform.GetChild(1).gameObject.transform.GetChild(bars - 1).gameObject;
        barGroup.SetActive(true);
        for (int i = 0; i < bars; i++)
        {
            QuestionChild child = barGroup.transform.GetChild(i).GetComponent<QuestionChild>();
            child.barAnswer = barAnswers[i];
        }
    }

    public void QuestionSpectrum_Init()
    {
        Text min = transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Text>();
        Text max = transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Text>();
        min.text = spectrumMin + "";
        max.text = spectrumMax + "";
    }

    public void ShowQuestion()
    {
        //display the question on the screen

        canvas.question.text = question;
        canvas.question.gameObject.SetActive(true);
    }
}
