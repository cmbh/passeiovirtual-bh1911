#pragma strict

public var cam:Camera;
public var carregando:GameObject;
public var mask:LayerMask = -1;
public var allScene:GameObject;
public var skin:GUISkin;

public var enableBackButton:boolean = true;

public var background4x3:Texture[];
public var background16x9:Texture[];

private var textureIndex:uint = 0;
private var optionIndex:uint = 0;

function Update () {
	if(Input.GetMouseButtonUp(0))
	{
		var ray:Ray = cam.ScreenPointToRay (Input.mousePosition);
		var hit:RaycastHit;
	    if (Physics.Raycast (ray, hit,2000,mask.value)) {
	    	Debug.Log(hit.transform.name);
	        LoadLevel(hit.transform.name);
	    }
	}
}

function OnGUI () {

	var backButtonStyle:GUIStyle = skin.GetStyle("BackButton");
	var buttonWidth:float = backButtonStyle.normal.background.width;
	var buttonHeight:float = backButtonStyle.normal.background.height;
	
	var buttonRect:Rect;
	var buttonPracaLib:Rect;
	var buttonPracaRep:Rect;
	var square:float = Screen.height * 0.2f;
	
	if( GameControl.Instance.AspectRatio == ScreenAspectRatio.ASPECTRATIO_4_3 )
	{
		GUI.DrawTexture( Rect( 0, 0, Screen.width, Screen.height),background4x3[textureIndex]);
		buttonRect = Rect( Screen.width - (Screen.width * 0.25f), Screen.height - (Screen.height*0.1f), buttonWidth, buttonHeight );
		buttonPracaLib = Rect( Screen.width * 0.12f, Screen.height * 0.72f,square,square);
		buttonPracaRep = Rect( Screen.width * 0.12f, Screen.height * 0.45f,square,square);

	}
	else
	{
		GUI.DrawTexture( Rect( 0, 0, Screen.width, Screen.height),background16x9[textureIndex]);
		buttonRect = Rect( Screen.width - (Screen.width * 0.27f), Screen.height - (Screen.height*0.12f), buttonWidth, buttonHeight );
		buttonPracaLib = Rect( Screen.width * 0.22f, Screen.height * 0.7f,square,square);
		buttonPracaRep = Rect( Screen.width * 0.22f, Screen.height * 0.45f,square,square);
	}
	
	GUI.color.a = 0;
	
	if( GUI.Button(buttonPracaLib,GUIContent("","b1")) || ( GUI.tooltip == "b1" && Input.GetButtonDown("Enter") ))
	{
		loadMapPracaLib();
	}
	
	if(GUI.Button(buttonPracaRep,GUIContent("","b2")) || ( GUI.tooltip == "b2" && Input.GetButtonDown("Enter") ))
	{
		loadMapPracaRep();
	}
	
	GUI.color.a = 1;
	
	if(PlayerPrefs.GetInt("inputType") == parseInt(InputType.KEYBOARD))
	{
		GUI.tooltip = selectArea("VerticalB");
	}
	else if(PlayerPrefs.GetInt("inputType") == parseInt(InputType.JOYSTICK) )
	{
		GUI.tooltip = selectArea("VerticalJoystick");
	}
	
			
	if(GUI.tooltip == "b1")
	{
		textureIndex = 0;
	}
	else if(GUI.tooltip == "b2")
	{
		textureIndex = 1;
	}

	if( enableBackButton && ( GUI.Button( buttonRect, "", skin.GetStyle("BackButton") ) || Input.GetButtonDown("Back") ) )
	{
		backToMenu();
	}

}

private function selectArea( axis:String )
{

		if( Input.GetAxis(axis) > 0 )
		{
			optionIndex++;
			
			if( optionIndex >= background4x3.Length )
				optionIndex = 0;
		}
		else
		if( Input.GetAxis(axis) < 0 )
		{
			optionIndex--;
			
			if( optionIndex < 0)
				optionIndex = background4x3.Length - 1;
		}
		
		switch( optionIndex )
		{
			case 0: return "b1"; break;
			case 1: return "b2"; break;
		}

}

private function backToMenu()
{
	this.allScene.SetActiveRecursively(false);
	
	Screen.lockCursor = true;
	Screen.showCursor = false;
	Time.timeScale = 1;
}

private function loadMapPracaLib()
{
	if( Application.loadedLevelName == "Praca_Liberdade" )
	{
		this.allScene.SetActiveRecursively(false);
	
		Screen.lockCursor = true;
		Screen.showCursor = false;
		Time.timeScale = 1;
	}
	else
		Application.LoadLevel("VideoPracaRepLiberdade");
}

private function loadMapPracaRep()
{
	if( Application.loadedLevelName == "Praca_Republica" )
	{
		this.allScene.SetActiveRecursively(false);
	
		Screen.lockCursor = true;
		Screen.showCursor = false;
		Time.timeScale = 1;
	}
	else	
		Application.LoadLevel("VideoPracaLibRepublica");
}

private function LoadLevel(levelName:String)
{
	if(levelName != Application.loadedLevel)
	{
		carregando.SetActiveRecursively(true);
		if(allScene)
		{
			allScene.active = false;
		}
		
		enableBackButton = false;
		//Time.timeScale = 1;
		//yield WaitForSeconds(0.01);
		Debug.Log("Carregando");
		Application.LoadLevel(levelName);
	}
}