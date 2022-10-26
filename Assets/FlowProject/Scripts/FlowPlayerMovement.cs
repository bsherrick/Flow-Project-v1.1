using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowPlayerMovement : MonoBehaviour
{
    [HideInInspector] public FlowMain flow;

    [Tooltip("Display debugging messages?")] public bool showDebug;

    [Header("References:")]
    [Tooltip("The Player Character Game Object.")] public GameObject playerCharacter;
    [Tooltip("The Player Character's Model.")] public GameObject playerModel;
    [Tooltip("The target board.")] public GameObject targetBoard;
    [Tooltip("The targets on the target board.")] public GameObject[] targets;
    [Tooltip("GameObjects to activate to show that a button has been pressed. (For controlButton mode)")] public GameObject[] buttonPressed;

    [Header("Settings:")]
    [Tooltip("TRUE for allowing user input for player movement.")] public bool allowUserInput;
    [Tooltip("Movement speed of player.")] public float movementSpeed = 10;
    [Tooltip("How strong the player model shakes when \"attacked\"")] public float modelFloatAmp = 0.11f;
    [Tooltip("How much the player model shakes when \"attacked\"")] public float modelFloatFreq = 21f;
    [Tooltip("How long the player model shakes when \"attacked\"")] public float modelShakeTime = 0.25f;

    [Header("Status (DO NOT MODIDY):")]
    [Tooltip("TRUE if Player is by wall1.")] public bool limitWall1 = false;
    [Tooltip("TRUE if Player is by wall2.")] public bool limitWall2 = false;
    [Tooltip("the vector position of playerCharacter.")] public Vector3 playerCharacterMovement;
    [Tooltip("Check for user input.")] public float axis;
    [Tooltip("The rigidbody of playerCharacter.")] public Rigidbody playerCharacterRigidbody;
    [Tooltip("The target that the playerCharacter is currently sitting on.")] public int currentTarget;
    [Tooltip("To copy the player model's attributes for when we want to change it back.")] public float floatAmpOriginal;
    [Tooltip("To copy the player model's attributes for when we want to change it back.")] public float floatFreqOriginal;

    public void Initialize()
    {
        playerCharacterRigidbody = playerCharacter.GetComponent<Rigidbody>(); //get playerCharacter rigidbody
        currentTarget = 3; //there are 5 targets, the playerCharacter is in the middle which is target number 3
    }

    void FixedUpdate()
    {
        if (!flow.gameOn)
        {
            return;
        }

        if (!allowUserInput)
        {
            return;
        }

        axis = Input.GetAxisRaw("Horizontal");

        if ((limitWall1 && axis < 0) || (limitWall2 && axis > 0)) //player will not be able to most past walls (boundaries)
        {
            return;
        }

        //Controls
        if ((flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_ClassicSmooth) || (flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_AlgorithmSmooth))
        {
            //Smooth Move
            playerCharacterMovement.Set(0f, 0f, -axis);
            playerCharacterMovement = playerCharacterMovement.normalized * movementSpeed * Time.deltaTime;
            playerCharacterRigidbody.MovePosition(playerCharacterRigidbody.transform.position + playerCharacterMovement);
        }
    }

    #region model shake

    /// <summary>
    /// Shake the player model when the player collides
    /// </summary>
    public void ShakeModel()
    {
        playerModel.GetComponentInChildren<FloatAndRotate>().Modify(-1, modelFloatAmp, modelFloatFreq, modelShakeTime);
        //playerModel.GetComponentInChildren<FloatAndRotate>().amplitude = modelFloatAmp;
        //playerModel.GetComponentInChildren<FloatAndRotate>().frequency = modelFloatFreq;
        //StartCoroutine(StopShakeModel());
    }

    //IEnumerator StopShakeModel()
    //{
    //    yield return new WaitForSeconds(modelShakeTime);
    //    playerModel.GetComponentInChildren<FloatAndRotate>().amplitude = floatAmpOriginal;
    //    playerModel.GetComponentInChildren<FloatAndRotate>().frequency = floatFreqOriginal;
    //}

    #endregion

    void Update()
    {
        if (!allowUserInput || !flow.gameOn)
        {
            return;
        }

        //Controls
        if (flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_Rhythm)
        {
            //Activate Button

            #region buttonInputs
            //Button 1
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                flow.Targets[0].TriggerTarget();
                buttonPressed[0].SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                buttonPressed[0].SetActive(false);
            }

            //Button 2
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                flow.Targets[1].TriggerTarget();
                buttonPressed[1].SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                buttonPressed[1].SetActive(false);
            }

            //Button 3
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                flow.Targets[2].TriggerTarget();
                buttonPressed[2].SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                buttonPressed[2].SetActive(false);
            }

            //Button 4
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                flow.Targets[3].TriggerTarget();
                buttonPressed[3].SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                buttonPressed[3].SetActive(false);
            }

            //Button 5
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                flow.Targets[4].TriggerTarget();
                buttonPressed[4].SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                buttonPressed[4].SetActive(false);
            }
            #endregion
        }
        else if ((flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_ClassicChop) || (flow.FlowGameConfig.gamePlay == FlowGameConfig.gamePlay_AlgorithmChop))
        {
            //Choppy Move

            //Left
            if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && currentTarget != 1)
            {
                playerCharacter.transform.position = new Vector3(playerCharacter.transform.position.x, playerCharacter.transform.position.y, targets[--currentTarget - 1].transform.position.z);
            }

            //Right
            else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && currentTarget != 5)
            {
                playerCharacter.transform.position = new Vector3(playerCharacter.transform.position.x, playerCharacter.transform.position.y, targets[++currentTarget - 1].transform.position.z);
            }
        }
    }

    
}
