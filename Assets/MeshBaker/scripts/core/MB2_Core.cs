using UnityEngine;
using System.Collections;

namespace DigitalOpus.MB.Core{
	public enum MB_OutputOptions{
		bakeIntoPrefab,
		bakeMeshsInPlace,
		bakeTextureAtlasesOnly,
		bakeIntoSceneObject
	}
	
	public enum MB_RenderType{
		meshRenderer,
		skinnedMeshRenderer
	}
	
	public enum MB2_OutputOptions{
		bakeIntoSceneObject,
		bakeMeshAssetsInPlace,
		bakeIntoPrefab
	}
	
	public enum MB2_LightmapOptions{
		preserve_current_lightmapping,
		ignore_UV2,
		copy_UV2_unchanged,
		generate_new_UV2_layout
	}	
}
