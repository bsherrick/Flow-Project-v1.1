using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif


#if UNITY_EDITOR

[CustomEditor(typeof(Unity_Purdue_Graphics))]
[ExecuteAlways]
public class CustomInspector1 : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("To change graphics please turn the camera filter scripts\non/off manually in the cameras under the Cameras\ngame object.");
        DrawDefaultInspector();
    }
}

[CustomEditor(typeof(Unity_Purdue_View))]
[ExecuteAlways]
public class CustomInspector2 : Editor
{
    public override void OnInspectorGUI()
    {
        Unity_Purdue_View script = (Unity_Purdue_View)target;

        GUILayout.Label("Select a camera view mode:");

        bool make2D = script.cameraIs2D;
        make2D = GUILayout.Toggle(make2D, "2D");
        if (make2D)
        {
            script.setMode(0);
        }

        bool make3D3rd = script.cameraIs3DThirdPerson;
        make3D3rd = GUILayout.Toggle(make3D3rd, "3D 3rd Person");
        if (make3D3rd)
        {
            script.setMode(1);
        }

        bool make3DFPS = script.cameraIs3DFPS;
        make3DFPS = GUILayout.Toggle(make3DFPS, "3D FPS");
        if (make3DFPS)
        {
            script.setMode(2);
        }

        DrawDefaultInspector();
        if (GUILayout.Button("Reset to Default"))
        {
            script.reset();
        }
    }
}

[CustomEditor(typeof(Unity_Purdue_Player))]
[ExecuteAlways]
public class CustomInspector3 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Unity_Purdue_Player script = (Unity_Purdue_Player)target;
        if (GUILayout.Button("ResetValues to Default"))
        {
            script.reset();
        }
    }
}

[CustomEditor(typeof(Unity_Purdue_Difficulty))]
[ExecuteAlways]
public class CustomInspector4 : Editor
{
    public override void OnInspectorGUI()
    {
        
        Unity_Purdue_Difficulty script = (Unity_Purdue_Difficulty)target;

        GUILayout.Label("Select a game mode:");

        bool jumpMode = script.gameModeJump;
        jumpMode = GUILayout.Toggle(jumpMode, "Jump Mode");
        if (jumpMode)
        {
            script.setGame(0);
        }

        bool dodgeMode = script.gameModeDodge;
        dodgeMode = GUILayout.Toggle(dodgeMode, "Dodge Mode");
        if (dodgeMode)
        {
            script.setGame(1);
        }

        //GUILayout.Label("To change graphics please turn the camera filter scripts\non/off manually in the cameras under the Cameras\ngame object.");
        GUILayout.Label("WARNING: Please do not change any values\nwhen the game is running.");
        DrawDefaultInspector();
        if (GUILayout.Button("ResetValues to Default"))
        {
            script.reset();
        }
        if (GUI.changed)
        {
            script.checkValue();
        }
    }
}

/***************************************************************************************************/

[CustomEditor(typeof(GameFlowFramework_PlayerCharacter))]
[ExecuteAlways]
public class CustomInspector5 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameFlowFramework_PlayerCharacter script = (GameFlowFramework_PlayerCharacter)target;

        GUILayout.Label("__________________________________________________________________");
        if (GUILayout.Button("Reset Values to Default"))
        {
            script.ResetValues();
        }
        if (GUI.changed)
        {
            script.UpdateCanvas();
        }
    }
}

[CustomEditor(typeof(GameFlowFramework_GameCamera))]
[ExecuteAlways]
public class CustomInspector6 : Editor
{
    public override void OnInspectorGUI()
    {
        GameFlowFramework_GameCamera script = (GameFlowFramework_GameCamera)target;

        GUILayout.Label("__________________________________________________________________");
        GUILayout.Label("Select a camera view mode:");

        bool make2D = script.cameraIs2D;
        make2D = GUILayout.Toggle(make2D, "2D");
        if (make2D)
        {
            script.SetMode(0);
        }

        bool make3D3rd = script.cameraIs3DThirdPerson;
        make3D3rd = GUILayout.Toggle(make3D3rd, "3D 3rd Person");
        if (make3D3rd)
        {
            script.SetMode(1);
        }

        bool make3DFPS = script.cameraIs3DFPS;
        make3DFPS = GUILayout.Toggle(make3DFPS, "3D FPS");
        if (make3DFPS)
        {
            script.SetMode(2);
        }

        DrawDefaultInspector();

        GUILayout.Label("__________________________________________________________________");
        if (GUILayout.Button("Reset Values to Default"))
        {
            script.ResetValues();
        }
    }
}

