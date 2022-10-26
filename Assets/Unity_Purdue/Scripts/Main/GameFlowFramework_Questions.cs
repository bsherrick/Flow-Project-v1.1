using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowFramework_Questions : MonoBehaviour
{
    public bool enableDebugLog = false;

    [Header("In-game Question Settings:")]
    [Header("__________________________________________________________________")]
    [Tooltip("The details of each question")]
    public Question[] questions;
    [Tooltip("If TRUE, web questions will be used instead of regular questions.")]
    public bool useQuestionWeb;

    [Header("Patch Prefabs (to be spawned in-game):")]
    [Header("__________________________________________________________________")]
    [Tooltip("The patch that collects answers using bars.")]
    public GameObject questionBars;
    [Tooltip("The patch that collects answers from a spectrum.")]
    public GameObject questionSpectrum;
    [Tooltip("The patch that comes before a question. Funnel type.")]
    public GameObject prePatchFunnel;
    [Tooltip("The patch that comes before a question. Force move type.")]
    public GameObject prePatchForceMove;

    GameFlowFramework_ScriptReferencer referencer;
    GameFlowFramework_Environment env;

    List<Question> questionTimed;
    List<Question> questionPatch;
    List<Question> questionHealth;
    List<Question> questionHits;
    List<Question> questionScore;

    List<GameObject> specials;

    int currentTime; //time elapsed in seconds

    void Start()
    {
        currentTime = 0;
        referencer = GetComponent<GameFlowFramework_ScriptReferencer>();
        env = referencer.GetComponent<GameFlowFramework_Environment>();
        specials = new List<GameObject>();
        questions = new Question[10];
        for (int i = 0; i < questions.Length; i++)
        {
            questions[i] = new Question();
            questions[i].barQuestionAnswers = new string[6];
            for (int j = 0; j < questions[i].barQuestionAnswers.Length; j++)
            {
                questions[i].barQuestionAnswers[j] = "[enter your answer here]";
            }
        }
        referencer.QuestionEditor.Init();
    }

    void Update()
    {
        /*
        if (!referencer.CanvasScript.gameHasStarted || referencer.CanvasScript.gameIsPaused)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1 was pressed");
            PatchShift();
        }
        */
    }

    public void Init()
    {
        questionTimed = new List<Question>();
        questionPatch = new List<Question>();
        questionHealth = new List<Question>();
        questionHits = new List<Question>();
        questionScore = new List<Question>();

        DistributeQuestionsByTrigger(); //sort questions out
        questionTimed.Sort(ListTimeSort); //Sort the timed questions (for efficiency)
        StartCoroutine(TriggerTimeCheckPerSecond());
    }

    public void TriggerTimeCheck()
    {
        for (int i = 0; i < questionTimed.Count; i++)
        {
            if ((questionTimed[i].realTime == currentTime)
                &&
                ((questionTimed[i].spawnAmount > 0) || (questionTimed[i].reuseQuestion)))
            {
                questionTimed[i].realTime += questionTimed[i].valueTime;
                SpawnQuestion(questionTimed[i]);
            }
        }
    }

    public void TriggerPatchCheck(int patchNumber)
    {
        for (int i = 0; i < questionPatch.Count; i++)
        {
            if ((questionPatch[i].valuePatch == patchNumber) && (questionPatch[i].spawnAmount > 0))
            {
                SpawnQuestion(questionPatch[i]);
            }
        }
    }

    public void TriggerHealthCheck(int health)
    {
        for (int i = 0; i < questionHealth.Count; i++)
        {
            if ((questionHealth[i].valueHealth == health)
                &&
                ((questionHealth[i].spawnAmount > 0) || (questionHealth[i].reuseQuestion)))
            {
                SpawnQuestion(questionHealth[i]);
            }
        }
    }

    public void TriggerHitsCheck(int hits)
    {
        for (int i = 0; i < questionHits.Count; i++)
        {
            if ((questionHits[i].valueHits == hits)
                &&
                ((questionHits[i].spawnAmount > 0) || (questionHits[i].reuseQuestion)))
            {
                SpawnQuestion(questionHits[i]);
            }
        }
    }

    public void TriggerScoreCheck(int score)
    {
        for (int i = 0; i < questionScore.Count; i++)
        {
            if ((questionScore[i].valueScore == score)
                &&
                ((questionScore[i].spawnAmount > 0) || (questionScore[i].reuseQuestion)))
            {
                SpawnQuestion(questionScore[i]);
            }
        }
    }

    IEnumerator TriggerTimeCheckPerSecond()
    {
        while (true)
        {
            TriggerTimeCheck();
            yield return new WaitForSeconds(1);
            currentTime++;
        }
    }

    void SpawnQuestion(Question q)
    {
        //Spawn a question

        GameObject p;
        if (q.prePatch == 1)
        {
            p = Instantiate(prePatchFunnel, env.currentSpawnLocation, Quaternion.identity);
            env.NewFlow(p, 2);
            specials.Add(p);
        }
        else if (q.prePatch == 2)
        {
            p = Instantiate(prePatchForceMove, env.currentSpawnLocation, Quaternion.identity);
            env.NewFlow(p, 2);
            specials.Add(p);
        }
        else
        {
            //No Pre Patch
        }

        GameObject spawnedQuestionPatch;
        QuestionParent parent;
        if (q.questionType == 0)
        { 
            //Spawn Bar Question
            spawnedQuestionPatch = Instantiate(questionBars, env.currentSpawnLocation, Quaternion.identity);
            env.NewFlow(spawnedQuestionPatch, 3);
            specials.Add(spawnedQuestionPatch);

            parent = spawnedQuestionPatch.GetComponent<QuestionParent>();
            parent.question = q.theQuestion;
            parent.fontSize = q.fontSize;
            string[] barAnswers = new string[6];
            for (int i = 0; i < q.barQuestionAnswers.Length; i++) //send the bar answers to children
            {
                barAnswers[i] = q.barQuestionAnswers[i];
            }
            parent.QuestionBar_Init(q.barQuestionAmount, barAnswers, q.barShowAllAnswers);
        }
        else if (q.questionType == 1)
        {
            //Spawn Spectrum Question
            spawnedQuestionPatch = Instantiate(questionSpectrum, env.currentSpawnLocation, Quaternion.identity);
            env.NewFlow(spawnedQuestionPatch, 3);
            specials.Add(spawnedQuestionPatch);

            parent = spawnedQuestionPatch.GetComponent<QuestionParent>();
            parent.question = q.theQuestion;
            parent.fontSize = q.fontSize;
            parent.spectrumMin = q.spectrumQuestionMin;
            parent.spectrumMax = q.spectrumQuestionMax;
            parent.spectrumRounded = q.spectrumDecimalPlace;
            parent.QuestionSpectrum_Init();
        }
        else
        {
            Debug.Log("SpawnQuestion(): ERROR IDENTIFYING QUESTION TYPE WHILE SPAWNING QUESTION");
            return;
        }

        q.spawnAmount--;

        if (enableDebugLog)
        {
            referencer.CanvasScript.PrintCurrentStats(q.theQuestion + ", S: " + q.spawnAmount + ", R: " + q.reuseQuestion);
        }
    }

    void PatchShift(Question q)
    {
        int index = env.flowObjects_current;
        Vector3 shift;
        int amount = env.patchLength * 3;
        if (q.prePatch != 0) { amount += env.patchLength; }

        if (enableDebugLog)
        {
            Debug.Log("PatchShift(): current player index is " + index);
            Debug.Log("First patch to shift is " + env.flowObjects[index + 1].patch.name);
        }

        for (int i = index + 1; i < env.flowObjects.Count; i++)
        {
            shift = env.flowObjects[i].patch.transform.position;
            shift.x += amount;
            env.flowObjects[i].patch.transform.position = new Vector3(shift.x, shift.y, shift.z);
        }

        Vector3 location = env.flowObjects[index].patch.transform.position;
        if (enableDebugLog)
        {
            Debug.Log("PatchShift(): current player patch location is " + location);
        }

        location.x += env.patchLength;
        GameObject a = Instantiate(env.genesisPatch, location, Quaternion.identity);
        env.NewFlow(a, 0);
        specials.Add(a);

        location.x += env.patchLength;
        GameObject b = Instantiate(env.genesisPatch, location, Quaternion.identity);
        env.NewFlow(b, 0);
        specials.Add(b);

        location.x += env.patchLength;
        SpawnQuestion(q);
    }

    public void OverLapTest()
    {
        for (int i = 0; i < specials.Count; i++)
        {
            for (int j = 0; j < env.flowObjects.Count; j++)
            {
                if ((specials[i].transform.position == env.flowObjects[j].patch.transform.position) &&
                    (specials[i] != env.flowObjects[j].patch))
                {
                    Debug.Log("OVERLAP at " + specials[i].transform.position);
                    Debug.Log("- specials[" + i + "]: " + specials[i].name);
                    Debug.Log("- flowObjects[" + j + "]: " + env.flowObjects[j].patch.name);
                }
            }
        }
    }
    
    void DistributeQuestionsByTrigger()
    {
        for (int i = 0; i < questions.Length; i++) //iterate through all the questions
        {
            if (questions[i].isActive)
            {
                if (questions[i].triggerPatch) { questionPatch.Add(questions[i]); }
                else if (questions[i].triggerTime)
                {
                    questions[i].realTime = questions[i].valueTime;
                    questionTimed.Add(questions[i]);
                }
                else if ( questions[i].triggerHealth) { questionHealth.Add(questions[i]); }
                else if (questions[i].triggerHits) { questionHits.Add(questions[i]); }
                else if (questions[i].triggerScore) { questionScore.Add(questions[i]); }
            }
            
            /*
            switch (questions[i].spawnTrigger)
            {
                case 0: //patch
                    questionPatch.Add(questions[i]);
                    break;
                case 1: //time
                    questions[i].realTime = questions[i].triggerTime;
                    questionTimed.Add(questions[i]);
                    break;
                case 2: //health
                    questionHealth.Add(questions[i]);
                    break;
                case 3: //hits
                    questionHits.Add(questions[i]);
                    break;
                case 4: //score
                    questionScore.Add(questions[i]);
                    break;
                default:
                    Debug.Log("DistributeQuestionsByTrigger(): ERROR UNKNOWN QUESTION TYPE at Index: " + i);
                    break;
            }
            */
            
        }
    }
    
    int ListTimeSort(Question a, Question b)
    {
        return a.valueTime.CompareTo(b.valueTime);
    }

    public void CheckQuestions() //Check for any game-breaking values
    {
        for (int i = 0; i < questions.Length; i++)
        {
            if (questions[i].fontSize < 1 || questions[i].fontSize > 40)
            {
                questions[i].fontSize = 30;
            }
            if (questions[i].barQuestionAmount < 1 || questions[i].barQuestionAmount > 6)
            {
                questions[i].barQuestionAmount = 2;
            }
            if (questions[i].spectrumQuestionMin >= questions[i].spectrumQuestionMax)
            {
                questions[i].spectrumQuestionMin = 0;
                questions[i].spectrumQuestionMax = 100;
            }
            if (questions[i].spectrumDecimalPlace < 0 || questions[i].spectrumDecimalPlace > 10)
            {
                questions[i].spectrumDecimalPlace = 0;
            }
            if (questions[i].barQuestionAnswers.Length != questions[i].barQuestionAmount)
            {
                questions[i].barQuestionAnswers = new string[questions[i].barQuestionAmount];
            }

            if (questions[i].valuePatch < 0)
            {
                questions[i].valuePatch = 0;
            }

            if (questions[i].valueTime < 0)
            {
                questions[i].valueTime = 0;
            }

            if (questions[i].spawnAmount < 1)
            {
                questions[i].spawnAmount = 1;
            }
        }
    }

    public void ResetValues()
    {
        questions = new Question[0];
    }

}

