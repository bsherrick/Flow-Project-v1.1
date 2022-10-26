using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionChild : MonoBehaviour
{
    [Header("Type of Question (pick one):")]
    public bool questionBars;
    public bool questionSpectrum;

    bool done;
    QuestionParent parent;

    //For Bar Questions
    [Header("For Bar Questions:")]
    public int barNumber;
    public string barAnswer;

    //For Spectrum Questions
    Transform min;
    Transform max;
    public string spectrumAnswer;

    void Start()
    {
        done = false;

        if (questionBars)
        {
            parent = transform.parent.gameObject.transform.parent.gameObject.transform.parent.GetComponent<QuestionParent>();
            if (parent.showAllAnswers)
            {
                transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Text>().text = barAnswer;
                transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        else if (questionSpectrum)
        {
            parent = transform.parent.gameObject.transform.parent.GetComponent<QuestionParent>();
        }
        else
        {
            Debug.Log("ERROR: unknown question type in QuestionChild");
        }
    }

    void Update()
    {
        if (questionBars) //Question bars emit a raycast
        {
            QuestionBar_RayCast();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !done)
        {
            done = true;

            if (questionBars)
            {
                parent.Answered(barAnswer, 0); //send final answer to parent
            }
            else if (questionSpectrum)
            {
                //QuestionSpectrum_Compute(other.gameObject.transform); //computer player relative location
                parent.Answered(spectrumAnswer, 1); //send final answer to parent
            }
            else
            {
                Debug.Log("ERROR: unknown question type in QuestionChild");
            }
        }
    }

    /// <summary>
    /// Question Bars emit a raycast toward the player's direction
    /// </summary>
    void QuestionBar_RayCast()
    {
        RaycastHit objectHit;
        Vector3 direction = transform.TransformDirection(Vector3.left);

        Debug.DrawRay(transform.transform.position, direction * 20, Color.red);

        if (Physics.Raycast(transform.transform.position, direction, out objectHit, 20))
        {
            if (objectHit.collider.gameObject.tag == "Player")
            {
                //raycast is hitting the player
                transform.GetChild(0).gameObject.SetActive(true); //set the "X" mark active
                if (!parent.showAllAnswers)
                {
                    parent.ChangeCurrentAnswer(barAnswer); //show the player what is currently selected
                }
            }
            else
            {
                //raycast is hitting something but not the player
                transform.GetChild(0).gameObject.SetActive(false); //set the "X" mark inactive
            }
        }
        else
        {
            //raycast is not hitting anything
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void QuestionSpectrum_GotHit(RaycastHit info)
    {
        float z = info.point.z; //Range: 4.55 <-----> -4.55

        //swap positive & negative values so that the left side is negative and the right side is positive
        z *= -1;

        //set bounds
        if (z < -4.5) { z = -4.5f; }
        if (z > 4.5) { z = 4.5f; }

        //calculate percentage
        float percent = (z + 4.5f)/9;

        //get lower/upper bound of the spectrum
        float min = parent.spectrumMin;
        float max = parent.spectrumMax;

        //get the distance between the lower and upper bound of the spectrum
        float range = max - min;

        //get the value derived from the percentage of the range
        float value = range * percent;

        //the value plus the lower bound of the spectrum equals the result we want
        float result = parent.spectrumMin + value;

        double rounded = System.Math.Round(result, parent.spectrumRounded);
        spectrumAnswer = rounded + "";
        parent.ChangeCurrentAnswer(spectrumAnswer);
    }
}
