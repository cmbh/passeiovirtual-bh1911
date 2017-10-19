using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;

namespace DigitalOpus.MB.Core{
	
public class MB2_BakeInPlace {
	public static void BakeMeshesInPlace(MB2_MeshCombiner mom, List<GameObject> objsToMesh, ProgressUpdateDelegate updateProgressBar){
#if UNITY_EDITOR
		Mesh mesh;

//		if (!MB_Utility.doCombinedValidate(mom, MB_ObjsToCombineTypes.prefabOnly)) return;
		
//		List<GameObject> objsToMesh = mom.objsToMesh;
//		if (mom.useObjsToMeshFromTexBaker && mom.GetComponent<MB2_TextureBaker>() != null){
//			objsToMesh = mom.GetComponent<MB2_TextureBaker>().objsToMesh;
//		}
		
		mom.DestroyMesh();
		
		GameObject[] objs = new GameObject[1];
		List<string> usedNames = new List<string>();
		MB_RenderType originalRenderType = mom.renderType;
		for (int i = 0; i < objsToMesh.Count; i++){
			if (objsToMesh[i] == null){
				Debug.LogError("The " + i + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
				return;					
			}
			objs[0] = objsToMesh[i];
			Renderer r = MB_Utility.GetRenderer(objsToMesh[i]);
			if (r is SkinnedMeshRenderer){
				mom.renderType = MB_RenderType.skinnedMeshRenderer;	
			} else {
				mom.renderType = MB_RenderType.meshRenderer;
			}
			mesh = mom.AddDeleteGameObjects(objs,null,false);
			if (mesh != null){
				//mom.ApplyAll();
				mom.Apply();
				Mesh mf = MB_Utility.GetMesh(objs[0]);
				if (mf != null){
					string baseName, folderPath, newFilename;
					string pth = AssetDatabase.GetAssetPath(mf);
					if (pth != null && pth.Length != 0){
						baseName = Path.GetFileNameWithoutExtension(pth) + "_" + objs[0].name + "_MB";
						folderPath = Path.GetDirectoryName(pth); 		
					} else { //try to get the name from prefab
						pth = AssetDatabase.GetAssetPath(objs[0]); //get prefab name
						if (pth != null && pth.Length != 0){
							baseName = Path.GetFileNameWithoutExtension(pth) + "_" + objs[0].name + "_MB";
							folderPath = Path.GetDirectoryName(pth);		
						} else { //save in root
							baseName = objs[0].name + "mesh_MB";
							folderPath = "Assets";
						}
					}
					//make name unique
					newFilename = Path.Combine(folderPath, baseName + ".asset");
					int j = 0;
					while(usedNames.Contains(newFilename)){
						newFilename = Path.Combine(folderPath, baseName + j + ".asset");
						j++;
					}
					usedNames.Add(newFilename);
					updateProgressBar("Created mesh saving mesh on " + objs[0].name + " to asset " + newFilename,.6f);
					if (newFilename != null && newFilename.Length != 0){
						Debug.Log("Creating mesh for " + objs[0].name + " with adjusted UVs at: " + newFilename);
						AssetDatabase.CreateAsset(mesh,  newFilename);
					} else {
						Debug.LogWarning("Could not save mesh for " + objs[0].name);	
					}
				}
			}
			mom.DestroyMesh();
		}
		mom.renderType = originalRenderType;
#endif	
	}
	
}
}