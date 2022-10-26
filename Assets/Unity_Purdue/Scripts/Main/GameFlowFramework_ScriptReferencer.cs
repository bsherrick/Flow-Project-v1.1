using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowFramework_ScriptReferencer : MonoBehaviour
{
    [Header("Scripts")]
    public GameFlowFramework_PlayerCharacter GameFlowFramework_PlayerCharacter;
    public GameFlowFramework_GameCamera GameFlowFramework_GameCamera;
    public GameFlowFramework_Environment GameFlowFramework_Environment;
    public GameFlowFramework_WinLoseCondition GameFlowFramework_WinLoseCondition;
    public GameFlowFramework_Questions GameFlowFramework_Questions;
    public GameFlowFramework_Web GameFlowFramework_Web;
    public CanvasScript CanvasScript;
    public QuestionEditor QuestionEditor;
    public GeneralSettings GeneralSettings;

    [Header("Audio Files")]
    public AudioSource soundtrack1;
    public AudioSource soundtrack2;
    public AudioSource effect_8bitExplode;
    public AudioSource effect_8bitVictory;
    public AudioSource effect_click;
    public AudioSource effect_laserShoot;

    [Header("Particle Systems")]
    public ParticleSystem destroyEffect1;
    public ParticleSystem destroyEffect2;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
