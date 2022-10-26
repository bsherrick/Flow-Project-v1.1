using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowQuestionHandler : MonoBehaviour
{
    [HideInInspector] public FlowMain flow;

    [Tooltip("Display debugging messages?")] public bool showDebug;

    [Header("Settings (Editor Only):")]
    #region
    [Tooltip("The prefab for a question of type slot.")] public GameObject questionLineSlot;
    [Tooltip("The prefab for a question of type spectrum.")] public GameObject questionSpectrum;
    #endregion

    [Header("Questions (To be configured by Questions):")]
    #region
    [Tooltip("contains indexes of Questions (with trigger set to Health)")] public List<int> qtHealth;
    [Tooltip("contains indexes of Questions (with trigger set to Score)")] public List<int> qtScore;
    [Tooltip("contains indexes of Questions (with trigger set to Line)")] public List<int> qtLine;
    [Tooltip("contains indexes of Questions (with trigger set to Time)")] public List<int> qtTime;
    [Tooltip("TRUE if a question is in progress (traveling to player)")] public bool questionInProgress = false;
    [Tooltip("index of the current question (note: value remains until next question spawns)")] public int qtCurrent = 0;
    #endregion

    [Header("Status (Per game session):")]
    #region
    [Tooltip("Questions waiting to be spawned.")] public List<int> qtQueue;
    [Tooltip("Answered questions along with their answers (for data collection).")] public List<string> qtAnswers;
    #endregion

    float triggerTimeLastValue; //used so that we dont called TriggerTime() too many times
    int triggerTimeLastQuestion;  //used so that we dont called TriggerTime() too many times

    public void Initialize()
    {
        StopAllCoroutines();

        triggerTimeLastValue = 0;
        triggerTimeLastQuestion = 0;
        qtQueue.Clear();
        qtAnswers.Clear();
        qtHealth.Clear();
        qtScore.Clear();
        qtLine.Clear();
        qtTime.Clear();
        questionInProgress = false;
        qtCurrent = 0;
    }

    public void BeginSpawningQuestions()
    {
        InvokeRepeating("SpawnQuestionLoop", 0, 1);
    }

    void SpawnQuestionLoop()
    {
        if (!questionInProgress && qtQueue.Count > 0)
        {
            SpawnQuestion(qtQueue[0]);
            qtQueue.RemoveAt(0);
        }
    }

    public void AssignQuestionTriggers()
    {
        for (int i = 0; i < 10; i++)
        {
            if (flow.FlowWebConfig.questionSettings[i].t_healthOn && flow.FlowWebConfig.questionSettings[i].g_active)
            {
                if (showDebug)
                {
                    Debug.Log("AssignQuestionTriggers(): qtHealth.Add() - " + i);
                }
                qtHealth.Add(i);
            }
            if (flow.FlowWebConfig.questionSettings[i].t_scoreOn && flow.FlowWebConfig.questionSettings[i].g_active)
            {
                if (showDebug)
                {
                    Debug.Log("AssignQuestionTriggers(): qtScore.Add() - " + i);
                }
                qtScore.Add(i);
            }
            if (flow.FlowWebConfig.questionSettings[i].t_lineOn && flow.FlowWebConfig.questionSettings[i].g_active)
            {
                if (showDebug)
                {
                    Debug.Log("AssignQuestionTriggers(): qtLine.Add() - " + i);
                }
                qtLine.Add(i);
            }
            if (flow.FlowWebConfig.questionSettings[i].t_timeOn && flow.FlowWebConfig.questionSettings[i].g_active)
            {
                if (showDebug)
                {
                    Debug.Log("AssignQuestionTriggers(): qtTime.Add() - " + i);
                }
                qtTime.Add(i);
            }
        }
    }

    public void TriggerHealth(int value)
    {
        if (qtHealth.Count < 1 || flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_Rhythm) { return; }

        for (int i = 0; i < qtHealth.Count; i++)
        {
            if (flow.FlowWebConfig.questionSettings[qtHealth[i]].t_health == value)
            {
                if (showDebug)
                {
                    Debug.Log("TriggerHealth(): " + qtHealth[i] + ", value == " + value);
                }
                qtQueue.Add(qtHealth[i]);
            }
        }
    }
    public void TriggerScore(int value)
    {
        if (qtScore.Count < 1 || flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_Rhythm) { return; }

        for (int i = 0; i < qtScore.Count; i++)
        {
            if (flow.FlowWebConfig.questionSettings[qtScore[i]].t_score == value)
            {
                if (showDebug)
                {
                    Debug.Log("TriggerScore(): " + qtScore[i] + ", value == " + value);
                }
                qtQueue.Add(qtScore[i]);
            }
        }
    }
    public void TriggerLine(int value)
    {
        if (qtLine.Count < 1 || flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_Rhythm) { return; }

        for (int i = 0; i < qtLine.Count; i++)
        {
            if (flow.FlowWebConfig.questionSettings[qtLine[i]].t_line == value)
            {
                if (showDebug)
                {
                    Debug.Log("TriggerLine(): " + qtLine[i] + ", value == " + value);
                }
                qtQueue.Add(qtLine[i]);
            }
        }
    }
    public void TriggerTime(float value)
    {
        if (qtTime.Count < 1 || flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_Rhythm) { return; }

        for (int i = 0; i < qtTime.Count; i++)
        {
            if (flow.FlowWebConfig.questionSettings[qtTime[i]].t_timeTotal == value)
            {
                if (triggerTimeLastValue == value && triggerTimeLastQuestion == qtTime[i]) //we have a repeat
                {
                    //do nothing
                }
                else
                {
                    triggerTimeLastValue = value;
                    triggerTimeLastQuestion = qtTime[i];

                    if (showDebug)
                    {
                        Debug.Log("TriggerTime(): " + qtTime[i] + ", value == " + value);
                    }

                    qtQueue.Add(qtTime[i]);
                }   
            }
        }
    }

    void SpawnQuestion(int i)
    {
        //Debug.Log("Spawn Question");
        questionInProgress = true;
        if (!QuestionIsValid(i))
        {
            if (showDebug)
            {
                Debug.Log("SpawnQuestion(): [NOT VALID] question number - " + i);
            }
            questionInProgress = false;
            return;
        }
        if (showDebug)
        {
            Debug.Log("SpawnQuestion(): question number - " + i);
        }
        qtCurrent = i;
        StartCoroutine(QuestionInProgress());
    }

    bool QuestionIsValid(int i)
    {
        if (flow.FlowWebConfig.questionSettings[i].g_repeatAmount < 0 && !flow.FlowWebConfig.questionSettings[i].g_neverExpire)
        {
            return false;
        }
        return true;
    }

    IEnumerator QuestionInProgress()
    {
        yield return new WaitForSeconds(flow.FlowWebConfig.questionSettings[qtCurrent].e_delayStart);

        if (flow.FlowWebConfig.questionSettings[qtCurrent].qt_type == 1) //line slot
        {
            flow.FlowWebConfig.questionSettings[qtCurrent].g_repeatAmount--;
            #region timeQuestion add/subtract time
            if (flow.FlowWebConfig.questionSettings[qtCurrent].t_timeOn)
            {
                if (flow.FlowGameConfig.timerCountdown)
                {
                    flow.FlowWebConfig.questionSettings[qtCurrent].t_timeTotal -= flow.FlowWebConfig.questionSettings[qtCurrent].t_time;
                }
                else
                {
                    flow.FlowWebConfig.questionSettings[qtCurrent].t_timeTotal += flow.FlowWebConfig.questionSettings[qtCurrent].t_time;
                }
            }
            #endregion
            flow.FlowLineGenerator.ChangeLineSpeed(flow.FlowWebConfig.questionSettings[qtCurrent].e_lineSpeedSetTo);

            #region spawn Question
            GameObject spawnedQ = Instantiate(questionLineSlot, flow.FlowLineGenerator.lineSpawnPoint, Quaternion.identity);
            flow.FlowLineGenerator.linesInGame.Add(spawnedQ);

            bool[] on = new bool[5];
            string[] qa = new string[5];

            on[0] = flow.FlowWebConfig.questionSettings[qtCurrent].qt_ls1On;
            on[1] = flow.FlowWebConfig.questionSettings[qtCurrent].qt_ls2On;
            on[2] = flow.FlowWebConfig.questionSettings[qtCurrent].qt_ls3On;
            on[3] = flow.FlowWebConfig.questionSettings[qtCurrent].qt_ls4On;
            on[4] = flow.FlowWebConfig.questionSettings[qtCurrent].qt_ls5On;
            qa[0] = flow.FlowWebConfig.questionSettings[qtCurrent].qt_ls1;
            qa[1] = flow.FlowWebConfig.questionSettings[qtCurrent].qt_ls2;
            qa[2] = flow.FlowWebConfig.questionSettings[qtCurrent].qt_ls3;
            qa[3] = flow.FlowWebConfig.questionSettings[qtCurrent].qt_ls4;
            qa[4] = flow.FlowWebConfig.questionSettings[qtCurrent].qt_ls5;

            QuestionLine ql = spawnedQ.GetComponent<QuestionLine>();
            ql.QuestionInit(flow.FlowWebConfig.questionSettings[qtCurrent].rp_healthAdd,
                flow.FlowWebConfig.questionSettings[qtCurrent].rp_scoreAdd,
                on, qa, flow.FlowWebConfig.questionSettings[qtCurrent].g_question, flow);

            #endregion
        }
        else if (flow.FlowWebConfig.questionSettings[qtCurrent].qt_type == 2) //spectrum
        {
            //TO DO
        }
    }

    public void QuestionAnswered()
    {
        StartCoroutine(QuestionEnding());
    }

    IEnumerator QuestionEnding()
    {
        yield return new WaitForSeconds(flow.FlowWebConfig.questionSettings[qtCurrent].e_delayEnd);
        QuestionStop();
    }

    void QuestionStop()
    {
        questionInProgress = false;
        qtCurrent = 0;

        if (flow.FlowGameConfig.gameMode == -1)
        {
            flow.FlowLineGenerator.ChangeLineSpeedAlgorithm(flow.FlowLineGenerator.dynamicLineSpeed);
            flow.FlowLineGenerator.SpawnLineAlgorithm();
        }
        else { 
            flow.FlowLineGenerator.ChangeLineSpeed(flow.FlowLineGenerator.me_speed);
            if (flow.FlowLineGenerator.index > 0 || flow.FlowLineGenerator.lineEndless || flow.FlowLineGenerator.lineMapRepeat > 0)
            {
                flow.FlowLineGenerator.SpawnLine();
            }
        }
    }
}
