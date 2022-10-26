using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unity_Purdue_Player : MonoBehaviour
{
    //Default Values
    static float movementSpeed_Default = 10;
    static float jumpForce_Default = 25;
    static bool autoMove_Default = true;
    static bool extraMovement_Default = true;
    static float jumpCoolDown_Default = 1;

    [Header("Player Attributes:")]
    [Tooltip("Player movement speed.")]
    public float movementSpeed = movementSpeed_Default;
    [Tooltip("Player jumping force.")]
    public float jumpForce = jumpForce_Default;
    [Tooltip("Player jumping cooldown.")]
    public float jumpCoolDown = jumpCoolDown_Default;
    [Tooltip("Player GameObject. It must have:\n 1. RigidBody\n 2. Collider")]
    public GameObject playerObject;

    [Header("Player Controls:")]
    [Tooltip("True if player moves forward automatically.")]
    public bool autoMove = autoMove_Default;
    [Tooltip("True if player can move an additional axis (sideways in addition to forward/backward).")]
    public bool extraMovement = extraMovement_Default;

    Vector3 movement; //the vector position of playerObject
    Rigidbody playerRigidbody; //the rigidbody of playerObject
    bool canJump = true; //check jumpCoolDown
    Unity_Purdue_View viewScript;
    Unity_Purdue_Difficulty difficultyScript;

    [HideInInspector]
    public float h;
    public float v;

    void Start()
    {
        playerRigidbody = playerObject.GetComponent<Rigidbody>();
        viewScript = GetComponent<Unity_Purdue_View>();
        difficultyScript = GetComponent<Unity_Purdue_Difficulty>();
    }

    void Update()
    {
        //Jump (only in Game Mode Jump)
        if (Input.GetKeyDown(KeyCode.Space) && canJump && difficultyScript.gameModeJump)
        {
            canJump = false; //disable further jumps
            playerRigidbody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse); //jump
            StartCoroutine(jumpCoolDownReset()); //allow jumping again after cooling down
        }
    }

    IEnumerator jumpCoolDownReset()
    {
        yield return new WaitForSeconds(jumpCoolDown);
        canJump = true;
    }

    void FixedUpdate()
    {
        h = 0;
        v = 0;

        if (autoMove && viewScript.swapAxis) //if automove is enabled AND played input is swapped (for left-right controls)
        {
            h = 1;
            v = -(Input.GetAxisRaw("Horizontal"));
        }
        else if (autoMove && !viewScript.swapAxis) //if automove is enabled AND played input is not swapped (for left-right controls)
        {
            h = 1;
            v = Input.GetAxisRaw("Vertical");
        }
        else if (!autoMove && viewScript.swapAxis) //if automove is disabled AND played input is swapped (for left-right controls)
        {
            h = Input.GetAxisRaw("Vertical");
            v = -(Input.GetAxisRaw("Horizontal"));
        }
        else if (!autoMove && !viewScript.swapAxis) //if automove is disabled AND played input is not swapped (for left-right controls)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }

        Move(h, v);
    }

    void Move(float h, float v)
    {
        //the function moves the player character

        if (extraMovement)
        {
            movement.Set(h, 0f, v);
        }
        else
        {
            movement.Set(h, 0f, 0f);
        }
        movement = movement.normalized * movementSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(playerObject.transform.position + movement);
    }

    public void reset()
    {
        movementSpeed = movementSpeed_Default;
        jumpForce = jumpForce_Default;
        autoMove = autoMove_Default;
        extraMovement = extraMovement_Default;
        jumpCoolDown = jumpCoolDown_Default;
    }
}