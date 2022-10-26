using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeModeEndPatch : MonoBehaviour
{
    bool done;
    Unity_Purdue_Difficulty difficult;

    void Start()
    {
        done = false;
        difficult = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Unity_Purdue_Difficulty>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !done)
        {
            done = true;
            difficult.endOfPatch();
        }
    }
}
