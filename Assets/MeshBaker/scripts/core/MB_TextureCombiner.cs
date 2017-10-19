//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
	using UnityEditor;
#endif

namespace DigitalOpus.MB.Core{
	/// <summary>
	/// Used internally during the material baking process
	/// </summary>
	public class MB_TextureCombiner{
		static bool VERBOSE = false;
		
		static string[] shaderTexPropertyNames = new string[] { "_MainTex", "_BumpMap", "_Normal", "_BumpSpecMap", "_DecalTex", "_Detail", "_GlossMap", "_Illum", "_LightTextureB0", "_ParallaxMap","_ShadowOffset", "_TranslucencyMap", "_SpecMap", "_TranspMap" };
		
		private List<Texture2D> _temporaryTextures = new List<Texture2D>();
		private List<Texture2D> _texturesWithReadWriteFlagSet = new List<Texture2D>();
	#if UNITY_EDITOR
		private Dictionary<Texture2D,TextureFormatInfo> _textureFormatMap = new Dictionary<Texture2D, TextureFormatInfo>();	
		
		class TextureFormatInfo{	
			public TextureImporterFormat format;
			public String platform;
			public TextureImporterFormat platformOverrideFormat;
			
			public TextureFormatInfo(TextureImporterFormat f, string p, TextureImporterFormat pf){
				format = f;
				platform = p;
				platformOverrideFormat = pf;
			}
		}
	#endif	
		
		class MeshBakerMaterialTexture{
			public Vector2 offset = new Vector2(0f,0f);
			public Vector2 scale = new Vector2(1f,1f);
			public Vector2 obUVoffset = new Vector2(0f,0f);
			public Vector2 obUVscale = new Vector2(1f,1f);
			public Texture2D t;
			public MeshBakerMaterialTexture(){}
			public MeshBakerMaterialTexture(Texture2D tx){ t = tx;	}
			public MeshBakerMaterialTexture(Texture2D tx, Vector2 o, Vector2 s, Vector2 oUV, Vector2 sUV){
				t = tx;
				offset = o;
				scale = s;
				obUVoffset = oUV;
				obUVscale = sUV;
			}
		}
		
		class MB_TexSet{
			public MeshBakerMaterialTexture[] ts;
			public List<Material> mats;
			public int idealWidth; //all textures will be resized to this size
			public int idealHeight;
			
			public MB_TexSet(MeshBakerMaterialTexture[] tss){
				ts = tss;
				mats = new List<Material>();
			}
			
			public override bool Equals (object obj)
			{
				if (!(obj is MB_TexSet)){
					return false;
				}
				MB_TexSet other = (MB_TexSet) obj;
				if(other.ts.Length != ts.Length){ 
					return false;
				} else {
					for (int i = 0; i < ts.Length; i++){
						if (ts[i].offset != other.ts[i].offset)
							return false;
						if (ts[i].scale != other.ts[i].scale)
							return false;
						if (ts[i].t != other.ts[i].t)
							return false;
						if (ts[i].obUVoffset != other.ts[i].obUVoffset)
							return false;
						if (ts[i].obUVscale != other.ts[i].obUVscale)
							return false;					
					}
					return true;
				}
			}
			
			public override int GetHashCode(){
				return 0; //not using this.
			}
		}	
	
		public delegate void FileSaveFunction(string pth, byte[] bytes);	
		
		/**<summary>Combines meshes and generates texture atlases.</summary>
	    *  <param name="createTextureAtlases">Whether or not texture atlases should be created. If not uvs will not be adjusted.</param>
	    *  <param name="progressInfo">A delegate function that will be called to report progress.</param>
	    *  <remarks>Combines meshes and generates texture atlases</remarks> */		
		public bool combineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects results, Material resultMaterial, List<GameObject> objsToMesh, List<Material> sourceMaterials, int atlasPadding, List<string> customShaderPropNames, bool resizePowerOfTwoTextures, bool fixOutOfBoundsUVs, int maxTilingBakeSize, bool saveAtlasesAsAssets = false, FileSaveFunction fileSaveFunction = null){
			return _combineTexturesIntoAtlases(progressInfo,results, resultMaterial, objsToMesh, sourceMaterials, atlasPadding, customShaderPropNames, resizePowerOfTwoTextures, fixOutOfBoundsUVs, maxTilingBakeSize, saveAtlasesAsAssets, fileSaveFunction);
		}
		
