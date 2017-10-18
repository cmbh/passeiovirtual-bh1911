#pragma strict

//@script ExecuteInEditMode;

static public var B_ACTIVE:boolean = false;

public var skin:GUISkin;
//public var cmbhLogo:Texture;
public var carregando:GameObject;

public var player:GameObject;
public var bonde:GameObject;
public var bondePraca:GameObject;
public var charreteAndando:GameObject;
public var charreteParada:GameObject;

public var sceneOne:Transform;
public var mainMenu:Transform;
public var configMenu:ConfigMenu;

public var selectionTexture:Texture;
public var logoTopo:Texture;
public var margenPagina:Texture;
public var boxBG:Texture;

public var menuButtonTexture:Texture;

public var charretMenuTexture:Texture[];
public var helpTexture4x3:Texture[];
public var helpTexture16x9:Texture[];

public var blur:BlurEffect;
public var cameraCharretBlurEffect:BlurEffect;
public var cameraBondeBlurEffect:BlurEffect;

public var mouseLookCamCharret:MonoBehaviour;
public var mouseLookCamBonde:MonoBehaviour;

//public var bg:Texture;

public var buttonFont:Font;
public var mapa:GameObject;

private var buttonSelected:uint = 0;

private var show:boolean = false;
private var showHelp:boolean = false;
private var loading:boolean = false;

private var mouseLookCam:MonoBehaviour;
private var mouseLookPlayer:MonoBehaviour;



public function Awake():void
{
	mouseLookCam = (Camera.main.GetComponent("MouseLook") as MonoBehaviour);
	mouseLookPlayer = (GameObject.FindWithTag("Player").GetComponent("MouseLook") as MonoBehaviour);
	
	if(Application.loadedLevelName == "Praca_Republica")
	{
		CharretEnabled(true);
	}
	
	Time.timeScale = 1;
}

public function OnEnable():void
{
	show = false;
	B_ACTIVE = false;
	
	/*if(B_ACTIVE)
		BondeActive();
	else
		BondeDeactive();
	*/
	//Screen.lockCursor = true;
	//Screen.showCursor = false;
}

function Update () {
	if(Input.GetKeyDown(KeyCode.Escape) && !configMenu.enabled && !showHelp && !mapa.active && !loading)
	{
		show = !show;
		Time.timeScale = 1-Time.timeScale;
		
		Screen.lockCursor = !show;
		Screen.showCursor = show;
		
		if(player.active)
		{
			if(blur)
				blur.enabled = show;
				
			mouseLookCam.enabled = !show;
			mouseLookPlayer.enabled = !show;
		}
		else if(!B_ACTIVE)
		{
			EnabledCharreteMenu(true);
		}
		else
		{
			EnabledBondeMenu(true);
		}
	}
	if(Input.GetKeyDown(KeyCode.Alpha4))
	{
		showHelp = !showHelp;
		Time.timeScale = (showHelp) ? 0 : 1;
			
	}
	if(Input.GetKeyDown(KeyCode.L))
	{
		player.transform.position = Vector3(86.81968,60.91533,36.85732);
	}
	if(show)
	{
		UpdateButtonSelected();
	}
}

private function UpdateButtonSelected():void
{
	if(Input.GetKeyDown(KeyCode.W) && buttonSelected > 0)
		buttonSelected--;
	else if(Input.GetKeyDown(KeyCode.S) && buttonSelected < 4)
		buttonSelected++;
	
	if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
	{
		SelectOption(buttonSelected);
	}
		
}

public function SelectOption(index:int):void
{
	if(index == 0)
	{
		show = false;
		Time.timeScale = 1;
		Screen.lockCursor = !show;
		Screen.showCursor = show;
		
		if(player.active)
			EnabledCamComponents();
		else if(!B_ACTIVE)
			EnabledCharreteMenu(false);
		else
			EnabledBondeMenu(false);
	}
	else if(index == 1)
	{
	
		if(charreteAndando && charreteAndando.active == true)
		{
			CharretEnabled(false);
			EnabledCamComponents();
			show = false;
			Screen.lockCursor = !show;
			Screen.showCursor = show;
			Time.timeScale = 1;
		}
		else if(B_ACTIVE)
		{
			BondeDeactive();
			EnabledCamComponents();
			show = false;
			Screen.lockCursor = !show;
			Screen.showCursor = show;
			Time.timeScale = 1;
		}
		else
		{
			mapa.SetActiveRecursively(true);
			EnabledCamComponents();
		}
		
		show = false;
//		Screen.lockCursor = !show;
//		Screen.showCursor = show;
//		Time.timeScale = 1;
//		
//		if(!B_ACTIVE)
//			BondeActive();
//		else
//			BondeDeactive();
//		
//		show = false;
//		Time.timeScale = 1;
					
	}
	else if(index == 2)
	{
		showHelp = true;
	}
	else if(index == 3)
	{
		show = false;
		
		if(configMenu)
			configMenu.enabled = true;
	}
	else if(index == 4)
	{
		Screen.lockCursor = false;
		Screen.showCursor = true;
		//carregando.SetActiveRecursively(true);
		Time.timeScale = 1;
		loading = true;
		LoadLevel("MainScene");
		//ActiveLevel(false);
	}
	else if(index == 5)
	{
		//Application.Quit();
		System.Diagnostics.Process.GetCurrentProcess().Kill();
		
		return;
	}
			
}

