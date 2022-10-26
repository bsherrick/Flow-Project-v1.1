using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [Header("References:")]
    [Tooltip("The script referencer.")] public FlowMain flow;

    [Header("General Settings:")]
    [Tooltip("Is this the start of the stage?")] public bool isStart;

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "LineBoard" && isStart)
        {
            //the line has moved out of its spawning location
            if (flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_AlgorithmSmooth || flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_AlgorithmChop) //if in Algorithm mode
            {
                flow.FlowLineGenerator.SpawnLineAlgorithm();
                //Debug.Log("Spawn Line");//spawn a new line
            }
            else if (flow.FlowGameConfig.gamePlay != FlowGameConfig.gamePlay_Rhythm) //if not in rhythm mode or Algorithm
            {
                flow.FlowLineGenerator.SpawnLine();
            }
            else
            {
                //flow.FlowLineGenerator.SpawnLineRhythm(); //spawn a new line
            }
        }
        else if (other.gameObject.tag == "LineBoard" && !isStart)
        {
            //the line has passed the ending location
            flow.FlowLineGenerator.linesInGame.Remove(other.transform.parent.gameObject);
            Destroy(other.transform.parent.gameObject);
        }
        else if (other.gameObject.tag == "LineBoardQuestion" && !isStart)
        {
            //the line has passed the ending location
            flow.FlowQuestionHandler.QuestionAnswered();
            flow.FlowLineGenerator.linesInGame.Remove(other.transform.parent.gameObject);
            Destroy(other.transform.parent.gameObject);
        }
    }
}