//[System.Serializable]
public class Question
{
    [Header("UI:")]
    [Header("__________________________________________________________________")]
    [Tooltip("The question to be asked.")]
    public string theQuestion = "[enter your question here]";
    [Tooltip("The font size of the question. Default: 30")]
    [Range(1, 40)]
    public int fontSize = 30;
    [Tooltip("Is this question active? Default: False")]
    public bool isActive = false;

    [Header("Pre-Question Patch:")]
    [Header("__________________________________________________________________")]
    [Tooltip("0: Don't spawn Pre-Question Patch.\n"
            + "1: Funnels the player toward the center.\n"
            + "2: Forces the player to move to the center.")]
    [Range(0, 2)]
    public int prePatch = 0;

    [Header("Question Type:")]
    [Header("__________________________________________________________________")]
    [Tooltip("0: Bar Questions\n"
            + "1: Spectrum Questions")]
    [Range(0, 1)]
    public int questionType = 0;

    [Header("Bar Question Settings:")]
    [Header("__________________________________________________________________")]
    [Tooltip("Amount of answers available for the player to choose from. Default: 2")]
    [Range(1, 6)]
    public int barQuestionAmount = 1;
    public string[] barQuestionAnswers;
    public bool barShowAllAnswers = true;

    [Header("Spectrum Question Settings:")]
    [Header("__________________________________________________________________")]
    [Tooltip("Minimum value on the spectrum question. Default: 0")]
    public float spectrumQuestionMin = 0;
    [Tooltip("Maximum value on the spectrum question. Default: 100")]
    public float spectrumQuestionMax = 100;
    [Tooltip("The player will receive results rounded to how many decimal places?. Default: 0")]
    [Range(0, 10)]
    public int spectrumDecimalPlace = 0;

