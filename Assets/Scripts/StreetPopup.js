#pragma strict

//@script ExecuteInEditMode;

public var index:String = "0";

function OnTriggerEnter (other:Collider) {
	if(other.CompareTag("Player"))
	{
		var text:String[] = XMLReader.GetString(index).Split("*"[0]);
		
		if(text.Length == 2)
			Popup._instance.Open(text[0],text[1]);
		else
			Popup._instance.Open(text[0],"");
	}
}

/*function Update():void
{	
	if(Input.GetMouseButtonDown(0))
		Popup._instance.Open(wayType,wayName);
}*/