//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;

public enum MB_ObjsToCombineTypes{
	prefabOnly,
	sceneObjOnly,
	dontCare
}

/// <summary>
/// Root class of all the baking Components
/// </summary>
public class MB2_MeshBakerRoot : MonoBehaviour {
	[HideInInspector] public MB2_TextureBakeResults textureBakeResults; //todo validate that is same on texture baker and meshbaker
	
	public virtual List<GameObject> GetObjectsToCombine(){
		return null;	
	}

	
	public static bool doCombinedValidate(MB2_MeshBakerRoot mom, MB_ObjsToCombineTypes objToCombineType){
//		MB2_MeshBaker mom = (MB2_MeshBaker) target;
		
		if (mom.textureBakeResults == null){
			Debug.LogError("Need to set textureBakeResults");
			return false;
		}
		if (!(mom is MB2_TextureBaker)){
			MB2_TextureBaker tb = mom.GetComponent<MB2_TextureBaker>();
			if (tb != null && tb.textureBakeResults != mom.textureBakeResults){
				Debug.LogWarning("textureBakeResults on this component is not the same as the textureBakeResults on the MB2_TextureBaker.");
			}
		}
		
		List<GameObject> objsToMesh = mom.GetObjectsToCombine();
		for (int i = 0; i < objsToMesh.Count; i++){
			GameObject go = objsToMesh[i];
			if (go == null){
				Debug.LogError("The list of objects to combine contains a null at position." + i + " Select and use [shift] delete to remove");
				return false;					
			}
			for (int j = i + 1; j < objsToMesh.Count; j++){
				if (objsToMesh[i] == objsToMesh[j]){
					Debug.LogError("The list of objects to combine contains duplicates.");
					return false;	
				}
			}
			if (MB_Utility.GetGOMaterials(go) == null){
				Debug.LogError("Object " + go + " in the list of objects to be combined does not have a material");
				return false;
			}
			if (MB_Utility.GetMesh(go) == null){
				Debug.LogError("Object " + go + " in the list of objects to be combined does not have a mesh");
				return false;
			}			
		}
		
		if (mom.textureBakeResults.doMultiMaterial){
			if (!validateSubmeshOverlap(mom)){//only warns currently
				return false;
			}
		}
		List<GameObject> objs = objsToMesh;
		if (mom is MB2_MeshBaker){
			MB2_TextureBaker tb = mom.GetComponent<MB2_TextureBaker>();
			if (((MB2_MeshBaker)mom).useObjsToMeshFromTexBaker && tb != null) objs = tb.objsToMesh; 
			if (objs == null || objs.Count == 0){
				Debug.LogError("No meshes to combine. Please assign some meshes to combine.");
				return false;
			}
		}

#if UNITY_EDITOR		
		for (int i = 0; i < objsToMesh.Count; i++){
			UnityEditor.PrefabType pt = UnityEditor.PrefabUtility.GetPrefabType(objsToMesh[i]);
			if (pt == UnityEditor.PrefabType.None ||
				pt == UnityEditor.PrefabType.PrefabInstance || 
				pt == UnityEditor.PrefabType.ModelPrefabInstance || 
				pt == UnityEditor.PrefabType.DisconnectedPrefabInstance ||
				pt == UnityEditor.PrefabType.DisconnectedModelPrefabInstance){
				// these are scene objects
				if (objToCombineType == MB_ObjsToCombineTypes.prefabOnly){
					Debug.LogWarning("The list of objects to combine contains scene objects. You probably want prefabs." + objsToMesh[i] + " is a scene object");	
				}
			} else if (objToCombineType == MB_ObjsToCombineTypes.sceneObjOnly){
				//these are prefabs
				Debug.LogWarning("The list of objects to combine contains prefab objects. You probably want scene objects." + objsToMesh[i] + " is a prefab object");
			}
		}
#endif
		return true;
	}

	static bool validateSubmeshOverlap(MB2_MeshBakerRoot mom){
		List<GameObject> objsToMesh = mom.GetObjectsToCombine();
		for (int i = 0; i < objsToMesh.Count; i++){
			Mesh m = MB_Utility.GetMesh(objsToMesh[i]);
			if (MB_Utility.doSubmeshesShareVertsOrTris(m) != 0){
				Debug.LogError("Object " + objsToMesh[i] + " in the list of objects to combine has overlapping submeshes (submeshes share vertices). If you are using multiple materials then this object can only be combined with objects that use the exact same set of textures (each atlas contains one texture). There may be other undesirable side affects as well. Mesh Master, available in the asset store can fix overlapping submeshes.");	
				return true;
			}
		}
		return true;
	}	
}

