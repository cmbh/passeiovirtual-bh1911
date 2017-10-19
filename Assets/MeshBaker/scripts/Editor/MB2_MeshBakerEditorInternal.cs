//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DigitalOpus.MB.Core;

using UnityEditor;

public class MB2_MeshBakerEditorInternal{
	//add option to exclude skinned mesh renderer and mesh renderer in filter
	//example scenes for multi material
	private static GUIContent
		outputOptoinsGUIContent = new GUIContent("Output", ""),
		openToolsWindowLabelContent = new GUIContent("Open Tools For Adding Objects", "Use these tools to find out what can be combined, discover problems with meshes, and quickly add objects."),
		renderTypeGUIContent = new GUIContent("Renderer","The type of renderer to add to the combined mesh."),
		objectsToCombineGUIContent = new GUIContent("Custom List Of Objects To Be Combined","You can add objects here that were not on the list in the MB2_TextureBaker as long as they use a material that is in the Material Bake Results"),
		textureBakeResultsGUIContent = new GUIContent("Material Bake Result","When materials are combined a MB2_TextureBakeResult Asset is generated. Drag that Asset to this field to use the combined material."),
		useTextureBakerObjsGUIContent = new GUIContent("Same As Texture Baker","Build a combined mesh using using the same list of objects that generated the Combined Material"),
		lightmappingOptionGUIContent = new GUIContent("Lightmapping UVs","preserve current lightmapping: Use this if all source objects are lightmapped and you want to preserve it. All source objects must use the same lightmap\n\n"+
																		 "generate new UV Layout: Use this if you want to bake a lightmap after the combined mesh has been generated\n\n" +
																		 "copy UV2 unchanged: Use this if UV2 is being used for something other than lightmaping.\n\n" + 
																		 "ignore UV2: A UV2 channel will not be generated for the combined mesh"),
		doNormGUIContent = new GUIContent("Include Normals"),
		doTanGUIContent = new GUIContent("Include Tangents"),
		doColGUIContent = new GUIContent("Include Colors"),
		doUVGUIContent = new GUIContent("Include UV"),
		doUV1GUIContent = new GUIContent("Include UV1");
	
	private SerializedObject meshBaker;
	private SerializedProperty  lightmappingOption, outputOptions, textureBakeResults, useObjsToMeshFromTexBaker, renderType, fixOutOfBoundsUVs, objsToMesh;
	private SerializedProperty doNorm, doTan, doUV, doUV1, doCol;
	bool showInstructions = false;
	bool showContainsReport = true;
	
	void _init (MB2_MeshBakerCommon target) {
		meshBaker = new SerializedObject(target);
		outputOptions = meshBaker.FindProperty("outputOption");
		objsToMesh = meshBaker.FindProperty("objsToMesh");
		renderType = meshBaker.FindProperty("renderType");
		useObjsToMeshFromTexBaker = meshBaker.FindProperty("useObjsToMeshFromTexBaker");
		textureBakeResults = meshBaker.FindProperty("textureBakeResults");
		lightmappingOption = meshBaker.FindProperty("lightmapOption");
		doNorm = meshBaker.FindProperty("doNorm");
		doTan = meshBaker.FindProperty("doTan");
		doUV = meshBaker.FindProperty("doUV");
		doUV1 = meshBaker.FindProperty("doUV1");
		doCol = meshBaker.FindProperty("doCol");
	}	
	
	public void OnInspectorGUI(MB2_MeshBakerCommon target){
		DrawGUI(target);
	}
	
