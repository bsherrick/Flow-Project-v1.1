using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveAnswerParent : MonoBehaviour
{

    GameObject[] answers;
    Unity_Purdue_Difficulty difficult;
    public bool isYesNoQuestion;
    public string question;

    Text uiQuestion;

    void Start()
    {
        difficult = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Unity_Purdue_Difficulty>();
        uiQuestion = transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        uiQuestion.text = question;
        if (isYesNoQuestion)
        {
            answers = new GameObject[2];
            answers[0] = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject; //Yes
            answers[1] = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject; //No
        }
        else
        {
            answers = new GameObject[5];
            answers[0] = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject; //1
            answers[1] = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject; //2
            answers[2] = transform.GetChild(0).gameObject.transform.GetChild(2).gameObject; //3
            answers[3] = transform.GetChild(0).gameObject.transform.GetChild(3).gameObject; //4
            answers[4] = transform.GetChild(0).gameObject.transform.GetChild(4).gameObject; //5
        }
    }

    public void answered(string answer)
    {
        if (isYesNoQuestion)
        {
            answers[0].SetActive(false); //Yes
            answers[1].SetActive(false); //No
        }
        else
        {
            answers[0].SetActive(false); //1
            answers[1].SetActive(false); //2
            answers[2].SetActive(false); //3
            answers[3].SetActive(false); //4
            answers[4].SetActive(false); //5
        }

        difficult.endOfPatch();
        Debug.Log(question + ": " + answer);
    }
}
