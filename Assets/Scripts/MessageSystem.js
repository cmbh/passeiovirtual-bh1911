#pragma strict

/*
* @Singleton
*/

static public var _instance:MessageSystem;

public var messages:String[];
public var skin:GUISkin;

private var show:boolean = false;

private var rect:Rect;
private var currentMessage:uint = 0;

function Start () {

	if(_instance != null)
	{
		Debug.LogWarning("There is another instance of MessageSystem on scene!");
		Destroy(this);
		return;
	}
	
	_instance = this;
	
	
	rect.width = 300;
	rect.height = 181;
	
	//Show(0);
}

function Update () {
	if(show)
	{
		rect.x = (Screen.width/2)-(rect.width/2);
		rect.y = Screen.height-rect.height-30;
	}
}

public function OnGUI():void
{	
	if(show)
	{		
		GUI.skin = skin;
		GUI.Box(rect,messages[currentMessage],GUI.skin.GetStyle("MessageBox"));
	}
}

public function Show(messageId:uint,autoHide:boolean):void
{
	if(!show)
	{
		currentMessage = messageId;
		show = true;
		
		if(autoHide)
			Invoke("Close",5);
	}
}

public function Show(messageId:uint):void
{
	Show(messageId,false);
}

public function Close():void
{
	if(show)
	{
		show = false;
	}
}