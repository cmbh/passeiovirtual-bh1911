using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MB_DynamicAddDeleteExample : MonoBehaviour {
	public GameObject prefab;
	
	List<GameObject> objsInCombined = new List<GameObject>();
	MB2_MeshBaker mbd;
	GameObject[] objs;
	void Start(){
		mbd = GetComponent<MB2_MeshBaker>(); 
		
		// instantiate 10k game objects
		int dim = 25;
		GameObject[] gos = new GameObject[dim * dim];
		for (int i = 0; i < dim; i++){
			for (int j = 0; j < dim; j++){
				GameObject go = (GameObject) Instantiate(prefab);
				gos[i*dim + j] = go.GetComponentInChildren<MeshRenderer>().gameObject;
				go.transform.position = (new Vector3(9f*i,0,9f * j));
				//put every third object in a list so we can add and delete it later
				if ((i*dim + j) % 3 == 0){
					objsInCombined.Add(gos[i*dim + j]);
				}
			}
		}
		//add objects to combined mesh
		mbd.AddDeleteGameObjects(gos, null);
		mbd.Apply();
		
		objs = objsInCombined.ToArray();
		//start routine which will periodically add and delete objects
		StartCoroutine(largeNumber());
	}
	
	IEnumerator largeNumber() {
		while(true){
			yield return new WaitForSeconds(1.5f);
			//Delete every third object
			mbd.AddDeleteGameObjects(null, objs);
			mbd.Apply(true,true,true,true,true,false,false,false);
			
			yield return new WaitForSeconds(1.5f);
			//Add objects back
			mbd.AddDeleteGameObjects(objs, null);
			mbd.Apply(true,true,true,true,true,false,false,false);
		}
	}
	
}
