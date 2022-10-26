using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitLineObject : MonoBehaviour
{
    [Header("Object Settings:")]
    [Tooltip("Object type:\n0. Empty\n1. Obstacle\n2. Collectible")] public int type;

    public void TargetHit()
    {
        Hit();
    }

    public void InTargetRange(Target t, bool b)
    {
        if (b)
        {
            t.lineObject.Add(this); 
        }
        else
        {
            t.lineObject.Remove(this);
        }
    }


    /// <summary>
    /// Disables the object once it has hit something.
    /// </summary>
    public void Hit()
    {
        transform.gameObject.SetActive(false);
    }
}
