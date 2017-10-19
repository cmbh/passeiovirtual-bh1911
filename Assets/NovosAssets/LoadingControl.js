#pragma strict

//public var bkTextureFull:Texture[];
//public var bkTextureWide:Texture[];
public var bkTexture:Texture;
public var font:Font;

function Start () {
	InvokeRepeating("AddPoint", 0.5, 0.5);
}
private var threePoints:String = "";
function AddPoint () {
	threePoints += ".";
	if(threePoints.Length > 3)
		threePoints = "";
}

public function OnGUI():void
{
	var native_width:float = 1920;
	var native_height:float = 1080;
	
	var resX:float = Screen.width / native_width; 
	var resY:float = Screen.height / native_height;
	//GUI.matrix = Matrix4x4.TRS (Vector3(0, 0, 0), Quaternion.identity, Vector3 (resX, resY, 1));
	
	GUI.depth = 100;
	var backText:Texture = bkTexture;
	/*
	var aspectRatio:float = (Screen.width / (Screen.height * 1.0f));
	
	var backText:Texture;
	
	if( aspectRatio < 1.5f )
	{
		backText = bkTextureFull[GameControl.input_type];
	}
	else
	{
		backText = bkTextureWide[GameControl.input_type];
	}*/
	
	if( backText )
		GUI.DrawTexture(Rect(0,0,Screen.width, Screen.height), backText);
	
	var fs:float = GUI.skin.label.fontSize;
	
	GUI.skin.label.fontSize = 40;
	var cF:Font = GUI.skin.label.font;
	GUI.skin.label.font = font; 
	
	var curSize:Vector2 = GUI.skin.label.CalcSize(GUIContent("Carregando..."));
	
	var fontSize:float = (Screen.height * GUI.skin.label.fontSize) / native_height;
	GUI.skin.label.fontSize = fontSize;
	
	curSize = GUI.skin.label.CalcSize(GUIContent("Carregando..."));
	 
	//GUI.Label(Rect(native_width/2-curSize.x/2, native_height/2+210,curSize.x, curSize.y),"Carregando"+threePoints);
	GUI.Label(Rect( ( Screen.width - curSize.x )/2, ((Screen.height - curSize.y)*3)/4 , curSize.x, curSize.y),"Carregando"+threePoints);
	
	GUI.skin.label.fontSize = fs;
	GUI.skin.label.font = cF;
}