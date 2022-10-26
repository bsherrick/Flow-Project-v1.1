using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionEnter : MonoBehaviour
{
    bool done;
    CanvasScript canvas;
    QuestionParent parent;

    void Start()
    {
        done = false;
        parent = transform.parent.gameObject.GetComponent<QuestionParent>();
        canvas = parent.canvas;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !done)
        {
            done = true;
            //canvas.SetGameSpeed(0.75f); //slow down game
            canvas.referencer.GameFlowFramework_PlayerCharacter.Slowed(true);
            parent.ShowQuestion(); //display the question on the screen
        }
    }
}
