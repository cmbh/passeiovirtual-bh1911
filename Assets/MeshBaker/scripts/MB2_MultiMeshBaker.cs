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

#if UNITY_EDITOR
	using UnityEditor;
#endif 


/// <summary>
/// Component that is an endless mesh. You don't need to worry about the 65k limit when adding meshes. It is like a List of combined meshes. Internally it manages
/// a collection of CombinedMeshes that are added and deleted as necessary. 
/// 
/// Note that this implementation does
/// not attempt to split meshes. Each mesh is added to one of the internal meshes as an atomic unit.
/// 
/// This class is a Component. It must be added to a GameObject to use it. It is a wrapper for MB2_MultiMeshCombiner which contains the same functionality but is not a component
/// so it can be instantiated like a normal class.
/// </summary>
public class MB2_MultiMeshBaker : MB2_MeshBakerCommon {	
		
	[HideInInspector] public MB2_MultiMeshCombiner meshCombiner = new MB2_MultiMeshCombiner();
	
	public override void ClearMesh(){
		_update_MB2_MeshCombiner();
		meshCombiner.ClearMesh();
	}
	public override void DestroyMesh(){
		_update_MB2_MeshCombiner();
		meshCombiner.DestroyMesh();
	}
	
	//todo could use this
//	public void BuildSceneMeshObject(){
//		if (resultSceneObject == null){
//			resultSceneObject = new GameObject("CombinedMesh-" + name);
//		}
//		_update_MB2_MeshCombiner();
////		meshCombiner.BuildSceneMeshObject();
//	}

	public override int GetNumObjectsInCombined(){
		return meshCombiner.GetNumObjectsInCombined();	
	}
	
	public override int GetNumVerticesFor(GameObject go){
		return meshCombiner.GetNumVerticesFor(go);
	}
	
	public override Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource, bool fixOutOfBoundUVs){
		if (resultSceneObject == null){
			resultSceneObject = new GameObject("CombinedMesh-" + name);	
		}
		_update_MB2_MeshCombiner();
		Mesh mesh = meshCombiner.AddDeleteGameObjects(gos,deleteGOs,disableRendererInSource,fixOutOfBoundUVs);		
		return mesh;
	}
	public override bool CombinedMeshContains(GameObject go){return meshCombiner.CombinedMeshContains(go);}
	public override void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true){
		_update_MB2_MeshCombiner();
		meshCombiner.UpdateGameObjects(gos,recalcBounds);
	}
	public override void Apply(){
		_update_MB2_MeshCombiner();
		meshCombiner.Apply();
	}
	
	public override void Apply(bool triangles,
					  bool vertices,
					  bool normals,
					  bool tangents,
					  bool uvs,
					  bool colors,
					  bool uv1,
					  bool uv2,
					  bool bones=false){
		_update_MB2_MeshCombiner();
		meshCombiner.Apply(triangles,vertices,normals,tangents,uvs,colors,uv1,uv2,bones);
	}

	/*
	public void UpdateSkinnedMeshApproximateBounds(){
		if (outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace){
			Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
			return;
		}
		if (resultSceneObject == null){
			Debug.LogWarning("Result Scene Object does not exist. No point in calling UpdateSkinnedMeshApproximateBounds.");
			return;			
		}
		SkinnedMeshRenderer smr = resultSceneObject.GetComponentInChildren<SkinnedMeshRenderer>();	
		if (smr == null){
			Debug.LogWarning("No SkinnedMeshRenderer on result scene object.");
			return;			
		}
		meshCombiner.UpdateSkinnedMeshApproximateBounds();
	}
	*/	
	
	public override void SaveMeshsToAssetDatabase(string folderPath,string newFileNameBase){
		meshCombiner.SaveMeshsToAssetDatabase(folderPath, newFileNameBase);
	}
	
	public override void RebuildPrefab(){
		if (renderType == MB_RenderType.skinnedMeshRenderer){
			Debug.LogWarning("Prefab will not be updated for skinned mesh. This is because all bones need to be included in the prefab for it to be usefull.");	
		} else {
			meshCombiner.RebuildPrefab(resultPrefab);
		}
	}
	
	void _update_MB2_MeshCombiner(){
		meshCombiner.name = name;
		meshCombiner.textureBakeResults = textureBakeResults;
		meshCombiner.resultSceneObject = resultSceneObject;
		meshCombiner.renderType = renderType;
		meshCombiner.outputOption = outputOption;
		meshCombiner.lightmapOption = lightmapOption;
		meshCombiner.doNorm = doNorm;
		meshCombiner.doTan = doTan;
		meshCombiner.doCol = doCol;	
		meshCombiner.doUV = doUV;
		meshCombiner.doUV1 = doUV1;		
	}
}
