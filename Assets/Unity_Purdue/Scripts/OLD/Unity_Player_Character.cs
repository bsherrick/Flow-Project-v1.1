using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unity_Player_Character : MonoBehaviour
{

    public Animator playerCharacterAnim;
    public Unity_Purdue_Player player;
    
    void Start()
    {
        
    }

    void Update()
    {
        /*
        //if player is jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerCharacterAnim.SetBool("isJumping", true);
        }

        //if player is moving
        if (player.h != 0 || player.v != 0)
        {
            playerCharacterAnim.SetBool("isMoving", true);
        }
        else
        {
            playerCharacterAnim.SetBool("isMoving", false);
        }
        */
    }

    public void floorTouched()
    {
        /*
        playerCharacterAnim.SetBool("isJumping", false);
        */
    }
}
