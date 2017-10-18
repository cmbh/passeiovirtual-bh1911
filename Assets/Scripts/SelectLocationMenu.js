#pragma strict

//@script ExecuteInEditMode;

/*
* @Singleton
*/

static public var _instance:SelectLocationMenu;

public var texture:Texture;
public var circles:Texture[];
public var spawns:Transform[];

private var opened:boolean = false;

private var hindex:uint = 0;
private var vindex:uint = 0;

private var haxis:float = .0f;
private var vaxis:float = .0f;

private var textureRect:Rect;

function Start () {
	if(_instance != null)
	{
		Debug.LogWarning("There is another instance of SelectLocationMenu on scene!");
		Destroy(this);
		return;
	}
	
	_instance = this;
}

function Update () {
	if(opened)
	{
		textureRect = Rect(0,0,Screen.width,Screen.height);
	
		haxis = Input.GetAxis("Horizontal");
		vaxis = Input.GetAxis("Vertical");
		
		/*if(haxis > 0)
		{
			hindex++;
		}
		else if(haxis < 0)
		{
			hindex--;
		}
		else if(vaxis > 0)
		{
			vindex++;
		}
		else if(vaxis < 0)
		{
			vindex--;
		}*/
		if(Input.GetKeyDown(KeyCode.D))
		{
			hindex++;
		}
		else if(Input.GetKeyDown(KeyCode.A))
		{
			hindex--;
		}
		
		if(Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			Close();
		}			
	}
}

function OnGUI():void
{
	if(opened)
	{
		GUI.DrawTexture(textureRect,texture);
		GUI.DrawTexture(textureRect,circles[hindex]);
	}
}

public function Open():void
{
	opened = true;
}

public function Close():void
{
	opened = false;
	GameControl.Instance.Resume();
}