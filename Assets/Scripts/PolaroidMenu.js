#pragma strict

/*
* Desenvolvedor: Eder (Loko) Moreira Raimundo
* Cria um menu dinamico em forma de fotos polaroids
*/

@script ExecuteInEditMode

class PhotoButton
{
	public var label:String = "";
	public var imageTexture:Texture = null;
	public var camera:Camera;
}

public var photosInRow:int = 4;

public var bgTexture:Texture;

public var buttons:PhotoButton[];
//public var imageTexture:Texture[];
//public var targets:Transform[];

public var player:Transform;
public var skin:GUISkin;

private var currentCam:Camera;

private var show = false;

function Start () {
	if(!skin)
	{
		skin = Resources.Load("MetalGUISkin",GUISkin);
	}
	
	if(!currentCam)
		currentCam = buttons[0].camera;
}

function Update () {
	if(Input.GetKeyDown(KeyCode.Q))
	{
		show = !show;
	}
}

public function OnGUI():void
{
	GUI.skin = skin;
	if(show)
	{
		for(var i:uint = 0; i < buttons.Length; i++)
		{
			DrawPolaroid(buttons[i],(i%photosInRow)*(200+10.0F),(i/photosInRow)*(200+10.0F));
		}
	}
}

private function DrawPolaroid(button:PhotoButton, x:float, y:float):void
{
	if(GUI.Button(Rect(x,y,200,200),""))
	{
		
		if(currentCam)
			currentCam.enabled = false;
			
		button.camera.enabled = true;
		
		currentCam = button.camera;
		
		show = false;
		//player.gameObject.SendMessage("UpdatePosition",targets[i]);
	}
	GUI.DrawTexture(Rect(x,y,200,200),bgTexture);
	if(button.imageTexture)
		GUI.DrawTexture(Rect(x+10,y+10,180,140),button.imageTexture);
	GUI.Label(Rect(x+10,y+160,180,40),button.label);
}