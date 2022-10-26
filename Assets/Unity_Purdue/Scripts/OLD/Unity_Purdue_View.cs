using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unity_Purdue_View : MonoBehaviour
{
    //Default Values
    static float smoothing2D_Default = 15;
    static float smoothing3D3rd_Default = 15;
    static float smoothing3DFPS_Default = 120;
    static bool swapAxis_Default = false;
    static bool cameraIs2D_Default = true;
    static bool cameraIs3DThirdPerson_Default = false;
    static bool cameraIs3DFPS_Default = false;

    Unity_Purdue_Player playerScript;
    Transform target;
    Vector3 offset;

    [HideInInspector]
    public bool cameraIs2D = cameraIs2D_Default;
    [HideInInspector]
    public bool cameraIs3DThirdPerson = cameraIs3DThirdPerson_Default;
    [HideInInspector]
    public bool cameraIs3DFPS = cameraIs3DFPS_Default;
    [HideInInspector]
    public bool swapAxis = swapAxis_Default;

    [Header("Camera Attributes (2D):")]
    [Tooltip("The 2D Camera that will follow the player.")]
    public GameObject camera2D;
    [Tooltip("How quickly the camera follows the player. Must be greater than 0.")]
    public float smoothing2D = smoothing2D_Default;
    Vector3 offset2D;

    [Header("Camera Attributes (3D 3rd Person):")]
    [Tooltip("The 3D third person Camera that will follow the player.")]
    public GameObject camera3D3rd;
    [Tooltip("How quickly the camera follows the player. Must be greater than 0.")]
    public float smoothing3D3rd = smoothing3D3rd_Default;
    Vector3 offset3D3rd;

    [Header("Camera Attributes (3D FPS):")]
    [Tooltip("The first person Camera that will follow the player.")]
    public GameObject camera3DFPS;
    [Tooltip("How quickly the camera follows the player. Must be greater than 0.")]
    public float smoothing3DFPS = smoothing3DFPS_Default;
    Vector3 offset3DFPS;

    void Start()
    {
        playerScript = GetComponent<Unity_Purdue_Player>();
        target = playerScript.playerObject.transform; //get the player character's location

        //2D
        offset2D = camera2D.transform.position - target.position;

        //3D3rd
        offset3D3rd = camera3D3rd.transform.position - target.position;

        //3DFPS
        offset3DFPS = camera3DFPS.transform.position - target.position;
    }

    void FixedUpdate()
    {
        //2D
        Vector3 targetCamPos2D = target.position + offset2D;
        camera2D.transform.position = Vector3.Lerp(camera2D.transform.position, targetCamPos2D, smoothing2D * Time.deltaTime);

        //3D3rd
        Vector3 targetCamPos3D3rd = target.position + offset3D3rd;
        camera3D3rd.transform.position = Vector3.Lerp(camera3D3rd.transform.position, targetCamPos3D3rd, smoothing3D3rd * Time.deltaTime);

        //3DFPS
        Vector3 targetCamPos3DFPS = target.position + offset3DFPS;
        camera3DFPS.transform.position = Vector3.Lerp(camera3DFPS.transform.position, targetCamPos3DFPS, smoothing3DFPS * Time.deltaTime);
    }

    public void setMode(int i)
    {
        //This function is used for changing game cameras. 0 for 2D, 1 for 3D third person, 2 for 3D first person

        if (i == 0)
        {
            cameraIs2D = true;
            cameraIs3DThirdPerson = false;
            cameraIs3DFPS = false;
            swapAxis = false; //no swap axis
        }
        else if (i == 1)
        {
            cameraIs2D = false;
            cameraIs3DThirdPerson = true;
            cameraIs3DFPS = false;
            swapAxis = true; //swap axis for correct player input
        }
        else
        {
            cameraIs2D = false;
            cameraIs3DThirdPerson = false;
            cameraIs3DFPS = true;
            swapAxis = true; //swap axis for correct player input
        }

        camera2D.SetActive(cameraIs2D);
        camera3D3rd.SetActive(cameraIs3DThirdPerson);
        camera3DFPS.SetActive(cameraIs3DFPS);
    }

    public void reset()
    {
       smoothing2D = smoothing2D_Default ;
       smoothing3D3rd = smoothing3D3rd_Default;
       smoothing3DFPS = smoothing3DFPS_Default;
       cameraIs2D = cameraIs2D_Default;
       cameraIs3DThirdPerson = cameraIs3DThirdPerson_Default;
       cameraIs3DFPS = cameraIs3DFPS_Default;
       swapAxis = swapAxis_Default;
    }
}
