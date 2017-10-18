#pragma strict

public var menuGame:MenuGame;

function Start () {

}

function OnTriggerEnter (other:Collider) {
	if(other.tag == "Character")
	{
		menuGame.CharretEnabled(false);
	}
}