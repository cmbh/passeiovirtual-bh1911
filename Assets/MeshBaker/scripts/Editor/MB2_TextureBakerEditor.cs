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

[CustomEditor(typeof(MB2_TextureBaker))]
public class MB2_TextureBakerEditor : Editor {
	
	MB2_TextureBakerEditorInternal tbe = new MB2_TextureBakerEditorInternal();
	
	public override void OnInspectorGUI(){
		tbe.DrawGUI((MB2_TextureBaker) target);	
	}

	public static void CreateCombinedMaterialAssets(MB2_TextureBaker target, string pth){
//		Debug.Log("CreateCombinedMaterialAssets= " + pth);
		MB2_TextureBaker mom = (MB2_TextureBaker) target;
		string baseName = Path.GetFileNameWithoutExtension(pth);
		string folderPath = pth.Substring(0,pth.Length - baseName.Length - 6);
		
		List<string> matNames = new List<string>();
		if (mom.doMultiMaterial){
			for (int i = 0; i < mom.resultMaterials.Length; i++){
				matNames.Add( folderPath +  baseName + "-mat" + i + ".mat" );
				AssetDatabase.CreateAsset(new Material(Shader.Find("Diffuse")), matNames[i]);
				mom.resultMaterials[i].combinedMaterial = (Material) AssetDatabase.LoadAssetAtPath(matNames[i],typeof(Material));
			}
		}else{
			matNames.Add( folderPath +  baseName + "-mat.mat" );
//			Debug.Log("mat " + matNames[0]);
			AssetDatabase.CreateAsset(new Material(Shader.Find("Diffuse")), matNames[0]);
			mom.resultMaterial = (Material) AssetDatabase.LoadAssetAtPath(matNames[0],typeof(Material));
		}
		//create the MB2_TextureBakeResults
		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MB2_TextureBakeResults>(),pth);
		mom.textureBakeResults = (MB2_TextureBakeResults) AssetDatabase.LoadAssetAtPath(pth, typeof(MB2_TextureBakeResults));
		AssetDatabase.Refresh();
	}	
}