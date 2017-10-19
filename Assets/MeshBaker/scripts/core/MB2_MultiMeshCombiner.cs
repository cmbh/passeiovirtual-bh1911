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

namespace DigitalOpus.MB.Core{
	
/// <summary>
/// This class is an endless mesh. You don't need to worry about the 65k limit when adding meshes. It is like a List of combined meshes. Internally it manages
/// a collection of MB2_MeshComber objects to which meshes added and deleted as necessary. 
/// 
/// Note that this implementation does
/// not attempt to split meshes. Each mesh is added to one of the internal meshes as an atomic unit.
/// 
/// This class is not a Component so it can be instantiated and used like a regular C Sharp class.
/// </summary>
[System.Serializable]
public class MB2_MultiMeshCombiner {
	
	static bool VERBOSE = false;

	static GameObject[] empty = new GameObject[0];
	
	Dictionary<GameObject,CombinedMesh> obj2MeshCombinerMap = new Dictionary<GameObject, CombinedMesh>();	
	List<CombinedMesh> meshCombiners = new List<CombinedMesh>();
	
	[System.Serializable]
	public class CombinedMesh{
		public MB2_MeshCombiner combinedMesh;
		public int extraSpace = -1;
		public int numVertsInListToDelete = 0;
		public int numVertsInListToAdd = 0;
		public List<GameObject> gosToAdd;
		public List<GameObject> gosToDelete;
		public List<GameObject> gosToUpdate;
		public bool isDirty = false; //needs apply
		
		public CombinedMesh(int maxNumVertsInMesh){
		 	combinedMesh = new MB2_MeshCombiner();
		 	extraSpace = maxNumVertsInMesh;
			numVertsInListToDelete = 0;
			numVertsInListToAdd = 0;
		 	gosToAdd = new List<GameObject>();
		 	gosToDelete = new List<GameObject>();
			gosToUpdate = new List<GameObject>();
		}
		
		public bool isEmpty(){
			List<GameObject> obsIn = new List<GameObject>();
			obsIn.AddRange(combinedMesh.GetObjectsInCombined());
			for (int i = 0; i < gosToDelete.Count; i++){
				obsIn.Remove(gosToDelete[i]);	
			}
			if (obsIn.Count == 0) return true;
			return false;
		}
	}	

	[SerializeField] int _maxVertsInMesh = 65535;	
	public int maxVertsInMesh { 
		get{return _maxVertsInMesh;}
		set{
			if (obj2MeshCombinerMap.Count > 0){
				Debug.LogError("Can't set the max verts in meshes once there are objects in the mesh.");
				return;
			} else {
				_maxVertsInMesh = value;
			}
		}
	}
	
	[SerializeField] string __name;
	public string name { 
		get{return __name;}
		set{__name = value;}
	}
	
	[SerializeField] MB2_TextureBakeResults __textureBakeResults;
	public MB2_TextureBakeResults textureBakeResults { 
		get{return __textureBakeResults;} 
		set{__textureBakeResults = value;}
	}
	
	[SerializeField] GameObject __resultSceneObject;
	public GameObject resultSceneObject { 
		get{return __resultSceneObject;}
		set{__resultSceneObject = value;} 
	}
	
	[SerializeField] MB_RenderType __renderType;
	public MB_RenderType renderType { 
		get{return __renderType;} 
		set{__renderType = value;} 
	}
	
	[SerializeField] MB2_OutputOptions __outputOption;
	public MB2_OutputOptions outputOption { 
		get{return __outputOption;} 
		set{__outputOption = value;} 
	}

	[SerializeField] MB2_LightmapOptions __lightmapOption;
	public MB2_LightmapOptions lightmapOption { 
		get{return __lightmapOption;} 
		set{
			if (obj2MeshCombinerMap.Count > 0 && __lightmapOption != value){
				Debug.LogWarning("Can't change lightmap option once objects are in the combined mesh.");	
			}
			__lightmapOption = value;
		} 
	}
	
	[SerializeField] bool __doNorm;
	public bool doNorm { 
		get{return __doNorm;} 
		set{__doNorm = value;} 
	}
	
	[SerializeField] bool __doTan;
	public bool doTan { 
		get{return __doTan;} 
		set{__doTan = value;}
	}
	
