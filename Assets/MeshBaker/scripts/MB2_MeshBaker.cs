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
/// Component that manages a single combined mesh. 
/// 
/// This class is a Component. It must be added to a GameObject to use it. It is a wrapper for MB2_MeshCombiner which contains the same functionality but is not a component
/// so it can be instantiated like a normal class.
/// </summary>
public class MB2_MeshBaker : MB2_MeshBakerCommon {	
	
	[HideInInspector] public MB2_MeshCombiner meshCombiner = new MB2_MeshCombiner();

	public bool doUV2(){return meshCombiner.doUV2();}
	public Mesh GetMesh(){return meshCombiner.GetMesh();}
	public int GetLightmapIndex(){return meshCombiner.GetLightmapIndex();}
	
	public override void ClearMesh(){
		_update_MB2_MeshCombiner();
		meshCombiner.ClearMesh();
	}
	public override void DestroyMesh(){
		_update_MB2_MeshCombiner();
		meshCombiner.DestroyMesh();
	}
	
	public void BuildSceneMeshObject(){
		if (resultSceneObject == null){
			resultSceneObject = new GameObject("CombinedMesh-" + name);
		}
		_update_MB2_MeshCombiner();
		meshCombiner.buildSceneMeshObject(resultSceneObject, meshCombiner.GetMesh());
		_update_MB2_MeshCombiner();
	}
	
	public override int GetNumObjectsInCombined(){
		return meshCombiner.GetNumObjectsInCombined();	
	}
	
	public override int GetNumVerticesFor(GameObject go){
		return meshCombiner.GetNumVerticesFor(go);
	}
	
	public override Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource, bool fixOutOfBoundUVs){
		if ((meshCombiner.outputOption == MB2_OutputOptions.bakeIntoSceneObject || (meshCombiner.outputOption == MB2_OutputOptions.bakeIntoPrefab && meshCombiner.renderType == MB_RenderType.skinnedMeshRenderer) )) BuildSceneMeshObject();
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
	
	[Obsolete("ApplyAll is deprecated, please use Apply instead.")]
	public void ApplyAll(){
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
	
	public override void SaveMeshsToAssetDatabase(string folderPath,string newFileNameBase){
		meshCombiner.SaveMeshsToAssetDatabase(folderPath, newFileNameBase);
	}
	
	public override void RebuildPrefab(){
		meshCombiner.RebuildPrefab(resultPrefab);
	}
	
	void _update_MB2_MeshCombiner(){
		meshCombiner.name = name;
		if (resultSceneObject != null){
			meshCombiner.targetRenderer = resultSceneObject.GetComponentInChildren<Renderer>();
		} else {
			meshCombiner.targetRenderer = null;
		}
		meshCombiner.textureBakeResults = textureBakeResults;
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
