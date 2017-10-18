#pragma strict

public var target:Transform;
public var navMeshAgent:NavMeshAgent;

function Start () {
	if(!navMeshAgent)
		navMeshAgent = transform.GetComponent(NavMeshAgent);	
}

function Update () {
	navMeshAgent.destination = target.position;
}