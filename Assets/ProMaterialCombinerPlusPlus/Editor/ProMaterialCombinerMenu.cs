/*
  Created by:
  Juan Sebastian Munoz Arango
  naruse@gmail.com
  All rights reserved
 */

namespace ProMaterialCombiner {
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;

	public sealed class ProMaterialCombinerMenu : EditorWindow {
	    private static GUIStyle smallTextStyle;
	    private static GUIStyle smallTextErrorStyle;
		private static GUIStyle smallTextWarningStyle;
        private static GUIStyle normalTextStyle;
        private static GUIStyle greenTextStyle;

        private bool flagObjNull = false;
        private Vector2 scrollPos = Vector2.zero;


	    private bool reuseTextures = true;
        private bool generatePrefab = true;
        private bool autoSelect = true;
        private bool batchMode = false;//flag for turning on/off batch mode.

	    private static string customAtlasName = "";
        private static CombinableObject combObj;

        private static List<CombinableObject> objsToBatch;//list of game objs for batch processing, not guaranteed that objs are optimizable.
        private static List<CombinableObject> filteredCombinableObjectsToBatchList;//guaranteed that all objs are optimizable.

	    private static ProMaterialCombinerMenu window;
	    [MenuItem("Window/Pro Material Combiner++")]
	    private static void Init() {
	        smallTextStyle = new GUIStyle();
	        smallTextStyle.fontSize = 9;

	        smallTextErrorStyle = new GUIStyle();
	        smallTextErrorStyle.normal.textColor = Color.red;
	        smallTextErrorStyle.fontSize = 9;

			smallTextWarningStyle = new GUIStyle();
			smallTextWarningStyle.normal.textColor = new Color(0.7725f, 0.5255f, 0);//~ dark yellow
			smallTextWarningStyle.fontSize = 9;

            normalTextStyle = new GUIStyle();
            greenTextStyle = new GUIStyle();
            greenTextStyle.normal.textColor = new Color(0, 0.25f, 0.15f);

	        window = (ProMaterialCombinerMenu) EditorWindow.GetWindow(typeof(ProMaterialCombinerMenu));
	        window.minSize = new Vector2(463, 200);
	        window.Show();

            combObj = new CombinableObject();
            objsToBatch = new List<CombinableObject>();
            filteredCombinableObjectsToBatchList = new List<CombinableObject>();
	    }

        private static void ReloadDataStructures() {
            Init();
            customAtlasName = "";
        }
        private bool NeedToReload() {
            if(window == null)
                return true;
            else
                return false;
        }

        private bool BatchModeOn() {
            return batchMode;
        }

        private void TurnBatchModeOff() {
            objsToBatch.Clear();
            batchMode = false;
            autoSelect = true;
        }

        private void AddToBatchList(GameObject[] arr) {
	        //dont include already optimized objects
	        List<GameObject> filteredArray = new List<GameObject>();
	        for(int i = 0; i < arr.Length; i++) {
	            if(!arr[i].name.Contains(Constants.OptimizedObjIdentifier))
	                filteredArray.Add(arr[i]);
	            else
	                Debug.LogWarning("Skipping " + arr[i].name + " game object as is already optimized.");
            }
            // lets check that the object we are going to include is not already in the objects to batch list.
            for(int i = 0; i < filteredArray.Count; i++) {
                bool objAlreadyOnList = false;
                for(int j = 0; j < objsToBatch.Count; j++) {
                    if(filteredArray[i].GetInstanceID() == objsToBatch[j].ObjectToCombine.GetInstanceID()) {
                        objAlreadyOnList = true;
                        Debug.LogWarning("Object: " + filteredArray[i].name + " is already on batch list, skipping...");
                        break;
                    }
                }
                if(!objAlreadyOnList) {
                    objsToBatch.Add(new CombinableObject(filteredArray[i]));
                }
            }
            FilterCombinableObjects();
        }
        //this function automatically updates from the objsToBatch list the filteredCombinableObjectsToBatchList List.
        private void FilterCombinableObjects() {
            filteredCombinableObjectsToBatchList.Clear();
            for(int i = 0; i < objsToBatch.Count; i++)
                if(objsToBatch[i].IsCorrectlyAssembled)
                    filteredCombinableObjectsToBatchList.Add(objsToBatch[i]);
        }

