using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveAnswer : MonoBehaviour
{
    bool done = false;
    InteractiveAnswerParent parent;

    public bool isYesNoQuestion;
    public bool isYes;
    public int number;
    string answer;

    void Start()
    {
        parent = transform.parent.gameObject.transform.parent.GetComponent<InteractiveAnswerParent>();
        if (parent == null)
        {
            Debug.Log("ERROR: can't find InteractiveAnswerParent from parent of parent");
        }
        if (isYesNoQuestion)
        {
            if (isYes)
            {
                answer = "Yes";
            }
            else
            {
                answer = "No";
            }
        }
        else
        {
            answer = number + "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !done)
        {
            done = true;
            parent.answered(answer);
        }
    }
}
