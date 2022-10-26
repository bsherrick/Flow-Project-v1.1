using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    //This script is to be placed on every obstacle. A collider set to "trigger" is required for all obstacles as well.

    Unity_Purdue_Difficulty script;

    void Start()
    {
        script = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Unity_Purdue_Difficulty>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the player character hits the obstacle

        if (other.gameObject.tag == "Player")
        {
            script.damaged();
        }
    }
}