        GameObject objToCombine = null;
	    void OnGUI() {
            if(NeedToReload())
                ReloadDataStructures();
            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                    GUILayout.Label("Atlas name(Optional)", GUILayout.Width(window.position.width/4));
                    customAtlasName = GUILayout.TextField(customAtlasName, GUILayout.Width(window.position.width/4));
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                    if(BatchModeOn())
                        autoSelect = false;
                    GUI.enabled = !BatchModeOn();
                    autoSelect = GUILayout.Toggle(autoSelect, "Auto select");
                    GUI.enabled = true;
                    reuseTextures = GUILayout.Toggle(reuseTextures, "Reuse Textures", GUILayout.Width(window.position.width/4));
                    generatePrefab = GUILayout.Toggle(generatePrefab, "Generate prefab");
                GUILayout.EndVertical();

                Vector2 size = GUI.skin.GetStyle("Button").CalcSize(new GUIContent(""));
                GUILayout.BeginVertical();
                    if(!BatchModeOn()) {
                        if(GUILayout.Button("Batch Mode", GUILayout.Width(window.position.width/4*0.9f), GUILayout.Height(size.y * 1.80f))) {//turns batch mode on
                            batchMode = true;
                            return;
                        }
                    } else {
                        if(GUILayout.Button("Add selection\nfor batching", GUILayout.Width(window.position.width/4*0.9f), GUILayout.Height(size.y * 1.80f))) {
                            AddToBatchList(Selection.gameObjects);
                        }
                        if(GUILayout.Button("Clear objects")) {
                            objsToBatch.Clear();
                        }
                    }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                    if(BatchModeOn()) {//if we are in batch mode
                        if(GUILayout.Button("Add selection and\nchildren for batch", GUILayout.Width(window.position.width/4*0.95f), GUILayout.Height(size.y * 1.80f)) || autoSelect) {
                            GameObject[] selectedGameObjects = Selection.gameObjects;
                            List<GameObject> objsToAdd = new List<GameObject>();
                            for(int i = 0; i < selectedGameObjects.Length; i++) {
                                Transform[] selectedObjs = selectedGameObjects[i].GetComponentsInChildren<Transform>(true);
                                for(int j = 0; j < selectedObjs.Length; j++)
                                    objsToAdd.Add(selectedObjs[j].gameObject);
                            }
                            AddToBatchList(objsToAdd.ToArray());
                        }
                        if(GUILayout.Button("Batch Mode Off")) {
                            TurnBatchModeOff();
                        }
                    } else {
                        GUI.enabled = !autoSelect;
                        if(GUILayout.Button("Add selected\nobject", GUILayout.Width(window.position.width/4*0.95f), GUILayout.Height(size.y * 1.80f)) || autoSelect)
                            objToCombine = Selection.activeGameObject;
                        GUI.enabled = true;
                    }
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if(BatchModeOn()) {
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Objects On List: " + objsToBatch.Count + " Optimizable Objects: " + filteredCombinableObjectsToBatchList.Count + " out of " + objsToBatch.Count);
                GUILayout.EndHorizontal();
            } else {
                GUILayout.BeginHorizontal();
                    if(autoSelect) {
                        objToCombine = Selection.activeGameObject;
                        GUILayout.Label("Auto selected object: " + ((objToCombine == null) ? "none." : objToCombine.name));
                    } else {
                        objToCombine = EditorGUILayout.ObjectField("GameObject to combine:", objToCombine, typeof(GameObject), true,GUILayout.Width(window.position.width/2)) as GameObject;
                    }
                    // assign only once the combObj.ObjectToCombine as this is a render loop
                    if(objToCombine != null) {
                        if(combObj.ObjectToCombine == null || objToCombine.GetInstanceID() != combObj.ObjectToCombine.GetInstanceID()) {
                            combObj.ObjectToCombine = objToCombine;
                            flagObjNull = false;
                        }
                    } else if(!flagObjNull) {
                        combObj.ObjectToCombine = objToCombine;
                        flagObjNull = true;
                    }
                    /**************************************************************************/
                    GUILayout.BeginVertical();
                        GUILayout.Label(combObj.IntegrityLog[0], smallTextErrorStyle);
                        GUILayout.Label(combObj.IntegrityLog[1], smallTextErrorStyle);
                    GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            Rect combinableObjectGUIRect = new Rect(3, 75, window.position.width-6, window.position.height-40-78);
            GUILayout.BeginArea(combinableObjectGUIRect);
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(window.position.width - 6), GUILayout.Height(window.position.height-40-78));
                    if(BatchModeOn())
                        DrawBatchModeGUI();
                    else
                        combObj.DrawGUI(reuseTextures);
                GUI.EndScrollView();
            GUILayout.EndArea();

            GUI.enabled = BatchModeOn() ? AtLeastOneObjectFromBatchCanBeCombined() : combObj.IsCorrectlyAssembled;
            GUILayout.BeginArea(new Rect(3, window.position.height - 40, window.position.width-6, 40));
                if(GUILayout.Button((BatchModeOn() ? "Batch " : "") + "Combine", GUILayout.Height(38))) {
                    if(BatchModeOn()) {
                        string progressBarInfo = "Please wait...";
                        float pace = 1/(float)filteredCombinableObjectsToBatchList.Count;
                        float progress = pace;
                        for(int i = 0; i < filteredCombinableObjectsToBatchList.Count; i++) {
                            EditorUtility.DisplayProgressBar("Optimization in progress... ", progressBarInfo, progress);
                            progressBarInfo = "Combining: " + filteredCombinableObjectsToBatchList[i].ObjectToCombine.name;
                            filteredCombinableObjectsToBatchList[i].CombineObject(customAtlasName, reuseTextures, generatePrefab);
                            filteredCombinableObjectsToBatchList[i].ObjectToCombine.SetActive(false);
                            progress += pace;
                        }
                        EditorUtility.ClearProgressBar();
                    } else {
                        combObj.CombineObject(customAtlasName, reuseTextures, generatePrefab);
                        combObj.ObjectToCombine.SetActive(false);
                    }

                }
            GUILayout.EndArea();
            GUI.enabled = true;
	    }

