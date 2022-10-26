using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowFramework_PlayerCharacter : MonoBehaviour
{
    //Default Values
    /****************************************************************************/
    //Player Attributes
    static int health_Default = 10;
    static int hits_Default = 0;
    static int score_Default = 0;
    static float modelFloatAmp_Default = 0.11f;
    static float modelFloatFreq_Default = 21f;
    static float modelShakeTime_Default = 0.25f;
    //Player Abilities
    static float movementSpeed_Default = 10;
    static float movementSpeedSlowed_Default = 5;
    static bool allowJump_Default = true;
    static float jumpForce_Default = 25;
    static float jumpCoolDown_Default = 1;
    //Player Controls
    static bool controlsEnabled_Default = true;
    static bool autoMove_Default = true;
    static bool extraMovement_Default = true;
    /****************************************************************************/
    [Header("General:")]
    [Header("__________________________________________________________________")]
    [Tooltip("Canvas script attached to the main canvas.")]
    public CanvasScript canvas;

    [Header("Player Attributes:")]
    [Header("__________________________________________________________________")]
    [Tooltip("Player GameObject. It must have:\n 1. RigidBody\n 2. Collider")]
    public GameObject playerObject;
    [Tooltip("Current Player model (a child of the Player Gameobject e.g. Spaceship), note: must have FloatAndRotate script attached to it.")]
    public GameObject playerModel;
    [Tooltip("The starting health of the player.")]
    public int health = health_Default;
    [Tooltip("The starting hit count of the player.")]
    public int hits = hits_Default;
    [Tooltip("The starting score of the player.")]
    public int score = score_Default;
    [Tooltip("How strong the player model shakes when \"attacked\"")]
    public float modelFloatAmp = modelFloatAmp_Default;
    [Tooltip("How much the player model shakes when \"attacked\"")]
    public float modelFloatFreq = modelFloatFreq_Default;
    [Tooltip("How long the player model shakes when \"attacked\"")]
    public float modelShakeTime = modelShakeTime_Default;

    [Header("Player Abilities:")]
    [Header("__________________________________________________________________")]
    [Tooltip("Player movement speed.")]
    public float movementSpeed = movementSpeed_Default;
    [Tooltip("Player movement speed when \"slowed\".")]
    public float movementSpeedSlowed = movementSpeedSlowed_Default;
    [Tooltip("Allow jumping?")]
    public bool allowJump = allowJump_Default;
    [Tooltip("Player jumping force.")]
    public float jumpForce = jumpForce_Default;
    [Tooltip("Player jumping cooldown.")]
    public float jumpCoolDown = jumpCoolDown_Default;

    [Header("Player Controls:")]
    [Header("__________________________________________________________________")]
    [Tooltip("True if player can be controlled by user.")]
    public bool controlsEnabled = controlsEnabled_Default;
    [Tooltip("True if player moves forward automatically.")]
    public bool autoMove = autoMove_Default;
    [Tooltip("True if player can move an additional axis (sideways in addition to forward/backward).")]
    public bool extraMovement = extraMovement_Default;

    GameFlowFramework_ScriptReferencer referencer;
    Vector3 movement; //the vector position of playerObject
    Rigidbody playerRigidbody; //the rigidbody of playerObject
    bool canJump = true; //check jumpCoolDown
    ParticleSystem playerHit; //the hit effect when the player collides with something
    float currentSpeed; //the current speed of the player

    [HideInInspector]
    public float h;
    [HideInInspector]
    public float v;

    //to copy the player model's attributes for when we want to change it back
    float floatAmpOriginal;
    float floatFreqOriginal;

    QuestionChild child; //for giving Raycast information

    void Start()
    {
        playerRigidbody = playerObject.GetComponent<Rigidbody>();
        referencer = GetComponent<GameFlowFramework_ScriptReferencer>();
        playerHit = playerObject.GetComponentInChildren<ParticleSystem>();
        currentSpeed = movementSpeed;

        floatAmpOriginal = playerModel.GetComponent<FloatAndRotate>().amplitude;
        floatFreqOriginal = playerModel.GetComponent<FloatAndRotate>().frequency;
    }

    void Update()
    {
        if (!controlsEnabled)
        {
            return;
        }

        //Jump
        if (allowJump && Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            canJump = false; //disable further jumps
            playerRigidbody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse); //jump
            StartCoroutine(JumpCoolDownReset()); //allow jumping again after cooling down
        }

        //Raycast
        CheckRayCast();
    }

    void FixedUpdate()
    {
        if (!controlsEnabled)
        {
            return;
        }

        h = 0;
        v = 0;

        if (autoMove && referencer.GameFlowFramework_GameCamera.swapAxis) //if automove is enabled AND played input is swapped (for left-right controls)
        {
            h = 1;
            v = -(Input.GetAxisRaw("Horizontal"));
        }
        else if (autoMove && !referencer.GameFlowFramework_GameCamera.swapAxis) //if automove is enabled AND played input is not swapped (for left-right controls)
        {
            h = 1;
            v = Input.GetAxisRaw("Vertical");
        }
        else if (!autoMove && referencer.GameFlowFramework_GameCamera.swapAxis) //if automove is disabled AND played input is swapped (for left-right controls)
        {
            h = Input.GetAxisRaw("Vertical");
            v = -(Input.GetAxisRaw("Horizontal"));
        }
        else if (!autoMove && !referencer.GameFlowFramework_GameCamera.swapAxis) //if automove is disabled AND played input is not swapped (for left-right controls)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }

        Move(h, v);
    }

    public void HealthToFifty()
    {
        health = 50;
        referencer.GameFlowFramework_Questions.TriggerHealthCheck(health);
    }

    IEnumerator JumpCoolDownReset()
    {
        yield return new WaitForSeconds(jumpCoolDown);
        canJump = true;
    }

    void Move(float h, float v)
    {
        //moving the player character

        if (extraMovement)
        {
            movement.Set(h, 0f, v);
        }
        else
        {
            movement.Set(h, 0f, 0f);
        }
        movement = movement.normalized * currentSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(playerObject.transform.position + movement);
    }

    /// <summary>
    /// To slow the player character.
    /// Useful for cases such as when a question is asked of the player.
    /// </summary>
    /// <param name="b">True for making the player slow down, False for returning the player to normal speed.</param>
    public void Slowed(bool b)
    {
        if (b)
        {
            currentSpeed = movementSpeedSlowed;
        }
        else
        {
            currentSpeed = movementSpeed;
        }
    }

    /// <param name="amount">The amount to add to target variable (negative values allowed).</param>
    /// <param name="target">Target variable to affect (0: health, 1: score, 2: time).</param>
    public void Collided(int amount, int target)
    {
        playerHit.Play();

        //check if target is valid
        if (target < 0 || target > 2)
        {
            Debug.Log("ERROR collided target variable!");
        }

        switch (target)
        {
            case 0:
                health += amount;
                referencer.GameFlowFramework_Questions.TriggerHealthCheck(health);
                hits -= amount;
                referencer.GameFlowFramework_Questions.TriggerHitsCheck(hits);
                break;
            case 1:
                score += amount;
                referencer.GameFlowFramework_Questions.TriggerScoreCheck(score);
                break;
            case 2:
                //TO DO
                break;
        }

        ShakePlayerModel();
        UpdateCanvas();
    }

    /// <summary>
    /// Shoots a raycast from the player character frontwards in order to detect objects in front
    /// Currently used only to detect spectrum questions
    /// </summary>
    void CheckRayCast()
    {

        RaycastHit objectHit;
        Vector3 fwd = playerObject.transform.TransformDirection(Vector3.right);

        Debug.DrawRay(playerObject.transform.position, fwd * 20, Color.red);

        if (Physics.Raycast(playerObject.transform.position, fwd, out objectHit, 20))
        {
            if (objectHit.collider.gameObject.tag == "Spectrum")
            {
                //raycast is hitting a spectrum question
                child = objectHit.collider.gameObject.GetComponent<QuestionChild>(); //get QuestionChild component
                child.QuestionSpectrum_GotHit(objectHit); //give the raycast information to the spectrum question
            }
            else
            {
                //raycast is hitting something but not a spectrum question
            }
        }
        else
        {
            //raycast is not hitting anything
        }

    }

    public void UpdateCanvas()
    {
        canvas.healthCount = health;
        canvas.scoreCount = score;
        canvas.hitsCount = hits;
    }

    void ShakePlayerModel()
    {
        playerModel.GetComponent<FloatAndRotate>().amplitude = modelFloatAmp;
        playerModel.GetComponent<FloatAndRotate>().frequency = modelFloatFreq;
        StartCoroutine(StopShakePlayerModel());
    }

    IEnumerator StopShakePlayerModel()
    {
        yield return new WaitForSeconds(modelShakeTime);
        playerModel.GetComponent<FloatAndRotate>().amplitude = floatAmpOriginal;
        playerModel.GetComponent<FloatAndRotate>().frequency = floatFreqOriginal;
    }

    public void ResetValues()
    {
        //Player Attributes
        health = health_Default;
        hits = hits_Default;
        score = score_Default;
        modelFloatAmp = modelFloatAmp_Default;
        modelFloatFreq = modelFloatFreq_Default;
        modelShakeTime = modelShakeTime_Default;
        //Player Abilities
        movementSpeed = movementSpeed_Default;
        movementSpeedSlowed = movementSpeedSlowed_Default;
        allowJump = allowJump_Default;
        jumpForce = jumpForce_Default;
        jumpCoolDown = jumpCoolDown_Default;
        //Player Controls
        controlsEnabled = controlsEnabled_Default;
        autoMove = autoMove_Default;
        extraMovement = extraMovement_Default;
        //other
        UpdateCanvas();
    }
}
