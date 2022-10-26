using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitLine : MonoBehaviour
{

    [Header("Line Settings:")]
    [Tooltip("The speed of the line movement.")] public float speed = 2;

    //reference to the line's 5 objects
    public GameObject[] lineObject;

    Rigidbody rigid;
    Vector3 movement;

    FlowMain flow;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
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

    /// <summary>
    /// Set reference and type of each object in a line.
    /// </summary>
    /// <param name="type">Size of array should be equal to the number of objects in a line.</param>
    /// <param name="f">The flow script to be passed down.</param>
    public void LineInit(int[] type, FlowMain f)
    {
        flow = f;
        lineObject = new GameObject[type.Length];
        for (int i = 0; i < type.Length; i++)
        {
            lineObject[i] = transform.GetChild(i).gameObject;
            HitLineObject lineObjectScript = lineObject[i].GetComponent<HitLineObject>();
            lineObjectScript.type = type[i];
            switch (type[i])
            {
                case 0: //empty
                    break;
                case 1: //obstacle
                    lineObject[i].transform.GetChild(0).gameObject.SetActive(true);
                    break;
                case 2: //collectible
                    lineObject[i].transform.GetChild(1).gameObject.SetActive(true);
                    break;
                default:
                    Debug.Log("Error setting line object, invalid type.");
                    break;
            }
            SetTheme(i, type[i]);
        }
    }

    void SetTheme(int lineObjectIndex, int type)
    {
        if (flow.FlowLineGenerator.themeDefault || flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_Rhythm)
        {
            //do nothing
            return;
        }

        if (flow.FlowLineGenerator.themeSpace)
        {
            switch (type)
            {
                case 0: //empty
                    break;
                case 1: //obstacle
                    lineObject[lineObjectIndex].transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
                    lineObject[lineObjectIndex].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    int rand = Random.Range(0, 6);
                    lineObject[lineObjectIndex].transform.GetChild(0).GetChild(0).GetChild(rand).gameObject.SetActive(true);
                    break;
                case 2: //collectible
                    lineObject[lineObjectIndex].transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = false;
                    lineObject[lineObjectIndex].transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    break;
                default:
                    Debug.Log("Error setting line object theme, invalid type.");
                    break;
            }
        }
    }
}