	[SerializeField] bool __doCol;
	public bool doCol { 
		get{return __doCol;} 
		set{__doCol = value;}
	}
	
	[SerializeField] bool __doUV;
	public bool doUV { 
		get{return __doUV;} 
		set{__doUV = value;}
	}
	
	[SerializeField] bool __doUV1;
	public bool doUV1 { 
		get{return __doUV1;} 
		set{__doUV1 = value;}
	}		
	
	public int GetNumObjectsInCombined(){
		return obj2MeshCombinerMap.Count;		
	}
	
	public int GetNumVerticesFor(GameObject go){
		CombinedMesh c = null;
		if (obj2MeshCombinerMap.TryGetValue(go,out c)){
			return c.combinedMesh.GetNumVerticesFor(go);
		} else {
			return -1;
		}
	}
	
	public List<GameObject> GetObjectsInCombined(){ //todo look at getting from keys
		List<GameObject> allObjs = new List<GameObject>();
		for (int i = 0; i < meshCombiners.Count; i++){
			allObjs.AddRange(meshCombiners[i].combinedMesh.GetObjectsInCombined());
		}
		return allObjs;
	}
	
	 public int GetLightmapIndex(){ //todo check that all meshcombiners use same lightmap index
		 if (meshCombiners.Count > 0) return meshCombiners[0].combinedMesh.GetLightmapIndex();
		 return -1;
	 }
		
	public bool CombinedMeshContains(GameObject go){
		return obj2MeshCombinerMap.ContainsKey(go);	
	}
	
//	public void BuildSceneMeshObject(){
//		Debug.Log("num combiners " + meshCombiners.Count + " num children " + meshCombiners[0].combinedMesh.resultSceneObject.transform.childCount);
//		for (int i = 0; i < meshCombiners.Count; i++){
//			meshCombiners[i].combinedMesh.BuildSceneMeshObject();
//		}
//		Debug.Log("num combiners " + meshCombiners.Count + " num children " + meshCombiners[0].combinedMesh.resultSceneObject.transform.childCount);		
//	}
		
	bool _validateTextureBakeResults(){
		if (textureBakeResults == null){
			Debug.LogError("Material Bake Results is null. Can't combine meshes.");	
			return false;
		}
		if (textureBakeResults.materials == null || textureBakeResults.materials.Length == 0){
			Debug.LogError("Material Bake Results has no materials in material to uvRect map. Try baking materials. Can't combine meshes.");	
			return false;			
		}
		if (textureBakeResults.doMultiMaterial){
			if (textureBakeResults.resultMaterials == null || textureBakeResults.resultMaterials.Length == 0){
				Debug.LogError("Material Bake Results has no result materials. Try baking materials. Can't combine meshes.");	
				return false;				
			}
		} else {
			if (textureBakeResults.resultMaterial == null){
				Debug.LogError("Material Bake Results has no result material. Try baking materials. Can't combine meshes.");	
				return false;				
			}
		}
		return true;
	}
/*	
	bool getIsGameObjectActive(GameObject g){
#if UNITY_3_5
		return g.active;
#else
		return g.activeInHierarchy;
#endif
	}
*/	
	public void Apply(){
		for (int i = 0; i < meshCombiners.Count; i++){
			if (meshCombiners[i].isDirty){
				meshCombiners[i].combinedMesh.Apply();
				meshCombiners[i].isDirty = false;
			}
		}
	}

	public void Apply(bool triangles,
					  bool vertices,
					  bool normals,
					  bool tangents,
					  bool uvs,
					  bool colors,
					  bool uv1,
					  bool uv2,
					  bool bones=false){
		for (int i = 0; i < meshCombiners.Count; i++){
			if (meshCombiners[i].isDirty){
				meshCombiners[i].combinedMesh.Apply(triangles,vertices,normals,tangents,uvs,colors,uv1,uv2,bones);
				meshCombiners[i].isDirty = false;
			}
		}
	}
	