        private void DrawBatchModeGUI() {
            for(int i = 0; i < objsToBatch.Count; i++) {
                GUILayout.BeginHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label((i+1).ToString() + ": " + objsToBatch[i].ObjectToCombine.name,
                                    objsToBatch[i].IsCorrectlyAssembled ? greenTextStyle : normalTextStyle,
                                    GUILayout.Width(100));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginVertical();
                    if(objsToBatch[i].IsCorrectlyAssembled) {
                        int atlasSize = objsToBatch[i].GetAproxAtlasSize(reuseTextures);
                        GUILayout.Label("Atlas size: ~(" + atlasSize + "x" + atlasSize +")+2.5%+", smallTextStyle, GUILayout.MinWidth(200));
                        GUILayout.Label("Shader: " + objsToBatch[i].GetShaderUsed() + ".   Materials: " + objsToBatch[i].GetMaterialsToCombineCount(), smallTextStyle, GUILayout.MinWidth(200));
                    } else {
                        GUILayout.Label(objsToBatch[i].IntegrityLog[0], smallTextErrorStyle, GUILayout.MinWidth(200));
                        GUILayout.Label(objsToBatch[i].IntegrityLog[1],  smallTextErrorStyle, GUILayout.MinWidth(200));
                    }
                    GUILayout.EndVertical();
                    if(GUILayout.Button("X", GUILayout.Width(20))) {
                        objsToBatch.RemoveAt(i);
                        FilterCombinableObjects();
                    }
                GUILayout.EndHorizontal();
            }
        }

        private bool AtLeastOneObjectFromBatchCanBeCombined() {
            if(filteredCombinableObjectsToBatchList.Count > 0)
                return true;
            else
                return false;
        }

	    private void OnInspectorUpdate() {
	        Repaint();
	    }

	    private void OnDidOpenScene() {
            if(objsToBatch != null) {
                objsToBatch.Clear();
                filteredCombinableObjectsToBatchList.Clear();
            }
            if(combObj != null)
            combObj.ObjectToCombine = null;
	    }
	}
}