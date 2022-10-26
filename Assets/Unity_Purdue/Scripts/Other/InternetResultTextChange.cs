using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InternetResultTextChange : MonoBehaviour
{
    public string successMessage = "SUCCESS";
    public string failureMessage = "FAILED";
    public float seconds = 0.5f;
    Text text;

    void Start()
    {
        text = GetComponent<Text>();
        text.text = "";
    }

   
    void Update()
    {
        
    }

    public void Result(bool outcome)
    {
        StopAllCoroutines();
        if (outcome)
        {
            text.text = successMessage;
        }
        else
        {
            text.text = failureMessage;
        }
        StartCoroutine(EraseResult());
    }

    IEnumerator EraseResult()
    {
        yield return new WaitForSecondsRealtime(seconds);
        text.text = "";
    }
}