    [Header("Question Spawn Trigger:")]
    [Header("__________________________________________________________________")]
    [Tooltip("The question will spawn when a patch number is reached.")]
    public bool triggerPatch = false;
    [Tooltip("The question will spawn when the time is reached.")]
    public bool triggerTime = false;
    [Tooltip("The question will spawn when player health is reached.")]
    public bool triggerHealth = false;
    [Tooltip("The question will spawn when player hits is reached.")]
    public bool triggerHits = false;
    [Tooltip("The question will spawn when player score is reached.")]
    public bool triggerScore = false;
    [Tooltip("Should the question spawn everytime the trigger activates it? If set to TRUE, spawnAmount will be ignored.")]
    public bool reuseQuestion = false;
    [Tooltip("How many times should the question spawn.")]
    public int spawnAmount = 1;

    [Header("Spawn Specifics (active depending on corresponding spawn trigger): ")]
    [Header("__________________________________________________________________")]
    [Tooltip("Spawn after which patch number?")]
    public int valuePatch = 5;
    [Tooltip("Spawn after how many seconds?\n"
            + "NOTE: if resuseQuestion is TRUE or spawnAmount > 1,\n"
            + "then the question will spawn every n seconds where n = triggerTime\n"
            + "until either spawnAmount is reached or indefinitely if resuseQuestion is TRUE.")]
    public int valueTime = 5;
    [Tooltip("Spawn after how much health?")]
    public int valueHealth = 10;
    [Tooltip("Spawn after how much hits?")]
    public int valueHits = 10;
    [Tooltip("Spawn after how much score?")]
    public int valueScore = 10;

    [Header("No edit necessary for the following values: ")]
    [Header("__________________________________________________________________")]
    [Tooltip("Will increase by triggerTime every time the question is spawned for repeated use.")]
    public int realTime;
}