	public void DrawGUI(MB2_MeshBakerCommon target){
		if (meshBaker == null){
			_init(target);
		}
		
		meshBaker.Update();

		showInstructions = EditorGUILayout.Foldout(showInstructions,"Instructions:");
		if (showInstructions){
			EditorGUILayout.HelpBox("1. Bake combined material(s).\n\n" +
									"2. If necessary set the 'Material Bake Results' field.\n\n" +
									"3. Add scene objects or prefabs to combine or check 'Same As Texture Baker'. For best results these should use the same shader as result material.\n\n" +
									"4. Select options and 'Bake'.\n\n" +
									"6. Look at warnings/errors in console. Decide if action needs to be taken.\n\n" +
									"7. (optional) Disable renderers in source objects.", UnityEditor.MessageType.None);
			
			EditorGUILayout.Separator();
		}				
		
		MB2_MeshBakerCommon mom = (MB2_MeshBakerCommon) target;
		
		EditorGUILayout.PropertyField(textureBakeResults, textureBakeResultsGUIContent);
		if (textureBakeResults.objectReferenceValue != null){
			showContainsReport = EditorGUILayout.Foldout(showContainsReport, "Shaders & Materials Contained");
			if (showContainsReport){
				EditorGUILayout.HelpBox(((MB2_TextureBakeResults)textureBakeResults.objectReferenceValue).GetDescription(), MessageType.Info);	
			}
		}
		
		EditorGUILayout.LabelField("Objects To Be Combined",EditorStyles.boldLabel);	
		if (mom.GetComponent<MB2_TextureBaker>() != null){
			EditorGUILayout.PropertyField(useObjsToMeshFromTexBaker, useTextureBakerObjsGUIContent);
		} else {
			useObjsToMeshFromTexBaker.boolValue = false;
		}
		
		if (!mom.useObjsToMeshFromTexBaker){
			
			if (GUILayout.Button(openToolsWindowLabelContent)){
				MB_MeshBakerEditorWindow mmWin = (MB_MeshBakerEditorWindow) EditorWindow.GetWindow(typeof(MB_MeshBakerEditorWindow));
				mmWin.target = (MB2_MeshBakerRoot) target;
			}	
			EditorGUILayout.PropertyField(objsToMesh,objectsToCombineGUIContent, true);
		}
		
		EditorGUILayout.LabelField("Output",EditorStyles.boldLabel);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(doNorm,doNormGUIContent);
		EditorGUILayout.PropertyField(doTan,doTanGUIContent);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(doUV,doUVGUIContent);
		EditorGUILayout.PropertyField(doUV1,doUV1GUIContent);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.PropertyField(doCol,doColGUIContent);	
		
		if (mom.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout){
			EditorGUILayout.HelpBox("Generating new lightmap UVs can split vertices which can push the number of vertices over the 64k limit.",MessageType.Warning);
		}
		EditorGUILayout.PropertyField(lightmappingOption,lightmappingOptionGUIContent);
		
		EditorGUILayout.PropertyField(outputOptions,outputOptoinsGUIContent);
		EditorGUILayout.PropertyField(renderType, renderTypeGUIContent);
		if (mom.outputOption == MB2_OutputOptions.bakeIntoSceneObject){
			mom.resultSceneObject = (GameObject) EditorGUILayout.ObjectField("Combined Mesh Object", mom.resultSceneObject, typeof(GameObject), true);
		} else if (mom.outputOption == MB2_OutputOptions.bakeIntoPrefab){
			mom.resultPrefab = (GameObject) EditorGUILayout.ObjectField("Combined Mesh Prefab", mom.resultPrefab, typeof(GameObject), true);			
		}
		
		if (GUILayout.Button("Bake")){
			bake(mom);
		}

		string enableRenderersLabel;
		bool disableRendererInSource = false;
		if (mom.objsToMesh.Count > 0){
			Renderer r = MB_Utility.GetRenderer(mom.objsToMesh[0]);
			if (r != null && r.enabled) disableRendererInSource = true;
		}
		if (disableRendererInSource){
			enableRenderersLabel = "Disable Renderers On Combined Objects";
		} else {
			enableRenderersLabel = "Enable Renderers On Combined Objects";
		}
		if (GUILayout.Button(enableRenderersLabel)){
			mom.EnableDisableSourceObjectRenderers(!disableRendererInSource);
		}	
		
		meshBaker.ApplyModifiedProperties();		
		meshBaker.SetIsDifferentCacheDirty();
	}
		
	public void updateProgressBar(string msg, float progress){
		EditorUtility.DisplayProgressBar("Combining Meshes", msg, progress);
	}
		
