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
using System.IO;
using DigitalOpus.MB.Core;

#if UNITY_EDITOR
	using UnityEditor;
#endif 

/// <summary>
/// Component that handles baking materials into a combined material.
/// 
/// The result of the material baking process is a MB2_TextureBakeResults object, which 
/// becomes the input for the mesh baking.
/// 
/// This class uses the MB_TextureCombiner to do the combining.
/// 
/// This class is a Component (MonoBehavior) so it is serialized and found using GetComponent. If
/// you want to access the texture baking functionality without creating a Component then use MB_TextureCombiner
/// directly.
/// </summary>
public class MB2_TextureBaker : MB2_MeshBakerRoot {	
	static bool VERBOSE = false;
		
	[HideInInspector] public int maxTilingBakeSize = 1024;
	[HideInInspector] public bool doMultiMaterial;
	[HideInInspector] public bool fixOutOfBoundsUVs = false;
	[HideInInspector] public Material resultMaterial;
	public MB_MultiMaterial[] resultMaterials = new MB_MultiMaterial[0];
	[HideInInspector] public int atlasPadding = 1;
	[HideInInspector] public bool resizePowerOfTwoTextures = true;
	public List<string> customShaderPropNames = new List<string>();
	public List<GameObject> objsToMesh;
	
	public override List<GameObject> GetObjectsToCombine(){
		return objsToMesh;
	}
	
	public MB_AtlasesAndRects[] CreateAtlases(ProgressUpdateDelegate progressInfo, bool saveAtlasesAsAssets = false, MB_TextureCombiner.FileSaveFunction fileSaveFunction = null){
		if (doMultiMaterial){
			for (int i = 0; i < resultMaterials.Length; i++){
				MB_MultiMaterial mm = resultMaterials[i];
				if (mm.combinedMaterial == null){
					Debug.LogError("Combined Material is null please create and assign a result material.");
					return null;					
				}
				Shader targShader = mm.combinedMaterial.shader;
				for (int j = 0; j < mm.sourceMaterials.Count; j++){
					if (mm.sourceMaterials[j] == null){
						Debug.LogError("There are null entries in the list of Source Materials");
						return null;
					}
					if (targShader != mm.sourceMaterials[j].shader){
						Debug.LogWarning("Source material " + mm.sourceMaterials[j] + " does not use shader " + targShader + " it may not have the required textures. If not empty textures will be generated.");	
					}
				}
			}
		} else {
			if (resultMaterial == null){
				Debug.LogError("Combined Material is null please create and assign a result material.");
				return null;
			}
			Shader targShader = resultMaterial.shader;
			for (int i = 0; i < objsToMesh.Count; i++){
				Material[] ms = MB_Utility.GetGOMaterials(objsToMesh[i]);
				for (int j = 0; j < ms.Length; j++){
					Material m = ms[j];
					if (m == null){
						Debug.LogError("Game object " + objsToMesh[i] + " has a null material. Can't build atlases");
						return null;
					}
					if (m.shader != targShader){
						Debug.LogWarning("Game object " + objsToMesh[i] + " does not use shader " + targShader + " it may not have the required textures. If not empty textures will be generated.");
					}
				}
			}
		}

		int numResults = 1;
		if (doMultiMaterial) numResults = resultMaterials.Length;
		MB_AtlasesAndRects[] results = new MB_AtlasesAndRects[numResults];
		for (int i = 0; i < results.Length; i++){
			results[i] = new MB_AtlasesAndRects();
		}
		MB_TextureCombiner tc = new MB_TextureCombiner();
		
		Material resMatToPass = null;
		List<Material> sourceMats = null;
		
		for (int i = 0; i < results.Length; i++){
			if (doMultiMaterial) {
				sourceMats = resultMaterials[i].sourceMaterials;
				resMatToPass = resultMaterials[i].combinedMaterial;
			} else {
				resMatToPass = resultMaterial;	
			}
			Debug.Log("Creating atlases for result material " + resMatToPass);
			if(!tc.combineTexturesIntoAtlases(progressInfo, results[i], resMatToPass, objsToMesh, sourceMats, atlasPadding, customShaderPropNames, resizePowerOfTwoTextures, fixOutOfBoundsUVs, maxTilingBakeSize, saveAtlasesAsAssets, fileSaveFunction)){
				return null;
			}
		}
		
		if (results != null){
			textureBakeResults.combinedMaterialInfo = results;
			textureBakeResults.doMultiMaterial = doMultiMaterial;
			textureBakeResults.resultMaterial = resultMaterial;
			textureBakeResults.resultMaterials = resultMaterials;
			textureBakeResults.fixOutOfBoundsUVs = fixOutOfBoundsUVs;
			unpackMat2RectMap(textureBakeResults);
			
			if (Application.isPlaying){
				if (doMultiMaterial){
					for (int j = 0; j < resultMaterials.Length; j++){
						Material resMat = resultMaterials[j].combinedMaterial; //resultMaterials[j].combinedMaterial;
						Texture2D[] atlases = results[j].atlases;
						for(int i = 0; i < atlases.Length;i++){
							resMat.SetTexture(results[j].texPropertyNames[i], atlases[i]);
							//todo set normal map might be a problem
			//				_setMaterialTextureProperty(resMat, newMesh.texPropertyNames[i], relativePath);
						}
					}					
				} else {
					Material resMat = resultMaterial; //resultMaterials[j].combinedMaterial;
					Texture2D[] atlases = results[0].atlases;
					for(int i = 0; i < atlases.Length;i++){
						resMat.SetTexture(results[0].texPropertyNames[i], atlases[i]);
						//todo set normal map might be a problem
		//				_setMaterialTextureProperty(resMat, newMesh.texPropertyNames[i], relativePath);
					}
				}
			}
		}
		
		if (VERBOSE) Debug.Log("Created Atlases");
		return results;
	}

