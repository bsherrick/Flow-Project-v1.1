using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Unity_Purdue_Player playerScript;

    //public Transform target;
    Transform target;
    public float smoothing = 5f;
    Vector3 offset;

    void Start()
    {
        target = playerScript.playerObject.transform;
        offset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
