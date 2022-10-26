using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionAnswer : MonoBehaviour
{
    public string answer;
    public QuestionLine parent;
    public bool active = false;

    public void Answered()
    {
        parent.Answered(answer);
    }

    
}