public function EnabledCamComponents():void
{
	if(blur)
		blur.enabled = false;
	
	(Camera.main.GetComponent("MouseLook") as MonoBehaviour).enabled = true;
	(GameObject.FindWithTag("Player").GetComponent("MouseLook") as MonoBehaviour).enabled = true;
}

public function DrawMainMenu():void
{
	//GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height),bg);
	GUI.DrawTexture(Rect(0,15,Screen.width,Screen.height-30),margenPagina);
	GUI.BeginGroup(Rect(0,0,Screen.width,Screen.height));
	//GUI.DrawTexture(Rect(Screen.width/2-boxBG.width/2,Screen.height/2-boxBG.height/2,boxBG.width,boxBG.height),boxBG);
	GUI.BeginGroup(Rect(0,Screen.height/2-(300+logoTopo.height/2+10)/2,Screen.width,(300+logoTopo.height/2+10)));
	
	GUI.DrawTexture(Rect(Screen.width/2-logoTopo.width/2.0f/2,0,logoTopo.width/2.0f,logoTopo.height/2.0f),logoTopo);
	GUI.DrawTexture(Rect(Screen.width/2-230, logoTopo.height/2+10, 460, 300),boxBG);
	
	GUI.EndGroup();
	//GUI.Box(Rect(Screen.width/2-200, 0, 400, Screen.height),"Pró-Memória");
	GUI.skin.label.font = buttonFont;
	GUI.skin.label.fontSize = 27;
	GUI.skin.label.alignment = TextAnchor.MiddleCenter;
	var buttonLabel:String[];
	
	if(player.active)
		buttonLabel = ["VOLTAR", "MAPA", "AJUDA", "CONFIGURAÇÕES", "MENU INICIAL", "SAIR DO JOGO"];
	else
		buttonLabel = ["VOLTAR", "PASSEIO VIRTUAL", "AJUDA", "CONFIGURAÇÕES", "MENU INICIAL", "SAIR DO JOGO"];
	
	for(var k:uint = 0; k < 6; k++)
	{
		GUI.Label(Rect(Screen.width/2-150, Screen.height/2-100+k*40, 300, 40),buttonLabel[k]);
		
		if(Screen.width-Input.mousePosition.x > Screen.width/2-150 && Screen.width-Input.mousePosition.x < Screen.width/2+150 && Screen.height-Input.mousePosition.y > Screen.height/2-100+k*40 && Screen.height-Input.mousePosition.y < Screen.height/2-55+k*40)
		{
			buttonSelected = k;
			if(Input.GetMouseButtonUp(0))
			{
				SelectOption(buttonSelected);
			} 
		}
		
	}
	GUI.DrawTexture(Rect(Screen.width/2-150, Screen.height/2-112+buttonSelected*40, 300, 50),selectionTexture);
	GUI.EndGroup();
}