	void bake(MB2_MeshBakerCommon mom){
		try{
			if (mom.outputOption == MB2_OutputOptions.bakeIntoSceneObject){
				_bakeIntoCombined(mom, MB_OutputOptions.bakeIntoSceneObject);
			} else if (mom.outputOption == MB2_OutputOptions.bakeIntoPrefab){
				_bakeIntoCombined(mom, MB_OutputOptions.bakeIntoPrefab);
			} else {
				if (mom is MB2_MeshBaker){
					if (MB2_MeshCombiner.EVAL_VERSION){
						Debug.LogError("Bake Meshes In Place is disabled in the evaluation version.");
					} else {
						if (!MB2_MeshBakerRoot.doCombinedValidate(mom, MB_ObjsToCombineTypes.prefabOnly)) return;
							
						List<GameObject> objsToMesh = mom.objsToMesh;
						if (mom.useObjsToMeshFromTexBaker && mom.GetComponent<MB2_TextureBaker>() != null){
							objsToMesh = mom.GetComponent<MB2_TextureBaker>().objsToMesh;
						}						
						MB2_BakeInPlace.BakeMeshesInPlace(((MB2_MeshBaker)mom).meshCombiner, objsToMesh, updateProgressBar);
					}
				} else {
					Debug.LogError("Multi-mesh Baker components cannot be used for Bake In Place. Use an ordinary Mesh Baker object instead.");	
				}
			}
		} catch(Exception e){
			Debug.LogError(e);	
		} finally {
			EditorUtility.ClearProgressBar();
		}
	}
	
	void _bakeIntoCombined(MB2_MeshBakerCommon mom, MB_OutputOptions prefabOrSceneObject){
		if (MB2_MeshCombiner.EVAL_VERSION && prefabOrSceneObject == MB_OutputOptions.bakeIntoPrefab){
			Debug.LogError("Cannot BakeIntoPrefab with evaluation version.");
			return;
		}
		if (prefabOrSceneObject != MB_OutputOptions.bakeIntoPrefab && prefabOrSceneObject != MB_OutputOptions.bakeIntoSceneObject){
			Debug.LogError("Paramater prefabOrSceneObject must be bakeIntoPrefab or bakeIntoSceneObject");
			return;
		}
		//MB2_MeshBakerCommon mom = (MB2_MeshBakerCommon) target;
		MB2_TextureBaker tb = mom.GetComponent<MB2_TextureBaker>();
		if (mom.textureBakeResults == null && tb != null){
			Debug.Log("setting results");
			mom.textureBakeResults = tb.textureBakeResults;	
		}

		if (mom.useObjsToMeshFromTexBaker){
			if (tb != null){
				mom.objsToMesh.Clear();
				mom.objsToMesh.AddRange(tb.objsToMesh);
			}	
		}
		
		Mesh mesh;
		if (!MB2_MeshBakerRoot.doCombinedValidate(mom, MB_ObjsToCombineTypes.sceneObjOnly)) return;	
		if (prefabOrSceneObject == MB_OutputOptions.bakeIntoPrefab && 
			mom.resultPrefab == null){
			Debug.LogError("Need to set the Combined Mesh Prefab field.");
			return;
		}
		mom.ClearMesh();
		
		mesh = mom.AddDeleteGameObjects(mom.objsToMesh.ToArray(),null,false, true);
		if (mesh != null){
			mom.Apply();
			updateProgressBar("Created mesh saving assets",.6f);
			if (prefabOrSceneObject == MB_OutputOptions.bakeIntoSceneObject){
//				mom.BuildSceneMeshObject();
			} else if (prefabOrSceneObject == MB_OutputOptions.bakeIntoPrefab){
				string prefabPth = AssetDatabase.GetAssetPath(mom.resultPrefab);
				if (prefabPth == null || prefabPth.Length == 0){
					Debug.LogError("Could not save result to prefab. Result Prefab value is not an Asset.");
					return;
				}
				string baseName = Path.GetFileNameWithoutExtension(prefabPth);
				string folderPath = prefabPth.Substring(0,prefabPth.Length - baseName.Length - 7);		
				string newFilename = folderPath + baseName + "-mesh";
				mom.SaveMeshsToAssetDatabase(folderPath,newFilename);
				
				if (mom.renderType == MB_RenderType.skinnedMeshRenderer){
					Debug.LogWarning("Render type is skinned mesh renderer. " +
							"Can't create prefab until all bones have been added to the combined mesh object " + mom.resultPrefab + 
							" Add the bones then drag the combined mesh object to the prefab.");	
					
				} else {
					mom.RebuildPrefab();
				}
			} else {
				Debug.LogError("Unknown parameter");
			}
		}
		
	}	
}

