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

using UnityEditor;

public class MB2_TextureBakerEditorInternal{
	//add option to exclude skinned mesh renderer and mesh renderer in filter
	//example scenes for multi material
	
	private static GUIContent insertContent = new GUIContent("+", "add a material");
	private static GUIContent deleteContent = new GUIContent("-", "delete a material");
	private static GUILayoutOption buttonWidth = GUILayout.MaxWidth(20f);

	private SerializedObject textureBaker;
	private SerializedProperty textureBakeResults, maxTilingBakeSize, doMultiMaterial, fixOutOfBoundsUVs, resultMaterial, resultMaterials, atlasPadding, resizePowerOfTwoTextures, customShaderPropNames, objsToMesh;
	
	bool resultMaterialsFoldout = true;
	bool showInstructions = false;
	bool showContainsReport = true;
	
	private static GUIContent
		createPrefabAndMaterialLabelContent = new GUIContent("Create Empty Assets For Combined Material", "Creates a material asset and a 'MB2_TextureBakeResult' asset. You should set the shader on the material. Mesh Baker uses the Texture properties on the material to decide what atlases need to be created. The MB2_TextureBakeResult asset should be used in the 'Material Bake Result' field."),
		openToolsWindowLabelContent = new GUIContent("Open Tools For Adding Objects", "Use these tools to find out what can be combined, discover possible problems with meshes, and quickly add objects."),
		fixOutOfBoundsGUIContent = new GUIContent("Fix Out-Of-Bounds UVs", "If mesh has uvs outside the range 0,1 uvs will be scaled so they are in 0,1 range. Textures will have tiling baked."),
		resizePowerOfTwoGUIContent = new GUIContent("Resize Power-Of-Two Textures", "Shrinks textures so they have a clear border of width 'Atlas Padding' around them. Improves texture packing efficiency."),
		customShaderPropertyNamesGUIContent = new GUIContent("Custom Shader Propert Names", "Mesh Baker has a list of common texture properties that it looks for in shaders to generate atlases. Custom shaders may have texture properties not on this list. Add them here and Meshbaker will generate atlases for them."),
		combinedMaterialsGUIContent = new GUIContent("Combined Materials", "Use the +/- buttons to add multiple combined materials. You will also need to specify which materials on the source objects map to each combined material."),
		maxTilingBakeSizeGUIContent = new GUIContent("Max Tiling Bake Size","This is the maximum size tiling textures will be baked to."),
		objectsToCombineGUIContent = new GUIContent("Objects To Be Combined","These can be prefabs or scene objects. They must be game objects with Renderer components, not the parent objects. Materials on these objects will baked into the combined material(s)"),
		textureBakeResultsGUIContent = new GUIContent("Material Bake Result","This asset contains a mapping of materials to UV rectangles in the atlases. It is needed to create combined meshes or adjust meshes so they can use the combined material(s). Create it using 'Create Empty Assets For Combined Material'. Drag it to the 'Material Bake Result' field to use it.");
	
