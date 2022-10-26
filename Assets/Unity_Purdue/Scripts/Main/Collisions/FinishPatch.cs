using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPatch : MonoBehaviour
{
    bool done;
    GameFlowFramework_ScriptReferencer referencer;
    GameFlowFramework_WinLoseCondition winlose;

    void Start()
    {
        done = false;
        referencer = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameFlowFramework_ScriptReferencer>();
        winlose = referencer.GameFlowFramework_WinLoseCondition;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !done)
        {
            done = true;

            referencer.CanvasScript.GameOver();
        }
    }
}
