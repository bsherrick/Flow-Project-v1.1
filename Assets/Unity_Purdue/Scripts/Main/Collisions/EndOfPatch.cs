using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfPatch : MonoBehaviour
{
    [Tooltip("0: genesis patch,\n1: regular patch,\n2: pre-question patch,\n3: question patch,\n4: finisher patch")]
    public int role;

    bool done;
    GameFlowFramework_Environment environment;

    void Start()
    {
        done = false;
        environment = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameFlowFramework_Environment>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !done)
        {
            done = true;
            environment.EnteredPatch(role);
        }
    }
}