public function OnGUI():void
{

	/*var native_width:float = 1024;
	var native_height:float = 768;
	
	var resX:float = Screen.width / native_width; 
	var resY:float = Screen.height / native_height; 
	GUI.matrix = Matrix4x4.TRS (Vector3(0, 0, 0), Quaternion.identity, Vector3 (resX, resY, 1));
	*/
	GUI.depth = -1000;
	if(show && !configMenu.enabled && !mapa.active && !showHelp)
	{
		DrawMainMenu();
		/*GUI.skin = skin;
		GUI.Box(Rect(Screen.width/2-200, 0, 400, Screen.height),"Pró-Memória");
		if(GUI.Button(Rect(Screen.width/2-150, Screen.height/2-100, 300, 40),"Continuar"))
		{
			show = false;
			Time.timeScale = 1;
			Screen.lockCursor = !show;
			Screen.showCursor = show;
		}
		if(GUI.Button(Rect(Screen.width/2-150, Screen.height/2-50, 300, 40),"Menu Principal"))
		{
			Screen.lockCursor = false;
			Screen.showCursor = true;
			//carregando.SetActiveRecursively(true);
			Time.timeScale = 1;
			//LoadLevel("MainScene");
			ActiveLevel(false);
		}
		if(GUI.Button(Rect(Screen.width/2-150, Screen.height/2, 300, 40),((!B_ACTIVE) ? "Passeio de bonde" : "Passeio virtual")))
		{
			if(!B_ACTIVE)
				BondeActive();
			else
				BondeDeactive();
			
			show = false;
			Time.timeScale = 1;
				
		}
		if(GUI.Button(Rect(Screen.width/2-150, Screen.height/2+50, 300, 40),"Sair"))
		{
			Application.Quit();
		}
		if(cmbhLogo)
			GUI.DrawTexture(Rect(Screen.width/2-cmbhLogo.width/2,Screen.height-cmbhLogo.height,cmbhLogo.width,cmbhLogo.height),cmbhLogo);
			*/
	}
	else if(!configMenu.enabled && !mapa.active && !showHelp && !loading)
	{
		GUI.DrawTexture(Rect(10,10,menuButtonTexture.width*0.5, menuButtonTexture.height*0.5),menuButtonTexture);
	}
	else if(showHelp)
	{
		helpMenu();
	}
	
	if(Application.loadedLevelName == "Praca_Republica" && !player.active)
	{
		var textureLib:Texture;
		var textureRep:Texture;
		
		if( GameControl.Instance.InputType == InputType.JOYSTICK )
		{
			textureLib = charretMenuTexture[2];
			textureRep = charretMenuTexture[3];
		}
		else
		{
			textureLib = charretMenuTexture[0];
			textureRep = charretMenuTexture[1];		
		}
		
		var factor:float = (textureLib.width * 1.0f ) / 1080.0f; 
		Debug.Log("Factor - "+factor);
		
		var bt1:Rect = Rect(Screen.width/2- ( textureLib.width * factor ), Screen.height- (textureLib.height * factor)-10, (textureRep.width * factor ), (textureLib.height * factor));
		var bt2:Rect = Rect(Screen.width/2, Screen.height-(textureRep.height * factor)-10, (textureRep.width * factor ), ( textureRep.height * factor ) );
		
		
		GUI.DrawTexture( bt1, textureLib);
		GUI.DrawTexture( bt2, textureRep);		
	
		var mousePoint:Vector2 = Input.mousePosition;
		mousePoint.y = Screen.height - mousePoint.y;
	
		var bt1Pressed = Input.GetMouseButtonDown(0) && bt1.Contains(mousePoint);
		var bt2Pressed = Input.GetMouseButtonDown(0) && bt2.Contains(mousePoint);
	
		if(Input.GetButtonDown("EspecialKey1") || bt1Pressed)
		{
			Application.LoadLevel("VideoPracaRepLiberdade");
		}
		if(Input.GetButtonDown("EspecialKey2") || bt2Pressed)
		{
			SelectOption(1);
		}
	
	}
}

private function LoadLevel(levelName:String)
{
	carregando.SetActiveRecursively(true);
	show = false;
	yield WaitForSeconds(0.01);
	Application.LoadLevel(levelName);
}

private function BondeActive():void
{
	player.SetActiveRecursively(false);
	if(bonde)
		bonde.SetActiveRecursively(true);
	if(bondePraca)
		bondePraca.SetActiveRecursively(false);
	B_ACTIVE = true;
	
	EnabledBondeMenu(false);
}

private function BondeDeactive():void
{
	player.SetActiveRecursively(true);
	if(bonde)
		bonde.SetActiveRecursively(false);
	if(bondePraca)
		bondePraca.SetActiveRecursively(true);
	B_ACTIVE = false;
	
	if(mouseLookCamBonde != null)
		mouseLookCamBonde.transform.rotation = new Quaternion(.0f,.0f,.0f,1.0f);
}

public function CharretEnabled(status:boolean):void
{
	player.SetActiveRecursively(!status);
	if(charreteAndando)
		charreteAndando.SetActiveRecursively(status);
	if(charreteParada)
		charreteParada.SetActiveRecursively(!status);
				
	EnabledCharreteMenu(false);
}

private function EnabledCharreteMenu(status:boolean):void
{	
	if(mouseLookCamCharret)
		mouseLookCamCharret.enabled = !status;
	if(cameraCharretBlurEffect)
		cameraCharretBlurEffect.enabled = status;
}

private function EnabledBondeMenu(status:boolean):void
{	
	mouseLookCamBonde.enabled = !status;
	cameraBondeBlurEffect.enabled = status;
}

private function ShowMainMenu(state:boolean):void
{
	//state;
}

private function ActiveLevel(activate:boolean):void
{
	if(sceneOne)
		sceneOne.gameObject.SetActiveRecursively(activate);
		
	if(mainMenu)
		mainMenu.gameObject.SetActiveRecursively(!activate);
}

private function helpMenu()
{
	var previousSkin:GUISkin = GUI.skin;

	GUI.skin = configMenu.otherGUISkin;
	
	var backButton:GUIStyle = GUI.skin.GetStyle("BackButton");

	var backRect:Rect = Rect( (Screen.width - backButton.normal.background.width)/2, Screen.height - (Screen.height * 0.1f), backButton.normal.background.width, backButton.normal.background.height );
	var bgRect:Rect = Rect( 0, 0, Screen.width, Screen.height);
	
	if( GameControl.Instance.AspectRatio == ScreenAspectRatio.ASPECTRATIO_4_3 )
	{
		GUI.DrawTexture(bgRect,helpTexture4x3[ GameControl.Instance.InputType ]);
	}
	else
	{
		GUI.DrawTexture(bgRect,helpTexture16x9[ GameControl.Instance.InputType ]);
	}
	
	if( GUI.Button( backRect, "", backButton ) )
	{
		showHelp = false;
	}
		
	GUI.skin = previousSkin;
}