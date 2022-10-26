using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHitDebug : MonoBehaviour
{

    AudioSource sound;
    FlowMain flow;

    void Start()
    {
        sound = GetComponent<AudioSource>();
        flow = GameObject.FindGameObjectWithTag("GameManager").GetComponent<FlowMain>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (flow.FlowLineGenerator.targetDebug && other.gameObject.tag == "LineObject" && other.gameObject.GetComponent<HitLineObject>().type != 0)
        {
            sound.Play();
        }
    }


}
