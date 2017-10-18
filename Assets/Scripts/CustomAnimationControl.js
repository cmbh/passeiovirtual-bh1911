#pragma strict

public var minDelay:float = 2.0f;
public var maxDelay:float = 5.0f;

function Start () {
	Invoke("PlayAnimation",Random.Range(minDelay,maxDelay));
}

function PlayAnimation () {
	animation.Play();
}