	public void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true){	
		if (gos == null){
			Debug.LogError("list of game objects cannot be null");
			return;	
		}
		
		//build gos lists
		for (int i = 0; i < meshCombiners.Count; i++){
			meshCombiners[i].gosToUpdate.Clear();
		}
		
		for (int i = 0; i < gos.Length; i++){
			CombinedMesh cm = null;
			obj2MeshCombinerMap.TryGetValue(gos[i],out cm);
			if (cm != null){ 
				cm.gosToUpdate.Add(gos[i]);
			} else {
				Debug.LogWarning("Object " + gos[i] + " is not in the combined mesh.");	
			}
		}
		
		for (int i = 0; i < meshCombiners.Count; i++){
			if (meshCombiners[i].gosToUpdate.Count > 0){
				GameObject[] gosToUpdate = meshCombiners[i].gosToUpdate.ToArray();
				meshCombiners[i].combinedMesh.UpdateGameObjects(gosToUpdate, recalcBounds);
			}
		}
	}

	public Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs){
		return AddDeleteGameObjects(gos,deleteGOs,true,textureBakeResults.fixOutOfBoundsUVs);
	}
	
	public Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource){		
		return AddDeleteGameObjects(gos,deleteGOs,disableRendererInSource,textureBakeResults.fixOutOfBoundsUVs);
	}	
	
	public Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource, bool fixOutOfBoundUVs){
		//Profile.StartProfile("MB2_MultiMeshCombiner.AddDeleteGameObjects1");
		//PART 1 ==== Validate
		if (!_validate(gos, deleteGOs)){
			return null;	
		}
		if (VERBOSE) Debug.Log("AddDeleteGameObjects adding:" + gos.Length + " deleting:" + deleteGOs.Length + " disableRendererInSource:" + disableRendererInSource + " fixOutOfBoundUVs:" + fixOutOfBoundUVs);		
		_distributeAmongBakers(gos, deleteGOs);
		return _bakeStep1(gos, deleteGOs, disableRendererInSource, fixOutOfBoundUVs);
	}
	
	bool _validate(GameObject[] gos, GameObject[] deleteGOs){
		_validateTextureBakeResults(); 

		if (gos != null){
			for (int i = 0; i < gos.Length; i++){
				if (gos[i] == null){
					Debug.LogError("The " + i + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
					return false;					
				}
				for (int j = i + 1; j < gos.Length; j++){
					if (gos[i] == gos[j]){
						Debug.LogError("GameObject " + gos[i] + "appears twice in list of game objects to add");
						return false;
					}
				}
				if (obj2MeshCombinerMap.ContainsKey(gos[i])){
					bool isInDeleteList = false;
					if (deleteGOs != null){
						for (int k = 0; k < deleteGOs.Length; k++){
							if (deleteGOs[k] == gos[i]) isInDeleteList = true;
						}
					}
					if (!isInDeleteList){
						Debug.LogError("GameObject " + gos[i] + " is already in the combined mesh");
						return false;
					}
				}
			}
		}
		if (deleteGOs != null){
			for (int i = 0; i < deleteGOs.Length; i++){
//				if (deleteGOs[i] == null){
//					//todo consider allowing removal of deleted GameObjects test to see if it works.
//					Debug.LogError("The " + i + "th object on the list of objects to delete is 'None'.");
//					return false;					
//				}				
				for (int j = i + 1; j < deleteGOs.Length; j++){
					if (deleteGOs[i] == deleteGOs[j] && deleteGOs[i] != null){
						Debug.LogError("GameObject " + deleteGOs[i] + "appears twice in list of game objects to delete");
						return false;
					}
				}
				if (!obj2MeshCombinerMap.ContainsKey(deleteGOs[i])){
					Debug.LogWarning("GameObject " + deleteGOs[i] + " on the list of objects to delete is not in the combined mesh.");				
				}				
			}
		}
		return true;
	}
	
	void _distributeAmongBakers(GameObject[] gos, GameObject[] deleteGOs){
		if (gos == null) gos = empty;
		if (deleteGOs == null) deleteGOs = empty;
		
		if (resultSceneObject == null) resultSceneObject = new GameObject("CombinedMesh-" + name);
		
		//PART 2 ==== calculate which bakers to add objects to
		for (int i = 0; i < meshCombiners.Count; i++){
			meshCombiners[i].extraSpace = _maxVertsInMesh - meshCombiners[i].combinedMesh.GetMesh().vertexCount;
		}
		//Profile.EndProfile("MB2_MultiMeshCombiner.AddDeleteGameObjects1");
		
		//Profile.StartProfile("MB2_MultiMeshCombiner.AddDeleteGameObjects2.1");		
		//first delete game objects from the existing combinedMeshes keep track of free space
		for (int i = 0; i < deleteGOs.Length; i++){
			CombinedMesh c = null;
			if (obj2MeshCombinerMap.TryGetValue(deleteGOs[i], out c)){
				if (VERBOSE) Debug.Log("Removing " + deleteGOs[i] + " from meshCombiner " + meshCombiners.IndexOf(c));
				c.numVertsInListToDelete += c.combinedMesh.GetNumVerticesFor(deleteGOs[i]);  //m.vertexCount;
				c.gosToDelete.Add(deleteGOs[i]);
							
			} else {
				Debug.LogWarning("Object " + deleteGOs[i] + " in the list of objects to delete is not in the combined mesh.");	
			}
		}
		for (int i = 0; i < gos.Length; i++){
			GameObject go = gos[i];
			int numVerts = MB_Utility.GetMesh(go).vertexCount;
			CombinedMesh cm = null;
			for (int j = 0; j < meshCombiners.Count; j++){
				if (meshCombiners[j].extraSpace + meshCombiners[j].numVertsInListToDelete - meshCombiners[j].numVertsInListToAdd > numVerts){
					cm = meshCombiners[j];
					if (VERBOSE) Debug.Log("Added " + gos[i] + " to combinedMesh " + j);					
					break;					
				}
			}
			if (cm == null){
				cm = new CombinedMesh(maxVertsInMesh);
				_setMBValues(cm.combinedMesh);
				meshCombiners.Add(cm);
				if (VERBOSE) Debug.Log("Created new combinedMesh");
			}
			cm.gosToAdd.Add(go);
			cm.numVertsInListToAdd += numVerts;
//			obj2MeshCombinerMap.Add(go,cm);
		}
	}
	
	Mesh _bakeStep1(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource, bool fixOutOfBoundUVs){
		//Profile.EndProfile("MB2_MultiMeshCombiner.AddDeleteGameObjects2.2");	
		//Profile.StartProfile("MB2_MultiMeshCombiner.AddDeleteGameObjects3"); 
		//PART 3 ==== Add delete meshes from combined
		for (int i = 0; i < meshCombiners.Count; i++){
			CombinedMesh cm = meshCombiners[i];	
			if (cm.combinedMesh.targetRenderer == null){
				GameObject go = cm.combinedMesh.buildSceneMeshObject(resultSceneObject,cm.combinedMesh.GetMesh(),true);
				cm.combinedMesh.targetRenderer = go.GetComponent<Renderer>();
			} else {
				if (cm.combinedMesh.targetRenderer.transform.parent != resultSceneObject.transform){
					Debug.LogError("targetRender objects must be children of resultSceneObject");
					return null;
				}
			}
			if (cm.gosToAdd.Count > 0 || cm.gosToDelete.Count > 0){
				cm.combinedMesh.AddDeleteGameObjects(cm.gosToAdd.ToArray(),cm.gosToDelete.ToArray(),disableRendererInSource,textureBakeResults.fixOutOfBoundsUVs);
			}
			Renderer r = cm.combinedMesh.targetRenderer;
			if (r is MeshRenderer){
				MeshFilter mf = r.gameObject.GetComponent<MeshFilter>();
				mf.sharedMesh = cm.combinedMesh.GetMesh();
			} else {
				SkinnedMeshRenderer smr = (SkinnedMeshRenderer) r;
				smr.sharedMesh = cm.combinedMesh.GetMesh();
			}
		}
		for (int i = 0; i < meshCombiners.Count; i++){
			CombinedMesh cm = meshCombiners[i];
			for (int j = 0; j < cm.gosToDelete.Count; j++){
				obj2MeshCombinerMap.Remove(cm.gosToDelete[j]);
				if (cm.gosToDelete[j] != null){
//					MB2_LOD lod = cm.gosToDelete[j].GetComponent<MB2_LOD>();
//					lod.SetStatus(MB2_LODStatus.notInMesh);
				}
			}
		}
		for (int i = 0; i < meshCombiners.Count; i++){	
			CombinedMesh cm = meshCombiners[i];
			for (int j = 0; j < cm.gosToAdd.Count; j++){
				obj2MeshCombinerMap.Add(cm.gosToAdd[j],cm);
//				MB2_LOD lod = cm.gosToAdd[j].GetComponent<MB2_LOD>();
//				lod.SetStatus(MB2_LODStatus.inMesh);				
			}
			if (cm.gosToAdd.Count > 0 || cm.gosToDelete.Count > 0){
				cm.gosToDelete.Clear();
				cm.gosToAdd.Clear();
				cm.numVertsInListToDelete = 0;
				cm.numVertsInListToAdd = 0;				
				cm.isDirty = true;		
			}
		}
		//Profile.EndProfile("MB2_MultiMeshCombiner.AddDeleteGameObjects3"); 
		if (VERBOSE){
			string s = "Meshes in combined:";
			for (int i = 0; i < meshCombiners.Count; i++){
				s += " mesh" + i + "(" + meshCombiners[i].combinedMesh.GetObjectsInCombined().Count + ")\n";
			}
			s += "children in result: " + resultSceneObject.transform.childCount;
			Debug.Log(s);
		}		
		if (meshCombiners.Count > 0){
			return meshCombiners[0].combinedMesh.GetMesh();
		} else {
			return null;
		}
	}
	
	public void ClearMesh(){
		DestroyMesh();
	}
	
	public void DestroyMesh(){
		for (int i = 0; i < meshCombiners.Count; i++){
#if UNITY_EDITOR
			if (meshCombiners[i].combinedMesh.targetRenderer != null) GameObject.DestroyImmediate( meshCombiners[i].combinedMesh.targetRenderer.gameObject );
#else
			if (meshCombiners[i].combinedMesh.targetRenderer != null) GameObject.Destroy( meshCombiners[i].combinedMesh.targetRenderer.gameObject );
#endif
			meshCombiners[i].combinedMesh.ClearMesh();
		}
		obj2MeshCombinerMap.Clear();
		meshCombiners.Clear();	
	}
	
	void _setMBValues(MB2_MeshCombiner targ){
		targ.renderType = renderType;
		targ.outputOption = MB2_OutputOptions.bakeIntoSceneObject;
		targ.lightmapOption = lightmapOption;
		targ.textureBakeResults = textureBakeResults;
		targ.doNorm = doNorm;
		targ.doTan = doTan;
		targ.doCol = doCol;	
		targ.doUV = doUV;
		targ.doUV1 = doUV1;		
	}
	
	public void SaveMeshsToAssetDatabase(string folderPath,string newFileNameBase){
#if UNITY_EDITOR
		for (int i = 0; i < meshCombiners.Count; i++){
			string newFilename = newFileNameBase + i + ".asset";
			Mesh mesh = meshCombiners[i].combinedMesh.GetMesh();
			string ap = AssetDatabase.GetAssetPath(mesh);
			if (ap == null || ap.Equals("")){
				Debug.Log("Saving mesh asset to " + newFilename);
				AssetDatabase.CreateAsset(mesh, newFilename);
			} else {
				Debug.Log("Mesh is an asset at " + ap);	
			}			
		}
#endif
	}
	
	public void RebuildPrefab(GameObject prefabRoot){
#if UNITY_EDITOR
		GameObject rootGO = (GameObject) PrefabUtility.InstantiatePrefab(prefabRoot);
		for (int i = 0; i < meshCombiners.Count; i++){
			meshCombiners[i].combinedMesh.buildSceneMeshObject(rootGO,meshCombiners[i].combinedMesh.GetMesh(),true);
		}		
		string prefabPth = AssetDatabase.GetAssetPath(prefabRoot);
		PrefabUtility.ReplacePrefab(rootGO,AssetDatabase.LoadAssetAtPath(prefabPth,typeof(GameObject)),ReplacePrefabOptions.ConnectToPrefab);
		Editor.DestroyImmediate(rootGO);		
#endif
	}
}
}