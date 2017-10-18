#pragma strict

public var velocity:Vector3;

function Update () {
	this.transform.Rotate(velocity*Time.deltaTime);
}