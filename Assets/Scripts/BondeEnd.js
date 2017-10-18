#pragma strict

public var resumeDelay:float = 15.0f;
public var menuGame:MenuGame;

private var character:Transform;

public function OnTriggerEnter (other:Collider) {
	if(other.tag == "Bonde")
	{
		other.SendMessage("Pause");
		character = other.transform;
		Debug.Log("aq");
		
		//Invoke("Resume",resumeDelay);
		if(menuGame)
		{
			menuGame.gameObject.SendMessage("BondeDeactive");
		}
		
	}
}

public function Resume():void
{
	if(character)
		character.SendMessage("Resume");
		
	character = null;
}