		bool _collectPropertyNames(Material resultMaterial, List<string> customShaderPropNames,List<string> texPropertyNames){
			//try custom properties remove duplicates
			for (int i = 0; i < texPropertyNames.Count; i++){
				string s = customShaderPropNames.Find(x => x.Equals(texPropertyNames[i]));
				if (s != null){
					customShaderPropNames.Remove(s);
				}
			}
			
			Material m = resultMaterial;
			if (m == null){
				Debug.LogError("Please assign a result material. The combined mesh will use this material.");
				return false;			
			}
	
			//Collect the property names for the textures
			string shaderPropStr = "";
			for (int i = 0; i < shaderTexPropertyNames.Length; i++){
				if (m.HasProperty(shaderTexPropertyNames[i])){
					shaderPropStr += ", " + shaderTexPropertyNames[i];
					if (!texPropertyNames.Contains(shaderTexPropertyNames[i])) texPropertyNames.Add(shaderTexPropertyNames[i]);
					if (m.GetTextureOffset(shaderTexPropertyNames[i]) != new Vector2(0f,0f)){
						Debug.LogWarning("Result material has non-zero offset. This is probably incorrect.");	
					}
					if (m.GetTextureScale(shaderTexPropertyNames[i]) != new Vector2(1f,1f)){
						Debug.LogWarning("Result material should probably have tiling of 1,1");
					}					
				}
			}
	
			for (int i = 0; i < customShaderPropNames.Count; i++){
				if (m.HasProperty(customShaderPropNames[i]) ){
					shaderPropStr += ", " + customShaderPropNames[i];
					texPropertyNames.Add(customShaderPropNames[i]);
					if (m.GetTextureOffset(customShaderPropNames[i]) != new Vector2(0f,0f)){
						Debug.LogWarning("Result material has non-zero offset. This is probably incorrect.");	
					}
					if (m.GetTextureScale(customShaderPropNames[i]) != new Vector2(1f,1f)){
						Debug.LogWarning("Result material should probably have tiling of 1,1.");
					}					
				} else {
					Debug.LogWarning("Result material shader does not use property " + customShaderPropNames[i] + " in the list of custom shader property names");	
				}
			}			
			
			return true;
		}
		
