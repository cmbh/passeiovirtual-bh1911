#pragma strict

public var charreteAndando:Transform;
public var player:Transform;
public var menuGame:MenuGame;

public var indexNextLevel:int = 5;

private var hasCollision:boolean = false;

public function Start():void
{
	if(!menuGame)
		menuGame = (GameObject.FindObjectOfType(MenuGame) as MenuGame);
}

function Update () {
	//if(hasCollision && Input.GetKeyDown(KeyCode.E))
	if(hasCollision && Input.GetButtonDown("Enter"))
	{
		MessageSystem._instance.Close();
		
		/*this.transform.parent.gameObject.SetActiveRecursively(false);
		charreteAndando.gameObject.SetActiveRecursively(true);
		player.gameObject.SetActiveRecursively(false);*/
		
		Application.LoadLevel(indexNextLevel);
				
//menuGame.SendMessage("CharretEnabled", true);
		
		//SelectLocationMenu._instance.Open();
		//GameControl._instance.Pause();
	}
}

public function OnTriggerEnter(other:Collider):void
{
	if(other.CompareTag("Player"))
	{
		hasCollision = true;
		MessageSystem._instance.Show(1);
		//GameControl._instance.Pause();
	}
}

public function OnTriggerExit(other:Collider):void
{
	if(other.CompareTag("Player"))
	{
		MessageSystem._instance.Close();
		hasCollision = false;
	}
}