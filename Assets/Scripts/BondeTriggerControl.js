#pragma strict

public var show:boolean = false;

public function OnGUI ():void {
	if(show)
	{
		//GUI.Box(Rect(0,0,300,20),"Aperte E para dar passear de bonde.");
		if(Input.GetButtonDown("Enter"))
		{
			MessageSystem._instance.Close();
			GameObject.FindObjectOfType(MenuGame).SendMessage("BondeActive");
			//this.gameObject.SetActiveRecursively(false);
		}
	}
	
}

public function OnTriggerEnter(other:Collider):void
{
	if(other.CompareTag("Player"))
	{
		show = true;
		MessageSystem._instance.Show(2);
	}
}

public function OnTriggerExit(other:Collider):void
{
	if(other.CompareTag("Player"))
	{
		show = false;
		MessageSystem._instance.Close();
	}
}