using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetPlayer : MonoBehaviour
{
    [Header("Public Variables:")]
    [Tooltip("The script referencer.")] public FlowMain flow;
    public GameObject canvasAnswer;
    public Text textAnswer;

    [Header("General Settings:")]
    [Tooltip("Tilt rotation in degrees.")] public float tiltDegrees;
    [Tooltip("Tilt speed.")] public float tiltSpeed;

    HitLineObject lineObject; //the script from the object in the line
    QuestionAnswer qAnswer; //the script from the answer in the line

    [Header("Status (Do Not Modify):")]
    public Vector3 currRotation;

    Transform model;
    bool onLeft = false;
    bool onRight = false;
    float speed;

    QuestionAnswer qa;

    void Update()
    {
        if (flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_Rhythm)
        {
            return;
        }

        RayCast();

        model = flow.FlowPlayerMovement.playerModel.transform;
        currRotation = new Vector3(model.rotation.x, model.rotation.y, model.rotation.z);

        //Left
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && !onRight)
        {
            onLeft = true;
            StopAllCoroutines();
            StartCoroutine(Rotate(tiltDegrees));
        }
        //Right
        else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && !onLeft)
        {
            onRight = true;
            StopAllCoroutines();
            StartCoroutine(Rotate(-tiltDegrees));
        }
        //None
        else if ( ((Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)) && onLeft)
            ||
            ((Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))) && onRight)
        {
            onLeft = false;
            onRight = false;
            StopAllCoroutines();
            StartCoroutine(Rotate(0));
        }
    }

    IEnumerator Rotate(float targetDegrees)
    {
        speed = tiltSpeed;
        if (targetDegrees == 0) { speed *= 2; }
        while (Mathf.Abs(model.transform.rotation.x) > -(Mathf.Abs(targetDegrees)))
        {
            
            model.transform.rotation = Quaternion.Slerp(model.transform.rotation, Quaternion.Euler(targetDegrees, 0, 0), speed * Time.deltaTime);
            yield return null;
        }
        model.transform.rotation = Quaternion.Euler(targetDegrees, 0, 0);
        yield return null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LineObject")
        {
            lineObject = other.gameObject.GetComponent<HitLineObject>();
            lineObject.Hit();
            flow.FlowGameConfig.PlayerHit(lineObject.type);

        }
        else if (other.gameObject.tag == "wall1")
        {
            flow.FlowPlayerMovement.limitWall1 = true;
        }
        else if (other.gameObject.tag == "wall2")
        {
            flow.FlowPlayerMovement.limitWall2 = true;
        }
        else if (other.gameObject.tag == "QuestionSlotAnswer")
        {
            qAnswer = other.gameObject.GetComponent<QuestionAnswer>();
            qAnswer.Answered();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "wall1")
        {
            flow.FlowPlayerMovement.limitWall1 = false;
        }
        else if (other.gameObject.tag == "wall2")
        {
            flow.FlowPlayerMovement.limitWall2 = false;
        }
    }

    public void ShowAnswer(bool show, string answer)
    {
        textAnswer.text = answer;
    }

    void RayCast()
    {
        RaycastHit objectHit;
        Vector3 direction = transform.TransformDirection(Vector3.right);

        Debug.DrawRay(transform.transform.position, direction * 15, Color.red);

        if (Physics.Raycast(transform.transform.position, direction, out objectHit, 15))
        {
            //Debug.Log("ray: " + objectHit.collider.gameObject.name);
            if (objectHit.collider.gameObject.tag == "QuestionSlotAnswer")
            {
                //raycast is QuestionSlotAnswer
                qa = objectHit.collider.gameObject.GetComponent<QuestionAnswer>();
                if (!qa.active) { return; }
                canvasAnswer.SetActive(true);
                textAnswer.text = qa.answer;
            }
            else
            {
                //raycast is hitting something but not QuestionSlotAnswer
                canvasAnswer.SetActive(false);
            }
        }
        else
        {
            //raycast is not hitting anything
            canvasAnswer.SetActive(false);
        }
    }
}