		bool _combineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects results, Material resultMaterial, List<GameObject> objsToMesh, List<Material> sourceMaterials, int atlasPadding, List<string> customShaderPropNames, bool resizePowerOfTwoTextures,bool fixOutOfBoundsUVs, int maxTilingBakeSize, bool saveAtlasesAsAssets, FileSaveFunction fileSaveFunction){
			bool success = false;
			try{
				_temporaryTextures.Clear();
				_texturesWithReadWriteFlagSet.Clear();
	#if UNITY_EDITOR		
				_textureFormatMap.Clear();
	#endif		
				List<GameObject> mcs = objsToMesh;
				if (mcs == null || mcs.Count == 0){
					Debug.LogError("No meshes to combine. Please assign some meshes to combine.");
					return false;
				}
				if (atlasPadding < 0){
					Debug.LogError("Atlas padding must be zero or greater.");
					return false;
				}
				if (maxTilingBakeSize < 2 || maxTilingBakeSize > 4096){
					Debug.LogError("Invalid value for max tiling bake size.");
					return false;			
				}
				
				if (progressInfo != null)
					progressInfo("Collecting textures for " + mcs.Count + " meshes.", .01f);
				
				List<string> texPropertyNames = new List<string>();	
				if (!_collectPropertyNames(resultMaterial, customShaderPropNames, texPropertyNames)){
					return false;
				}
				success = __combineTexturesIntoAtlases(progressInfo,results, resultMaterial, texPropertyNames, objsToMesh,sourceMaterials,atlasPadding,resizePowerOfTwoTextures,fixOutOfBoundsUVs, maxTilingBakeSize, saveAtlasesAsAssets, fileSaveFunction);
			} catch (Exception ex){
				Debug.LogError(ex);
			} finally {
				_destroyTemporaryTextures();
				_SetReadFlags();
			}
			return success;
		}
		
		bool __combineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects results, Material resultMaterial, List<string> texPropertyNames, List<GameObject> objsToMesh, List<Material> sourceMaterials, int atlasPadding, bool resizePowerOfTwoTextures, bool fixOutOfBoundsUVs , int maxTilingBakeSize, bool saveAtlasesAsAssets, FileSaveFunction fileSaveFunction){
			int numTextures = texPropertyNames.Count;
			List<GameObject> mcs = objsToMesh;
			bool outOfBoundsUVs = false;
			List<MB_TexSet> texsAndObjs = new List<MB_TexSet>(); //one per distinct set of textures		
			
			//collect a distinct set of textures to combine
			if (VERBOSE) Debug.Log("__combineTexturesIntoAtlases atlases:" + texPropertyNames.Count + " objsToMesh:" + objsToMesh.Count + " fixOutOfBoundsUVs:" + fixOutOfBoundsUVs);
			for (int i = 0; i < mcs.Count; i++){
				GameObject mc = mcs[i];
				if (VERBOSE) Debug.Log("Collecting textures for object " + mc);
				
				if (mc == null){
					Debug.LogError("The list of objects to mesh contained nulls.");
					return false;
				}
				
				Mesh sharedMesh = MB_Utility.GetMesh(mc);
				if (sharedMesh == null){
					Debug.LogError("Object " + mc.name + " in the list of objects to mesh has no mesh.");				
					return false;
				}
	
				Material[] sharedMaterials = MB_Utility.GetGOMaterials(mc);
				if (sharedMaterials == null){
					Debug.LogError("Object " + mc.name + " in the list of objects has no materials.");
					return false;				
				}
				
				for(int matIdx = 0; matIdx < sharedMaterials.Length; matIdx++){
					Material mat = sharedMaterials[matIdx];
					
					//check if this material is on the list of source materaials
					if (sourceMaterials != null && !sourceMaterials.Contains(mat)){
						continue;
					}
					
					Rect uvBounds = new Rect();
					bool mcOutOfBoundsUVs = MB_Utility.hasOutOfBoundsUVs(sharedMesh,ref uvBounds,matIdx);
					outOfBoundsUVs = outOfBoundsUVs || mcOutOfBoundsUVs;					
					
					if (mat.name.Contains("(Instance)")){
						Debug.LogError("The sharedMaterial on object " + mc.name + " has been 'Instanced'. This was probably caused by a script accessing the meshRender.material property in the editor. " +
							           " The material to UV Rectangle mapping will be incorrect. To fix this recreate the object from its prefab or re-assign its material from the correct asset.");	
						return false;
					}
					
					if (fixOutOfBoundsUVs){
						if (!MB_Utility.validateOBuvsMultiMaterial(sharedMaterials)){
							Debug.LogWarning("Object " + mc.name + " uses the same material on multiple submeshes. This may generate strange results especially when used with fix out of bounds uvs. Try duplicating the material.");		
						}
					}
					
					if (progressInfo != null)
						progressInfo("Collecting textures for " + mat, .01f);
					
					//collect textures scale and offset for each texture in objects material
					MeshBakerMaterialTexture[] mts = new MeshBakerMaterialTexture[texPropertyNames.Count];
					for (int j = 0; j < texPropertyNames.Count; j++){
						Texture2D tx = null;
						Vector2 scale = Vector2.one;
						Vector2 offset = Vector2.zero;
						Vector2 obUVscale = Vector2.one;
						Vector2 obUVoffset = Vector2.zero; 
						if (mat.HasProperty(texPropertyNames[j])){
							Texture txx = mat.GetTexture(texPropertyNames[j]);
							if (txx != null){
								if (txx is Texture2D){
									tx = (Texture2D) txx;
									TextureFormat f = tx.format;
									if (f == TextureFormat.ARGB32 ||
										f == TextureFormat.RGBA32 ||
										f == TextureFormat.BGRA32 ||
										f == TextureFormat.RGB24  ||
										f == TextureFormat.Alpha8
									) //DXT5 does not work
									{ } else {
										//TRIED to copy texture using tex2.SetPixels(tex1.GetPixels()) but bug in 3.5 means DTX1 and 5 compressed textures come out skewed
										//Debug.LogWarning(mc.name + " in the list of objects to mesh uses Texture "+tx.name+" uses format " + f + " that is not in: ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT. These formats cannot be resized. MeshBaker will create duplicates.");
										//tx = createTextureCopy(tx);
	#if UNITY_EDITOR									
										Debug.LogWarning("Object " + mc.name + " in the list of objects to mesh uses Texture "+tx.name+" uses format " + f + " that is not in: ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT. These textures cannot be resized. Try changing texture format. If format says 'compressed' try changing it to 'truecolor'" );													
										setTextureFormat(tx, new TextureFormatInfo(TextureImporterFormat.ARGB32, getPlatformString(), TextureImporterFormat.AutomaticTruecolor), true);
										tx = (Texture2D) mat.GetTexture(texPropertyNames[j]);
	#else
										Debug.LogError("Object " + mc.name + " in the list of objects to mesh uses Texture "+tx.name+" uses format " + f + " that is not in: ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT. These textures cannot be resized at runtime. Try changing texture format. If format says 'compressed' try changing it to 'truecolor'" );																						
										return false;	
	#endif
									}
									
								} else {
									Debug.LogError("Object " + mc.name + " in the list of objects to mesh uses a Texture that is not a Texture2D. Cannot build atlases.");				
									return false;
								}
							}
							offset = mat.GetTextureOffset(texPropertyNames[j]);
							scale = mat.GetTextureScale(texPropertyNames[j]);
						}
						if (tx == null){
							Debug.LogWarning("No texture selected for " + texPropertyNames[j] + " in object " + mcs[i].name);
						}
						if (fixOutOfBoundsUVs && mcOutOfBoundsUVs){
							obUVscale = new Vector2(uvBounds.width,uvBounds.height);
							obUVoffset = new Vector2(uvBounds.x,uvBounds.y);
						}
						mts[j] = new MeshBakerMaterialTexture(tx,offset,scale,obUVoffset,obUVscale);
					}
				
					//Add to distinct set of textures
					MB_TexSet setOfTexs = new MB_TexSet(mts);
					MB_TexSet setOfTexs2 = texsAndObjs.Find(x => x.Equals(setOfTexs));
					if (setOfTexs2 != null){
						setOfTexs = setOfTexs2;
					} else {
						texsAndObjs.Add(setOfTexs);	
					}
					if (!setOfTexs.mats.Contains(mat)){
						setOfTexs.mats.Add(mat);
					}
				}
			}
	
			//Textures in each material (_mainTex, Bump, Spec ect...) must be same size
			//Calculate the best sized to use. Takes into account tiling
			int _padding = atlasPadding;
			if (texsAndObjs.Count == 1){
				Debug.Log("All objects use the same textures.");
				_padding = 0;
			}		
			
			for(int i = 0; i < texsAndObjs.Count; i++){			
				MB_TexSet txs = texsAndObjs[i];
				txs.idealWidth = 1;
				txs.idealHeight = 1;
				int tWidth = 1;
				int tHeight = 1;			
				//get the best size all textures in a TexSet must be the same size.
				for (int j = 0; j < txs.ts.Length; j++){
					MeshBakerMaterialTexture matTex = txs.ts[j];
					
					if (matTex.t == null){
						Debug.LogWarning("Creating empty texture for " + texPropertyNames[j]);
						matTex.t = _createTemporaryTexture(tWidth,tHeight,TextureFormat.ARGB32, true);
					}
					if (!matTex.scale.Equals(Vector2.one)){
						Debug.LogWarning("Texture " + matTex.t + "is tiled by " + matTex.scale + " tiling will be baked into a texture with maxSize:" + maxTilingBakeSize);
					}
					if (!matTex.obUVscale.Equals(Vector2.one)){
						Debug.LogWarning("Texture " + matTex.t + "has out of bounds UVs that effectively tile by " + matTex.obUVscale + " tiling will be baked into a texture with maxSize:" + maxTilingBakeSize);
					}	
					
					Vector2 dim = getAdjustedForScaleAndOffset2Dimensions(matTex,fixOutOfBoundsUVs,maxTilingBakeSize);
					tWidth = (int) dim.x;
					tHeight = (int) dim.y;
					if (matTex.t.width * matTex.t.height > tWidth * tHeight){
						tWidth = matTex.t.width;
						tHeight = matTex.t.height;
					}
				}
				if (resizePowerOfTwoTextures){
					if (IsPowerOfTwo(tWidth)){
						tWidth -= _padding * 2; 
					}
					if (IsPowerOfTwo(tHeight)){
						tHeight -= _padding * 2; 
					}
					if (tWidth < 1) tWidth = 1;
					if (tHeight < 1) tHeight = 1;
				}			
				txs.idealWidth = tWidth;
				txs.idealHeight = tHeight;
			}
			
			//Build the atlases
			Rect[] uvRects = null;
			Texture2D[] atlases = new Texture2D[numTextures];
			Rect[] rs = null;
			long estArea = 0;		
	
			StringBuilder report = new StringBuilder();
			if (numTextures > 0){
				report = new StringBuilder();
				report.AppendLine("Report");
				for (int i = 0; i < texsAndObjs.Count; i++){
					MB_TexSet txs = texsAndObjs[i];
					report.AppendLine("----------");
					report.Append("Will be resized to:" + txs.idealWidth + "x" + txs.idealHeight);
					for (int j = 0; j < txs.ts.Length; j++){
						if (txs.ts[j].t != null)
							report.Append(" [" + texPropertyNames[j] + " " + txs.ts[j].t.name + " " + txs.ts[j].t.width + "x" + txs.ts[j].t.height + "]");
						else 
							report.Append(" [" + texPropertyNames[j] + " null]");
					}
					report.AppendLine("");
					report.Append("Materials using:");
					for (int j = 0; j < txs.mats.Count; j++){
						report.Append(txs.mats[j].name + ", ");
					}
					report.AppendLine("");
				}
			}		
	
			if (progressInfo != null)
				progressInfo("Creating txture atlases.", .1f);
			
			//Resize and create empty textures if none existed
			int atlasSizeX = 1;
			int atlasSizeY = 1;
			for (int i = 0; i < numTextures; i++){
				if (VERBOSE) Debug.Log("Beginning loop " + i + " num temporary textures " + _temporaryTextures.Count );
				for(int j = 0; j < texsAndObjs.Count; j++){	
					MB_TexSet txs = texsAndObjs[j];
					
					int tWidth = txs.idealWidth;
					int tHeight = txs.idealHeight;
	
					Texture2D tx = txs.ts[i].t;
					if (tx == null) tx = txs.ts[i].t = _createTemporaryTexture(tWidth,tHeight,TextureFormat.ARGB32, true);
	
					if (progressInfo != null)
						progressInfo("Adjusting for scale and offset " + tx, .01f);	
					setReadWriteFlag(tx, true, true); 
					tx = getAdjustedForScaleAndOffset2(txs.ts[i],fixOutOfBoundsUVs,maxTilingBakeSize);				
					
					//create a resized copy if necessary
					if (tx.width != tWidth || tx.height != tHeight) {
						if (progressInfo != null) progressInfo("Resizing texture '" + tx + "'", .01f);
						if (VERBOSE) Debug.Log("Copying and resizing texture " + texPropertyNames[i] + " from " + tx.width + "x" + tx.height + " to " + tWidth + "x" + tHeight);
						setReadWriteFlag((Texture2D) tx, true, true);
						tx = _resizeTexture((Texture2D) tx,tWidth,tHeight);
					}
					txs.ts[i].t = tx;
				}
	
				Texture2D[] texToPack = new Texture2D[texsAndObjs.Count];
				for (int j = 0; j < texsAndObjs.Count;j++){
					Texture2D tx = texsAndObjs[j].ts[i].t;
					estArea += tx.width * tx.height;
					texToPack[j] = tx;
				}
	#if UNITY_EDITOR
				if (Math.Sqrt(estArea) > 1000f){
					if (EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone){
						Debug.LogWarning("If the current selected build target is not standalone then the generated atlases may be capped at size 1024. If build target is Standalone then atlases of 4096 can be built");	
					}
				}
	#endif			
				if (Math.Sqrt(estArea) > 3500f){
					Debug.LogWarning("The maximum possible atlas size is 4096. Textures may be shrunk");
				}
				atlases[i] = new Texture2D(1,1,TextureFormat.ARGB32,true);
				if (progressInfo != null)
					progressInfo("Packing texture atlas " + texPropertyNames[i], .25f);	
				if (i == 0){
					if (progressInfo != null)
						progressInfo("Estimated min size of atlases: " + Math.Sqrt(estArea).ToString("F0"), .1f);			
					Debug.Log("Estimated texture minimum size:" + Math.Sqrt(estArea).ToString("F0"));
					
//					if (MB2_MeshCombiner.EVAL_VERSION){
//						MB2_Watermark.AddWatermarkTo(texToPack);
//					}				
					
					if (texsAndObjs.Count == 1){ //don't want to force power of 2 so tiling will still work
						uvRects = rs = new Rect[1] {new Rect(0f,0f,1f,1f)};
						atlases[i] = _copyTexturesIntoAtlas(texToPack,_padding,rs,texToPack[0].width,texToPack[0].height);
					} else {
						int maxAtlasSize = 4096;
						uvRects = rs = atlases[i].PackTextures(texToPack,_padding,maxAtlasSize,false);
					}
					
					
					Debug.Log("After pack textures size " + atlases[i].width + " " + atlases[i].height);
					atlasSizeX = atlases[i].width;
					atlasSizeY = atlases[i].height;	
					atlases[i].Apply();
				} else {
					atlases[i] = _copyTexturesIntoAtlas(texToPack,_padding,rs, atlasSizeX, atlasSizeY);
				}
				if (saveAtlasesAsAssets){
					_saveAtlasToAssetDatabase(atlases[i], texPropertyNames[i], i, resultMaterial, fileSaveFunction);
				}			
				_destroyTemporaryTextures(); // need to save atlases before doing this
			}
	
	
			
			Dictionary<Material,Rect> mat2rect_map = new Dictionary<Material, Rect>();
			for (int i = 0; i < texsAndObjs.Count; i++){
				List<Material> mats = texsAndObjs[i].mats;
				for (int j = 0; j < mats.Count; j++){
					if (!mat2rect_map.ContainsKey(mats[j])){
						mat2rect_map.Add(mats[j],uvRects[i]);
					}
				}
			}
			
			results.atlases = atlases;                             // one per texture on source shader
			results.texPropertyNames = texPropertyNames.ToArray(); // one per texture on source shader
			results.mat2rect_map = mat2rect_map;
				
			_destroyTemporaryTextures();
			_SetReadFlags();
			if (report != null)
				Debug.Log(report.ToString());
			return true;
		}
		
		Texture2D _copyTexturesIntoAtlas(Texture2D[] texToPack,int padding, Rect[] rs, int w, int h){
			Texture2D ta = new Texture2D(w,h,TextureFormat.ARGB32,true);
			MB_Utility.setSolidColor(ta,Color.clear);
			for (int i = 0; i < rs.Length; i++){
				Rect r = rs[i];
				Texture2D t = texToPack[i];
				int x = Mathf.RoundToInt(r.x * w);
				int y = Mathf.RoundToInt(r.y * h);
				int ww = Mathf.RoundToInt(r.width * w);
				int hh = Mathf.RoundToInt(r.height * h);
				if (t.width != ww && t.height != hh){
					t = MB_Utility.resampleTexture(t,ww,hh);
					_temporaryTextures.Add(t);	
				}
				ta.SetPixels(x,y,ww,hh,t.GetPixels());
			}
			ta.Apply();
			return ta;
		}
		
		bool IsPowerOfTwo(int x){
	    	return (x & (x - 1)) == 0;
		}	
		
	
		void setReadWriteFlag(Texture2D tx, bool isReadable, bool addToList){
	#if UNITY_EDITOR
			AssetImporter ai = AssetImporter.GetAtPath( AssetDatabase.GetAssetOrScenePath(tx) );
			if (ai != null && ai is TextureImporter){
				TextureImporter textureImporter = (TextureImporter) ai;
				if (textureImporter.isReadable != isReadable){
					if (addToList) _texturesWithReadWriteFlagSet.Add(tx);
					textureImporter.isReadable = isReadable;	
	//				Debug.LogWarning("Setting read flag for Texture asset " + AssetDatabase.GetAssetPath(tx) + " to " + isReadable);
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx));
				}
			}
	#endif
		}
	
	#if UNITY_EDITOR			
		string getPlatformString(){
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone){
				return "iPhone";	
			}
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android){
				return "Android";
			}
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux ||
				EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
				EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64 ||
				EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel){
				return "Standalone";	
			}
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebPlayer ||
				EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebPlayerStreamed){
				return "Web";
			}
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.FlashPlayer){
				return "FlashPlayer";			
			}
			return null;
		}
	#endif
		
		void setTextureSize(Texture2D tx){
	#if UNITY_EDITOR		
			AssetImporter ai = AssetImporter.GetAtPath( AssetDatabase.GetAssetOrScenePath(tx) );
			if (ai != null && ai is UnityEditor.TextureImporter){
				TextureImporter textureImporter = (TextureImporter) ai;
				int maxSize = Mathf.Max(tx.width,tx.height);
				textureImporter.maxTextureSize = maxSize;
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx), ImportAssetOptions.ForceUpdate);
			}
	#endif
		}
	
	#if UNITY_EDITOR	
		void setTextureFormat(Texture2D tx, TextureFormatInfo tfi, bool addToList){	
			
			AssetImporter ai = AssetImporter.GetAtPath( AssetDatabase.GetAssetOrScenePath(tx) );
			if (ai != null && ai is UnityEditor.TextureImporter){
				string s;
				if (addToList){
					s = "Setting texture format for ";
				} else {
					s = "Restoring texture format for ";
				}
				s += tx + " to " + tfi.format;
				if (tfi.platform != null){
					s += " setting platform override format for " + tfi.platform + " to " + tfi.platformOverrideFormat;
				}
				Debug.Log(s);
				TextureImporter textureImporter = (TextureImporter) ai;
				TextureFormatInfo restoreTfi = new TextureFormatInfo(textureImporter.textureFormat, tfi.platform, TextureImporterFormat.AutomaticTruecolor);
				string platform = tfi.platform;
				bool doImport = false;
				if (platform != null){
					int maxSize;
					TextureImporterFormat f;						
					textureImporter.GetPlatformTextureSettings(platform, out maxSize, out f);
					restoreTfi.platformOverrideFormat = f;
					if (f != 0){ //f == 0 means no override or platform doesn't exist
						textureImporter.SetPlatformTextureSettings(platform, maxSize, tfi.platformOverrideFormat);	
						doImport = true;
					}
				}
						
				if (textureImporter.textureFormat != tfi.format){
					textureImporter.textureFormat = tfi.format;
					doImport = true;
				}
				if (addToList) _textureFormatMap.Add(tx, restoreTfi);			
				if (doImport) AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx), ImportAssetOptions.ForceUpdate);
			}
		}	
	#endif	
		
		bool _isCompressed(Texture2D tx){
	#if UNITY_EDITOR		
			AssetImporter ai = AssetImporter.GetAtPath( AssetDatabase.GetAssetOrScenePath(tx) );
			if (ai != null && ai is TextureImporter){
				TextureImporter textureImporter = (TextureImporter) ai;
				TextureImporterFormat tf = textureImporter.textureFormat;
				if (tf !=  TextureImporterFormat.ARGB32){
					return true;	
				}
			}
	#endif
			return false;
		}
		
		Vector2 getAdjustedForScaleAndOffset2Dimensions(MeshBakerMaterialTexture source,bool fixOutOfBoundsUVs, int maxSize){
			if (source.offset.x == 0f && source.offset.y == 0f && source.scale.x == 1f && source.scale.y == 1f){
				if (fixOutOfBoundsUVs){
					if (source.obUVoffset.x == 0f && source.obUVoffset.y == 0f && source.obUVscale.x == 1f && source.obUVscale.y == 1f){
						return new Vector2(source.t.width,source.t.height); //no adjustment necessary
					}
				} else {
					return new Vector2(source.t.width,source.t.height); //no adjustment necessary
				}
			}
	
			if (VERBOSE) Debug.Log("getAdjustedForScaleAndOffset2Dimensions: " + source.t + " " + source.obUVoffset + " " + source.obUVscale);
			float newWidth = source.t.width * source.scale.x;
			float newHeight = source.t.height * source.scale.y;
			if (fixOutOfBoundsUVs){
				newWidth *= source.obUVscale.x;	 
				newHeight *= source.obUVscale.y;
			}
			if (newWidth > maxSize) newWidth = maxSize;
			if (newHeight > maxSize) newHeight = maxSize;
			if (newWidth < 1f) newWidth = 1f;
			if (newHeight < 1f) newHeight = 1f;	
			return new Vector2(newWidth,newHeight);
		}
		
		Texture2D getAdjustedForScaleAndOffset2(MeshBakerMaterialTexture source,bool fixOutOfBoundsUVs, int maxSize){
			if (source.offset.x == 0f && source.offset.y == 0f && source.scale.x == 1f && source.scale.y == 1f){
				if (fixOutOfBoundsUVs){
					if (source.obUVoffset.x == 0f && source.obUVoffset.y == 0f && source.obUVscale.x == 1f && source.obUVscale.y == 1f){
						return source.t; //no adjustment necessary
					}
				} else {
					return source.t; //no adjustment necessary
				}
			}
			Vector2 dim = getAdjustedForScaleAndOffset2Dimensions(source, fixOutOfBoundsUVs, maxSize);
			
			if (VERBOSE) Debug.Log("getAdjustedForScaleAndOffset2: " + source.t + " " + source.obUVoffset + " " + source.obUVscale);
			float newWidth = dim.x;
			float newHeight = dim.y;
			
			float scx = source.scale.x;
			float scy = source.scale.y;
			float ox = source.offset.x;
			float oy = source.offset.y;
			if (fixOutOfBoundsUVs){
				scx *= source.obUVscale.x;
				scy *= source.obUVscale.y;
				ox += source.obUVoffset.x;
				oy += source.obUVoffset.y;
			}
			Texture2D newTex = _createTemporaryTexture((int)newWidth,(int)newHeight,TextureFormat.ARGB32,true);
			for (int i = 0;i < newTex.width; i++){
				for (int j = 0;j < newTex.height; j++){
					float u = i/newWidth*scx + ox;
					float v = j/newHeight*scy + oy;
					newTex.SetPixel(i,j,source.t.GetPixelBilinear(u,v));
				}			
			}
			newTex.Apply();
			return newTex;
		}	
		
		//used to track temporary textures that were created so they can be destroyed
		Texture2D _createTemporaryTexture(int w, int h, TextureFormat texFormat, bool mipMaps){
			Texture2D t = new Texture2D(w,h,texFormat,mipMaps);
			MB_Utility.setSolidColor(t,Color.clear);
			_temporaryTextures.Add(t);
			return t;
		}
		
		Texture2D _createTextureCopy(Texture2D t){
			Texture2D tx = MB_Utility.createTextureCopy(t);
			_temporaryTextures.Add(tx);
			return tx;	
		}
						
		Texture2D _resizeTexture(Texture2D t, int w, int h){
			Texture2D tx = MB_Utility.resampleTexture(t,w,h);
			_temporaryTextures.Add(tx);
			return tx;							
		}
		
		void _destroyTemporaryTextures(){
			if (VERBOSE) Debug.Log("Destroying " + _temporaryTextures.Count + " temporary textures");
			for (int i = 0; i < _temporaryTextures.Count; i++){
				_destroy( _temporaryTextures[i] );
			}
			_temporaryTextures.Clear();
		}	
		
		void _SetReadFlags(){
			for (int i = 0; i < _texturesWithReadWriteFlagSet.Count; i++){
				setReadWriteFlag(_texturesWithReadWriteFlagSet[i], false,false);
			}
			_texturesWithReadWriteFlagSet.Clear();
	#if UNITY_EDITOR		
			foreach (Texture2D tex in _textureFormatMap.Keys){
				setTextureFormat(tex, _textureFormatMap[tex],false);
			}
			_textureFormatMap.Clear();
	#endif		
		}
	
		
		void _destroy(UnityEngine.Object o){
	#if UNITY_EDITOR
			string p = AssetDatabase.GetAssetPath(o);
			if (p != null && p.Equals("")) // don't try to destroy assets
				MonoBehaviour.DestroyImmediate(o);
	#else
			MonoBehaviour.Destroy(o);				
	#endif		
		}	
		
		/**
		 pass in System.IO.File.WriteAllBytes for parameter fileSaveFunction. This is necessary because on Web Player file saving
		 functions only exist for Editor classes
		 */	
		void _saveAtlasToAssetDatabase(Texture2D atlas, string texPropertyName, int atlasNum, Material resMat, FileSaveFunction fileSaveFunction){
	#if UNITY_EDITOR	
			string prefabPth = AssetDatabase.GetAssetPath(resMat);
			if (prefabPth == null || prefabPth.Length == 0){
				Debug.LogError("Could save atlas. Could not find result material in AssetDatabase.");
				return;
			}
			string baseName = Path.GetFileNameWithoutExtension(prefabPth);
			string folderPath = prefabPth.Substring(0,prefabPth.Length - baseName.Length - 4);		
			string fullFolderPath = Application.dataPath + folderPath.Substring("Assets".Length,folderPath.Length - "Assets".Length);
			
			string pth = fullFolderPath + baseName + "-" + texPropertyName + "-atlas" + atlasNum + ".png";
			Debug.Log("Created atlas for: " + texPropertyName + " at " + pth);
			//need to create a copy because sometimes the packed atlases are not in ARGB32 format
			Texture2D newTex = MB_Utility.createTextureCopy(atlas);
			byte[] bytes = newTex.EncodeToPNG();
			Editor.DestroyImmediate(newTex);
			
			fileSaveFunction(pth, bytes);
	
			AssetDatabase.Refresh();
			
			string relativePath = folderPath + baseName +"-" + texPropertyName + "-atlas" + atlasNum + ".png";                      				
			setTextureSize((Texture2D) (AssetDatabase.LoadAssetAtPath(relativePath, typeof(Texture2D))));
			_setMaterialTextureProperty(resMat, texPropertyName, relativePath);
		
	#endif		
		}
		
		void _setMaterialTextureProperty(Material target, string texPropName, string texturePath){
	#if UNITY_EDITOR
			if (VERBOSE) Debug.Log("Assigning atlas " + texturePath + " to result material " + target + " for property " + texPropName);
			if (texPropName.Equals("_BumpMap")){
				setNormalMap( (Texture2D) (AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D))));
			}
			if (target.HasProperty(texPropName)){
				target.SetTexture(texPropName, (Texture2D) (AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D))));
			}
	#endif		
		}
	
		void setNormalMap(Texture2D tx){
	#if UNITY_EDITOR		
			AssetImporter ai = AssetImporter.GetAtPath( AssetDatabase.GetAssetOrScenePath(tx) );
			if (ai != null && ai is TextureImporter){
				TextureImporter textureImporter = (TextureImporter) ai;
				if (!textureImporter.normalmap){
					textureImporter.normalmap = true;
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx));
				}
			}
	#endif		
		}
	}
}