[CustomEditor(typeof(GameFlowFramework_Environment))]
[ExecuteAlways]
public class CustomInspector7 : Editor
{
    public override void OnInspectorGUI()
    {
        GameFlowFramework_Environment script = (GameFlowFramework_Environment)target;

        GUILayout.Label("__________________________________________________________________");
        GUILayout.Label("Select a game mode:");

        bool mode0 = script.gameMode0;
        mode0 = GUILayout.Toggle(mode0, "0: Jumper (Old)");
        GUILayout.Label("The very first game mode created.\nNot currently used for any purpose except for sweet memories.");
        if (mode0)
        {
            script.SetGameMode(0);
        }

        bool mode1 = script.gameMode1;
        mode1 = GUILayout.Toggle(mode1, "1:INFINITE_RUNNER");
        GUILayout.Label("Infinite patches. Difficulty based on patch priorities.");
        if (mode1)
        {
            script.SetGameMode(1);
        }

        bool mode2 = script.gameMode2;
        mode2 = GUILayout.Toggle(mode2, "2:BASIC_OPTIMIZATION");
        GUILayout.Label("User defined no. of patches. \nDifficulty based on basic optimization.");
        if (mode2)
        {
            script.SetGameMode(2);
        }

        bool mode3 = script.gameMode3;
        mode3 = GUILayout.Toggle(mode3, "3:SIMULATED_ANNEALING");
        GUILayout.Label("User defined no. of patches. \nDifficulty based on Simulated Annealing.");
        if (mode3)
        {
            script.SetGameMode(3);
        }

        bool mode4 = script.gameMode4;
        mode4 = GUILayout.Toggle(mode4, "4:MAKE_MY_OWN");
        GUILayout.Label("Design your own level with options to turn \nit into an infinite runner and more.");
        if (mode4)
        {
            script.SetGameMode(4);
        }

        DrawDefaultInspector();

        GUILayout.Label("__________________________________________________________________");
        if (GUILayout.Button("Reset Values to Default"))
        {
            script.ResetValues();
        }
        if (GUI.changed)
        {
            script.UpdateCanvas();
            script.CheckValues();
        }
    }
}

[CustomEditor(typeof(GameFlowFramework_WinLoseCondition))]
[ExecuteAlways]
public class CustomInspector8 : Editor
{
    public override void OnInspectorGUI()
    {
        GameFlowFramework_WinLoseCondition script = (GameFlowFramework_WinLoseCondition)target;
        DrawDefaultInspector();

        GUILayout.Label("__________________________________________________________________");
        if (GUILayout.Button("Reset Values to Default"))
        {
            script.ResetValues();
        }
        if (GUI.changed)
        {
            script.SetPlayerPrefs();
        }
    }
}

[CustomEditor(typeof(CanvasScript))]
[ExecuteAlways]
public class CustomInspector9 : Editor
{
    public override void OnInspectorGUI()
    {
        CanvasScript script = (CanvasScript)target;
        DrawDefaultInspector();

        /*
        if (GUI.changed)
        {
            script.updateUI();
        }
        */
    }
}

[CustomEditor(typeof(GameFlowFramework_ScriptReferencer))]
[ExecuteAlways]
public class CustomInspector10 : Editor
{
    public override void OnInspectorGUI()
    {
        GameFlowFramework_ScriptReferencer script = (GameFlowFramework_ScriptReferencer)target;

        GUILayout.Label("This script is used for referencing other scripts.");
        GUILayout.Label("__________________________________________________________________");
        DrawDefaultInspector();
        GUILayout.Label("__________________________________________________________________");
    }
}

[CustomEditor(typeof(GameFlowFramework_Questions))]
[ExecuteAlways]
public class CustomInspector11 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameFlowFramework_Questions script = (GameFlowFramework_Questions)target;

        GUILayout.Label("__________________________________________________________________");
        if (GUILayout.Button("Reset Values to Default"))
        {
            script.ResetValues();
        }
        if (GUI.changed)
        {
            //script.CheckQuestions();
        }
    }
}

[CustomEditor(typeof(GameFlowFramework_Web))]
[ExecuteAlways]
public class CustomInspector12 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameFlowFramework_Web script = (GameFlowFramework_Web)target;

        GUILayout.Label("__________________________________________________________________");
        if (GUILayout.Button("Reset Values to Default"))
        {
            script.ResetValues();
        }
    }
}

#endif