	public MB_AtlasesAndRects[] CreateAtlases(){
		return CreateAtlases(null, false, null);
	}		

	void unpackMat2RectMap(MB2_TextureBakeResults results){
		List<Material> ms = new List<Material>();
		List<Rect> rs = new List<Rect>();
		for (int i = 0; i < results.combinedMaterialInfo.Length; i++){
			MB_AtlasesAndRects newMesh = results.combinedMaterialInfo[i];
			Dictionary<Material,Rect> map = newMesh.mat2rect_map;
			foreach(Material m in map.Keys){
				ms.Add(m);
				rs.Add(map[m]);
			}
		}
		results.materials = ms.ToArray();
		results.prefabUVRects = rs.ToArray();
	}
	
	/**
	 pass in System.IO.File.WriteAllBytes for parameter fileSaveFunction. This is necessary because on Web Player file saving
	 functions only exist for Editor classes
	 */
	public void CreateAndSaveAtlases(ProgressUpdateDelegate progressInfo, MB_TextureCombiner.FileSaveFunction fileSaveFunction){
		MB_AtlasesAndRects[] mAndAs = null;
		try{
			if (doCombinedValidate(this, MB_ObjsToCombineTypes.dontCare)){
				mAndAs = CreateAtlases(progressInfo, true, fileSaveFunction);
				if (mAndAs != null){
					MB2_MeshBakerCommon mb = GetComponent<MB2_MeshBakerCommon>();
					if (mb != null){
						mb.textureBakeResults = textureBakeResults;	
					}
				}
			}
		} catch(Exception e){
			Debug.LogError(e);	
		} finally {
			if (mAndAs != null){
				for(int j = 0; j < mAndAs.Length; j++){
					MB_AtlasesAndRects mAndA = mAndAs[j];
					if (mAndA != null && mAndA.atlases != null){
						for (int i = 0; i < mAndA.atlases.Length; i++){
							if (mAndA.atlases[i] != null){
								MB_Utility.Destroy(mAndA.atlases[i]);
							}
						}
					}
				}
			}
		}
	}
}

