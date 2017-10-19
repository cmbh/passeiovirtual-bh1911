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

[CustomEditor(typeof(MB2_MeshBaker))]
public class MB2_MeshBakerEditor : Editor {
	MB2_MeshBakerEditorInternal mbe = new MB2_MeshBakerEditorInternal();
	[MenuItem("GameObject/Create Other/Mesh Baker/Mesh And Material Baker")]
	public static GameObject CreateNewMeshBaker(){
		MB2_MeshBaker[] mbs = (MB2_MeshBaker[]) Editor.FindObjectsOfType(typeof(MB2_MeshBaker));
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
		GameObject nmb = new GameObject("MeshBaker" + largest);
		nmb.transform.position = Vector3.zero;
		nmb.AddComponent<MB2_TextureBaker>();
		nmb.AddComponent<MB2_MeshBaker>();
		return nmb.gameObject;
	}
	
//	void OnEnable () {
//		mbe.OnEnable((MB2_MeshBakerCommon) target);
//	}
	
	public override void OnInspectorGUI(){
		mbe.OnInspectorGUI((MB2_MeshBakerCommon) target);
	}
}
