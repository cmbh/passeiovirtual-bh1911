/*
 * Developed by Felipe Teixeira
 * E-mail: felipetnh@gmail.com
 * 07/June/2012
**/

using UnityEngine;
using System.Collections.Generic;

public class iTweenConstantSpeed : MonoBehaviour {
	
	/*iTweenPath path;
	Vector3[] position;
	List<Vector3> nodes = new List<Vector3>(){Vector3.zero};
	public int amount = 100;
	public float distance = 2;
	
	void Start(){
		path = this.gameObject.GetComponent("iTweenPath") as iTweenPath;
		position = iTweenPath.GetPath("iTweenPath");
		nodes[0] = position[0];
		int atual = 0;
		for( int i = 0; i < amount; i++ ){
			if( Vector3.Distance(nodes[atual],iTween.PointOnPath(position,(float)i/amount)) > distance ){
				nodes.Add( iTween.PointOnPath(position,(float)i/amount) );
				atual++;
			}
		}
		nodes.Add(position[position.Length-1]);
		
		path.nodes = nodes;
		//path.nodes.Clear();
		path.nodeCount = nodes.Count;
		//path.nodes.AddRange(nodes.ToArray());
		
		//this.enabled = false;
	}*/
	
	static public Vector3[] RecalculatePath(Transform[] positions, GameObject target)
	{
		//iTweenPath path;
		List<Vector3> nodes = new List<Vector3>(){Vector3.zero};
		Vector3[] position = new Vector3[positions.Length];
		int amount = 100;
		float distance = 2;
	
		for(int j = 0; j < positions.Length; j++)
		{
			position[j] = positions[j].position;
		}
		
		nodes[0] = position[0];
		int atual = 0;
		
		for( int i = 0; i < amount; i++ ){
			if( Vector3.Distance(nodes[atual],iTween.PointOnPath(position,(float)i/amount)) > distance ){
				nodes.Add( iTween.PointOnPath(position,(float)i/amount) );
				atual++;
			}
		}
		
		return position;
	}
}
