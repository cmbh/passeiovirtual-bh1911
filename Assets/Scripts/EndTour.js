#pragma strict

public var resumeDelay:float = 15.0f;

private var character:Transform;

public function OnTriggerEnter (other:Collider) {
	if(other.tag == "Character")
	{
		other.SendMessage("Pause");
		character = other.transform;
		Debug.Log("aq");
		//Invoke("Resume",resumeDelay);
	}
}

public function Resume():void
{
	if(character)
		character.SendMessage("Resume");
		
	character = null;
}