#pragma strict

public var textures:Texture[];
public var carregando:Transform;

private var alpha:float = 0.0F;
private var alphaFade:float = 0.30F;
private var currentTextetureIndex:uint = 0;

private var async : AsyncOperation;

public function Start()
{
	async = Application.LoadLevelAsync (1);
    
    async.allowSceneActivation = false;
    
    yield async;
}

function Update () {
	alpha += alphaFade*Time.deltaTime;
	
	if(alpha <= .0F || alpha >= 1.0F)
	{
		if(alpha <= .0F)
			currentTextetureIndex++;
			
		if(textures.Length > currentTextetureIndex)
		{
			alphaFade = -alphaFade;
			//alpha = 0.0F;
		}
		else
		{
			carregando.gameObject.SetActive(true);
			InvokeRepeating("EnableSceneStart",4,0.5);
			//Application.LoadLevel(1);
		}
		
	}
	
}
public function EnableSceneStart():void
{
	if(async.progress >= .9f && carregando.active)
	{
		async.allowSceneActivation = true;
	}
}
public function OnGUI():void
{
	
	if(alpha > 0.0F)
	{
		
		GUI.color.a = alpha;
		
		//if( GameConfig.Instance.AspectRatio == ScreenAspectRatio.ASPECTRATIO_4_3 )
		//{}
		
		/*
		var native_width:float = textures[currentTextetureIndex].width;
		var native_height:float = textures[currentTextetureIndex].height;
		
		var resX:float = Screen.width / native_width; 
		var resY:float = Screen.height / native_height; 
		GUI.matrix = Matrix4x4.TRS (Vector3(0, 0, 0), Quaternion.identity, Vector3 (resX, resY, 1));
		GUI.color.a = alpha;
		//GUI.DrawTexture(Rect(Screen.width/2-textures[currentTextetureIndex].width*3/2,Screen.height/2-textures[currentTextetureIndex].height*3/2,textures[currentTextetureIndex].width*3,textures[currentTextetureIndex].height*3),textures[currentTextetureIndex]);
		GUI.DrawTexture(Rect(0,0,native_width,native_height),textures[currentTextetureIndex]);
		*/
	}
}