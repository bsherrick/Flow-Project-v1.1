using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorTouch : MonoBehaviour
{

    Unity_Player_Character upc;

    void Start()
    {
        upc = GetComponentInParent<Unity_Player_Character>();   
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            upc.floorTouched();
        }
    }
}
