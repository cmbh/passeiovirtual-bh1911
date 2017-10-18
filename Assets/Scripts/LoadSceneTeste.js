#pragma strict

public var mt:MovieTexture;
//public var plane:Transform;

public var levelName:String = "praca_republica";

//private var t:float = 0;

private var async : AsyncOperation;
function Start () {
	
	
	//plane.renderer.material.mainTexture = mt;
	audio.clip = mt.audioClip;
	audio.Play();
	
	mt.loop = false;
	mt.Play();
 	//StartCoroutine("LoadMyScene");
 	
 	//Debug.Log("start");

	
	
	async = Application.LoadLevelAsync (levelName);
    
    async.allowSceneActivation = false;
    
    yield async;
    
    //Debug.Log("fim");
}

function OnGUI () {
	//GUI.Box(Rect(0,0,200,30),""+async.progress);
	//GUI.Box(Rect(0,0,400,30),""+t);
	//Debug.Log(async.isDone);
//	if(Application.isLoadingLevel)
//	{
//		GUI.Box(Rect(0,40,200,30),"carregando");
//		t++;
//	}
//	if(async.isDone)
//	{
//		GUI.Box(Rect(0,80,200,30),"Pronto");
//	}
	
	if(async.progress >= .9f && !mt.isPlaying)
	{
		async.allowSceneActivation = true;
	}
	
	GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height),mt);
	
	if( async.progress >= .9f )
	{
		if( GameControl.Instance.InputType == InputType.KEYBOARD || GameControl.Instance.InputType == InputType.KEYBOARD_MOUSE )
			GUI.Label(Rect(10, Screen.height-60, 400, 80),"Aperte Espaço para pular");
		else
		if( GameControl.Instance.InputType == InputType.JOYSTICK )
			GUI.Label(Rect(10, Screen.height-60, 400, 80),"Aperte X ou Y para pular");
			
		if( Input.GetButton("Jump") )
			async.allowSceneActivation = true;	
		
	}
	else
	GUI.Label(Rect(10, Screen.height-60, 200, 80),"Carregando...");
}

function LoadMyScene()
{

	//Debug.Log("start");

	var async : AsyncOperation;
	
	async = Application.LoadLevelAsync ("PRACA_V009");
    
    async.allowSceneActivation = false;
    
    yield async;
    
    //Debug.Log("fim");
    
    async.allowSceneActivation = true;
}

function OnApplicationQuit():void
{
	if(async != null)
		async.allowSceneActivation = false;
}