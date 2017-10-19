using UnityEngine;
using System.Collections;

public class MB_Example : MonoBehaviour {
	
	public MB2_MeshBaker meshbaker;
	public GameObject[] objsToCombine;
	
   	void Start(){
	  //Add the objects to the combined mesh	
	  //Must have previously baked textures for these in the editor
      meshbaker.AddDeleteGameObjects(objsToCombine, null);
		
      //apply the changes we made this can be slow. See documentation
	  meshbaker.Apply();
	}
	
	void LateUpdate(){
		//Apply changes after this and other scripts have made changes
		//Only to vertecies, tangents and normals
		//Only want to call this once per frame since it is slow
		meshbaker.UpdateGameObjects(objsToCombine);
		meshbaker.Apply(false,true,true,true,false,false,false,false);	
	}
}