	[MenuItem("GameObject/Create Other/Mesh Baker/Material Baker")]
	public static void CreateNewTextureBaker(){
		MB2_TextureBaker[] mbs = (MB2_TextureBaker[]) Editor.FindObjectsOfType(typeof(MB2_TextureBaker));
    	Regex regex = new Regex(@"(\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		int largest = 0;
		try{
			for (int i = 0; i < mbs.Length; i++){
				Match match = regex.Match(mbs[i].name);
				if (match.Success){
					int val = Convert.ToInt32(match.Groups[1].Value);
					if (val >= largest)
						largest = val + 1;
				}
			}
		} catch(Exception e){
			if (e == null) e = null; //Do nothing supress compiler warning
		}
		GameObject nmb = new GameObject("MaterialBaker" + largest);
		nmb.transform.position = Vector3.zero;
		nmb.AddComponent<MB2_TextureBaker>();
	}

	void _init(MB2_TextureBaker target) {
		textureBaker = new SerializedObject(target);
		doMultiMaterial = textureBaker.FindProperty("doMultiMaterial");
		fixOutOfBoundsUVs = textureBaker.FindProperty("fixOutOfBoundsUVs");
		resultMaterial = textureBaker.FindProperty("resultMaterial");
		resultMaterials = textureBaker.FindProperty("resultMaterials");
		atlasPadding = textureBaker.FindProperty("atlasPadding");
		resizePowerOfTwoTextures = textureBaker.FindProperty("resizePowerOfTwoTextures");
		customShaderPropNames = textureBaker.FindProperty("customShaderPropNames");
		objsToMesh = textureBaker.FindProperty("objsToMesh");
		maxTilingBakeSize = textureBaker.FindProperty("maxTilingBakeSize");
		textureBakeResults = textureBaker.FindProperty("textureBakeResults");
	}	
	
	public void DrawGUI(MB2_TextureBaker mom){
		if (textureBaker == null){
			_init(mom);
		}
		
		textureBaker.Update();

		showInstructions = EditorGUILayout.Foldout(showInstructions,"Instructions:");
		if (showInstructions){
			EditorGUILayout.HelpBox("1. Create Empty Assets For Combined Material(s)\n\n" +
									"2. Select shader on result material(s).\n\n" +
									"3. Add scene objects or prefabs to combine. For best results these should use the same shader as result material.\n\n" +
									"4. Bake materials into combined material(s).\n\n" +
									"5. Look at warnings/errors in console. Decide if action needs to be taken.\n\n" +
									"6. You are now ready to build combined meshs or adjust meshes to use the combined material(s).", UnityEditor.MessageType.None);
			
		}				
		
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Output",EditorStyles.boldLabel);
		if (GUILayout.Button(createPrefabAndMaterialLabelContent)){
			string newPrefabPath = EditorUtility.SaveFilePanelInProject("Asset name", "", "asset", "Enter a name for the baked texture results");
			if (newPrefabPath != null){
				MB2_TextureBakerEditor.CreateCombinedMaterialAssets(mom, newPrefabPath);
			}
		}	
		EditorGUILayout.PropertyField(textureBakeResults, textureBakeResultsGUIContent);
		if (textureBakeResults.objectReferenceValue != null){
			showContainsReport = EditorGUILayout.Foldout(showContainsReport, "Shaders & Materials Contained");
			if (showContainsReport){
				EditorGUILayout.HelpBox(((MB2_TextureBakeResults)textureBakeResults.objectReferenceValue).GetDescription(), MessageType.Info);	
			}
		}
		EditorGUILayout.PropertyField(doMultiMaterial,new GUIContent("Multiple Combined Materials"));		
		
		if (mom.doMultiMaterial){
			EditorGUILayout.LabelField("Source Material To Combined Mapping",EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			resultMaterialsFoldout = EditorGUILayout.Foldout(resultMaterialsFoldout, combinedMaterialsGUIContent);
			if(GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, buttonWidth)){
				if (resultMaterials.arraySize == 0){
					mom.resultMaterials = new MB_MultiMaterial[1];	
				} else {
					resultMaterials.InsertArrayElementAtIndex(resultMaterials.arraySize-1);
				}
			}
			if(GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth)){
				resultMaterials.DeleteArrayElementAtIndex(resultMaterials.arraySize-1);
			}			
			EditorGUILayout.EndHorizontal();
			if (resultMaterialsFoldout){
				for(int i = 0; i < resultMaterials.arraySize; i++){
					EditorGUILayout.Separator();
					EditorGUILayout.LabelField("---------- submesh:" + i,EditorStyles.boldLabel);
					EditorGUILayout.Separator();
					SerializedProperty resMat = resultMaterials.GetArrayElementAtIndex(i);
					EditorGUILayout.PropertyField(resMat.FindPropertyRelative("combinedMaterial"));
					SerializedProperty sourceMats = resMat.FindPropertyRelative("sourceMaterials");
					EditorGUILayout.PropertyField(sourceMats,true);
				}
			}
			
		} else {			
			EditorGUILayout.PropertyField(resultMaterial,new GUIContent("Combined Mesh Material"));
		}		

		EditorGUILayout.Separator();		
		EditorGUILayout.LabelField("Objects To Be Combined",EditorStyles.boldLabel);	
		if (GUILayout.Button(openToolsWindowLabelContent)){
			MB_MeshBakerEditorWindow mmWin = (MB_MeshBakerEditorWindow) EditorWindow.GetWindow(typeof(MB_MeshBakerEditorWindow));
			mmWin.target = (MB2_MeshBakerRoot) mom;
		}	
		EditorGUILayout.PropertyField(objsToMesh,objectsToCombineGUIContent, true);		
		
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Material Bake Options",EditorStyles.boldLabel);		
		EditorGUILayout.PropertyField(atlasPadding,new GUIContent("Atlas Padding"));
		EditorGUILayout.PropertyField(resizePowerOfTwoTextures, resizePowerOfTwoGUIContent);
		EditorGUILayout.PropertyField(customShaderPropNames,customShaderPropertyNamesGUIContent,true);
		EditorGUILayout.PropertyField(maxTilingBakeSize, maxTilingBakeSizeGUIContent);
		EditorGUILayout.PropertyField(fixOutOfBoundsUVs,fixOutOfBoundsGUIContent);		
		
		EditorGUILayout.Separator();				
		if (GUILayout.Button("Bake Materials Into Combined Material")){
			mom.CreateAndSaveAtlases(updateProgressBar, System.IO.File.WriteAllBytes);
			EditorUtility.ClearProgressBar();
			EditorUtility.SetDirty(mom.textureBakeResults);
		}
		textureBaker.ApplyModifiedProperties();		
		textureBaker.SetIsDifferentCacheDirty();
	}
		
	public void updateProgressBar(string msg, float progress){
		EditorUtility.DisplayProgressBar("Combining Meshes", msg, progress);
	}
}
