using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionLine : MonoBehaviour
{

    public float speed = 2;
    public string question;
    public QuestionAnswer[] answers;
    Rigidbody rigid;
    Vector3 movement;
    FlowMain flow;
    int addHealth;
    int addScore;
    public Text textQuestion;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void InitReferences()
    {
        answers = new QuestionAnswer[5];
        answers[0] = transform.GetChild(0).gameObject.GetComponent<QuestionAnswer>();
        answers[1] = transform.GetChild(1).gameObject.GetComponent<QuestionAnswer>();
        answers[2] = transform.GetChild(2).gameObject.GetComponent<QuestionAnswer>();
        answers[3] = transform.GetChild(3).gameObject.GetComponent<QuestionAnswer>();
        answers[4] = transform.GetChild(4).gameObject.GetComponent<QuestionAnswer>();
        textQuestion = transform.GetChild(5).gameObject.GetComponentInChildren<Text>(); 
    }

    void FixedUpdate()
    {
        if (flow != null)
        {
            movement.Set(-1, 0f, 0f);
            movement = movement.normalized * flow.FlowLineGenerator.lineSpeed * Time.deltaTime;
            rigid.MovePosition(rigid.transform.position + movement);
        }
    }

    public void Answered(string qAnswer)
    {
        flow.FlowQuestionHandler.qtAnswers.Add("QUESTION[" + question + "]=ANSWER[" + qAnswer + "]");
        flow.FlowGameConfig.PlayerHitQuestion(addHealth, addScore);
        flow.FlowLineGenerator.linesInGame.Remove(gameObject);
        Destroy(gameObject);
    }

    public void QuestionInit(int addHealth, int addScore, bool[] on, string[] qAnswers, string question, FlowMain f)
    {
        flow = f;
        InitReferences();

        this.addHealth = addHealth;
        this.addScore = addScore;
        this.question = question;
        textQuestion.text = this.question;
        
        for (int i = 0; i < 5; i++)
        {
            if (on[i])
            {
                answers[i].gameObject.SetActive(true);
                answers[i].parent = this;
                answers[i].answer = qAnswers[i];
                answers[i].active = true;

            }
        }
    }
}
