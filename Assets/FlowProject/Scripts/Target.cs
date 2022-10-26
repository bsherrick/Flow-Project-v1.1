using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [HideInInspector] public FlowMain flow;

    [Header("References:")]
    [Tooltip("List of particle effects to play when the target is hit.")] public ParticleSystem[] hitEffects;
    [Tooltip("List of particle effects to play when the target is missed.")] public ParticleSystem[] missEffects;
    [Tooltip("List of beats (audio) available.")] public AudioSource[] beats;

    [Header("Settings:")]
    [Tooltip("Beat:\n0. Clap\n1. Hat\n2. Kick\n3. Snare")] public int beat = 0;

    public List<HitLineObject> lineObject; //the script from the object in the line

    public void TriggerTarget()
    {
        beats[beat].Play();

        if (lineObject.Count == 0) //miss
        {
            return;
        }

        for (int i = 0; i < lineObject.Count; i++)
        {
            lineObject[i].TargetHit();
            flow.FlowGameConfig.TargetHit(1);
        }
        lineObject.Clear();

        for (int i = 0; i < hitEffects.Length; i++) { hitEffects[i].Play(); } //play effects
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LineObject" && other.gameObject.GetComponent<HitLineObject>().type != 0)
        {
            other.gameObject.GetComponent<HitLineObject>().InTargetRange(this, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "LineObject" && other.gameObject.GetComponent<HitLineObject>().type != 0)
        {
            other.gameObject.GetComponent<HitLineObject>().InTargetRange(this, false);  
            flow.FlowGameConfig.TargetHit(0);
            for (int i = 0; i < missEffects.Length; i++) { missEffects[i].Play(); } //play effects
        }
    }
}
