#pragma strict

@script ExecuteInEditMode;

public var skin:GUISkin;
public var otherGUISkin:GUISkin;
public var cmbhLogo:Texture;
public var fadeTexture:Texture;
public var carregando:GameObject;

public var albumObject:GameObject;
public var mainMenuObjects:GameObject;

public var mainCamera:Transform;
public var sceneOne:Transform;
public var mainMenu:Transform;

public var activeFade:boolean = false;

private var currentMenu:uint = 0;
private var buttonSelected:uint = 0;
private var alpha:float = .0F;
private var alphaSpeed:float = .05F;
private var stopMenu:boolean = false;


public var boxBG:Texture;
public var selectionTexture:Texture;
public var logoTopo:Texture;
public var margenPagina:Texture;

public var textureHelp4x3:Texture[];
public var textureHelp16x9:Texture[];

public var buttonFont:Font;
public var book:BookControl;

public var menu:GameObject;
public var anotherCam:GameObject;

private var resolution:uint = 0;

private var configBehavior:Component;

public function Start():void
{

	for(var i:uint = 0; i < Screen.resolutions.Length; i++)
	{
		if(Screen.resolutions[i].width == Screen.currentResolution.width 
		&& Screen.resolutions[i].height == Screen.currentResolution.height)
		{
			currentResolution = resolution = i;
			return;
		}
	}
	
	audioListener.volume = GameControl.Instance.EnviromentVolume;
	audio.volume = GameControl.Instance.MusicVolume;
}

function OnEnable () {

	currentMenu = 0;
	alpha = .0F;
	alphaSpeed = .05F;
	stopMenu = false;
	mainCamera.GetComponent(BlurEffect).enabled = false;
}

function Update () {

	if(activeFade)
	{
		alpha += alphaSpeed;
		if(alpha >= 1.0F)
		{
			//albumObject.SetActiveRecursively(!albumObject.active);
			//mainMenuObjects.SetActiveRecursively(!mainMenuObjects.active);
			activeFade = false;
			alphaSpeed = -alphaSpeed;
		}
		else if(alpha <= 0.0F)
		{
			activeFade = false;
			alphaSpeed = -alphaSpeed;
		}
	}
	
	if(mainCamera.animation["IntroCameraAnimation"].normalizedTime >= 0.98 && currentMenu == 0)
	{
		activeFade = true;
		currentMenu = 1;
		mainCamera.GetComponent(BlurEffect).enabled = true;
		//Debug.Log("Fim1");
	}
	else if(mainCamera.animation.IsPlaying("CameraAnimation"))
	{ 
		//Debug.Log(mainCamera.animation["CameraAnimation"].speed+" - "+currentMenu);
		if(mainCamera.animation["CameraAnimation"].normalizedTime >= 0.98 && currentMenu == 1 && mainCamera.animation["CameraAnimation"].speed == 1)
		{
			activeFade = true;
			alpha = .0f;
			currentMenu = 2;
			this.book.enabled = true;
			//mainCamera.GetComponent(BlurEffect).enabled = true;
			//Debug.Log("Fim2");
		}
		else if(mainCamera.animation["CameraAnimation"].normalizedTime <= 0.1 && currentMenu == 2 && mainCamera.animation["CameraAnimation"].speed == -1)
		{
			activeFade = true;
			alpha = .0f;
			currentMenu = 1;
			mainCamera.GetComponent(BlurEffect).enabled = true;
			//Debug.Log("Fim3");
		}
	}
	UpdateButtonSelected();
	EnableSceneStart();
}

public function OnGUI():void
{
	if(!stopMenu)
	{
		if(alpha > 0)
		{
			GUI.skin = skin;
			GUI.color.a = alpha;
			if(currentMenu == 1)
			{
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
				var buttonLabel:String[] = ["Passeio Virtual", "Álbum de fotos", "Configurações", "Ajuda", "Créditos", "Sair"];
				for(var k:uint = 0; k < 6; k++)
				{
					GUI.Label(Rect(Screen.width/2-150, Screen.height/2-50+k*40, 300, 40),buttonLabel[k]);
					
					if(Screen.width-Input.mousePosition.x > Screen.width/2-150 && Screen.width-Input.mousePosition.x < Screen.width/2+150 && Screen.height-Input.mousePosition.y > Screen.height/2-60+k*40 && Screen.height-Input.mousePosition.y < Screen.height/2-10+k*40)
					{
						buttonSelected = k;
						if(Input.GetMouseButtonUp(0))
						{
							SelectOption(buttonSelected);
						} 
					}
					
				}
				GUI.DrawTexture(Rect(Screen.width/2-150, Screen.height/2-63+buttonSelected*40, 300, 50),selectionTexture);
				GUI.EndGroup();
//				if(GUI.Button(Rect(Screen.width/2-150, Screen.height/2-100, 300, 40),"Passeio virtual"))
//				{
//					//LoadLevel("PRACA_V009");
//					ActiveLevel(true);
//				}
//				if(GUI.Button(Rect(Screen.width/2-150, Screen.height/2-50, 300, 40),"Album de fotos"))
//				{
//					//activeFade = true;
//					alpha = .0f;
//					alphaSpeed = -alphaSpeed;
//					
//					//currentMenu = 0;
//					
//					mainCamera.animation["CameraAnimation"].speed = 1;
//					mainCamera.animation["CameraAnimation"].time = 0;
//					mainCamera.animation.Play("CameraAnimation");
//					mainCamera.GetComponent(BlurEffect).enabled = false;
//				}
//				if(GUI.Button(Rect(Screen.width/2-150, Screen.height/2, 300, 40),"Passeio de bonde"))
//				{
//					MenuGame.B_ACTIVE = true;
//					ActiveLevel(true);
//					//LoadLevel("PRACA_V009");
//				}
//				if(GUI.Button(Rect(Screen.width/2-150, Screen.height/2+50, 300, 40),"Sair"))
//				{
//					Application.Quit();
//				}
				//if(cmbhLogo)
					//GUI.DrawTexture(Rect(Screen.width/2-cmbhLogo.width/2,Screen.height-cmbhLogo.height,cmbhLogo.width,cmbhLogo.height),cmbhLogo);
			}
			else if(currentMenu == 2)
			{
				GUI.DrawTexture(Rect(0,15,Screen.width,Screen.height-30),margenPagina);
				if(GUI.Button(Rect(Screen.width/2-128, Screen.height-60,256,32),"",otherGUISkin.GetStyle("BackButton")))
				{
					//currentMenu = 1;
					this.book.enabled = false;
					
					alpha = .0f;
					alphaSpeed = -alphaSpeed;
					//mainCamera.GetComponent(BlurEffect).enabled = false;
					mainCamera.animation["CameraAnimation"].speed = -1;
					mainCamera.animation["CameraAnimation"].time = mainCamera.animation["CameraAnimation"].length;
					mainCamera.animation.Play("CameraAnimation");
				}
			}
			else if(currentMenu == 3)
			{
				ConfigMenu();
			}
			else if( currentMenu == 4 )
			{
				helpMenu();
			}
		}
		/*else if(alpha > 0)
		{
			GUI.color.a = alpha;
			GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height),fadeTexture);
		}
		else
		{
			if(GUI.Button(Rect(Screen.width-310, 10, 300, 40),"Voltar"))
			{
				activeFade = true;
			}
		}*/
	}
}

private function SelectOption(index:int):void
{
	if(index == 0)
	{
		//anotherCam.SetActiveRecursively(true);		
		//menu.SetActiveRecursively(false);
		
		
		
		LoadLevel("Praca_Liberdade");
		//mainCamera.GetComponent(BlurEffect).enabled = false;
		//ActiveLevel(true);
	}
	else if(index == 1)
	{
		//activeFade = true;
		alpha = .0f;
		alphaSpeed = -alphaSpeed;
		
		//currentMenu = 0;
		mainCamera.GetComponent(BlurEffect).enabled = false;
		mainCamera.animation["CameraAnimation"].speed = 1;
		mainCamera.animation["CameraAnimation"].time = 0;
		mainCamera.animation.Play("CameraAnimation");
		mainCamera.GetComponent(BlurEffect).enabled = false;
	}else if(index == 2)
	{
		//mainCamera.GetComponent(BlurEffect).enabled = false;
		currentMenu = 3;
	}
	else if( index == 3 )
	{
		currentMenu = 4;
	}
	else if(index == 5)
	{
		//Application.Quit();
		System.Diagnostics.Process.GetCurrentProcess().Kill();
	}
}

private function UpdateButtonSelected():void
{
	if(Input.GetKeyDown(KeyCode.W) && buttonSelected > 0)
		buttonSelected--;
	else if(Input.GetKeyDown(KeyCode.S) && buttonSelected < 5)
		buttonSelected++;
	
	if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
	{
		SelectOption(buttonSelected);
	}
		
}

private var helpIndex:int = GameControl.Instance.InputType;

private function helpMenu()
{
	var previousSkin:GUISkin = GUI.skin;

	GUI.skin = otherGUISkin;
	
	var backButton:GUIStyle = otherGUISkin.GetStyle("BackButton");

	var leftRect:Rect = Rect( Screen.width * 0.1f ,Screen.height - (Screen.height * 0.1f), leftButton.width*2, leftButton.height*2 );
	var rightRect:Rect = Rect( Screen.width - (Screen.width * 0.1f) ,Screen.height - (Screen.height * 0.1f), rightButton.width*2, rightButton.height*2 );
	var backRect:Rect = Rect( (Screen.width - backButton.normal.background.width)/2, Screen.height - (Screen.height * 0.1f), backButton.normal.background.width, backButton.normal.background.height );
	var bgRect:Rect = Rect( 0, 0, Screen.width, Screen.height);
	
	if( GameControl.Instance.AspectRatio == ScreenAspectRatio.ASPECTRATIO_4_3 )
	{
		GUI.DrawTexture(bgRect,textureHelp4x3[ helpIndex ]);
	}
	else
	{
		GUI.DrawTexture(bgRect,textureHelp16x9[ helpIndex ]);
	}
	
	if( GUI.Button( leftRect, leftButton ) )
	{
		helpIndex--;
	}
	
	if( GUI.Button( rightRect, rightButton ) )
	{
		helpIndex++;
	}
	
	if( GUI.Button( backRect, "", backButton ) )
	{
		currentMenu = 1;
	}

	if( helpIndex >= textureHelp4x3.Length )
		helpIndex = 0;
		
	if( helpIndex < 0 )
		helpIndex = textureHelp4x3.Length-1;
		
	GUI.skin = previousSkin;
}



private var async : AsyncOperation;

private function LoadLevel(levelName:String)
{
	//mainCamera.GetComponent(BlurEffect).enabled = false;
	carregando.SetActive(true);
	stopMenu = true;
	//yield WaitForSeconds(0.01);
	
	//InvokeRepeating("EnableSceneStart",4,0.5);
	async = Application.LoadLevelAsync (levelName);
    
    async.allowSceneActivation = false;
    
    yield async;
}

public function EnableSceneStart():void
{
	if(async && async.progress >= .9f && carregando.active)
	{
		async.allowSceneActivation = true;
	}
}

private function ActiveLevel(activate:boolean):void
{
	if(sceneOne)
		sceneOne.gameObject.SetActiveRecursively(activate);
		
	if(mainMenu)
		mainMenu.gameObject.SetActiveRecursively(!activate);
}

public var configMenuBg:Texture;
public var configMenuOptionsBg:Texture;
public var configText:Texture;
public var rightButton:Texture;
public var leftButton:Texture;
private var currentResolution:uint = 0;
private var inputType:uint = 0;
public var audioListener:AudioListener;
private function ConfigMenu():void
{
	GUI.skin = otherGUISkin;
	var i:uint = 0;
		
	var inputType:int = parseInt( GameControl.Instance.InputType );
		
	var names : String[] = QualitySettings.names;
	var resolutions : Resolution[] = Screen.resolutions;
	
	var controles:String[] = ["Teclado","Teclado+Mouse","Joystick"];
	    
	//backgroud
	GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height),configMenuBg);
	//margem
	GUI.DrawTexture(Rect(0,15,Screen.width,Screen.height-30),margenPagina);
	
	GUI.BeginGroup(Rect(Screen.width/2-256, Screen.height/2-237, 512, 474));
	
	GUI.DrawTexture(Rect(0, 0, 512, 64),configText);
	
	GUI.DrawTexture(Rect(0, 74, 512, 400),configMenuOptionsBg);
	
	GUI.BeginGroup(Rect(40,120,472,400));
	
	//GUILayout.BeginVertical ();
	
	//GUI.skin.label.font = null;
	GUI.skin.label.fontSize = 20;
	
	
	if(resolutions.Length > 0)
	{
		//GUILayout.BeginHorizontal();
		GUI.Label(Rect(0,5,150,30),"RESOLUÇÃO:");
		if(currentResolution > 0)
		{
			GUI.DrawTexture(Rect(160,0,32,32),leftButton);
			if(GUI.Button(Rect(160,0,32,32),""))
				currentResolution--;
		}
		//Debug.Log(resolutions[Screen.currentResolution]);
		//if(currentResolution < resolutions.Length)
			//GUI.Label(Rect(210,0,150,30),(""+resolutions[currentResolution].width + "x" + resolutions[currentResolution].height));
		
		if(currentResolution < resolutions.Length-1)
		{
			GUI.DrawTexture(Rect(330,0,32,32),rightButton);
			if(GUI.Button(Rect(330,0,32,32),""))
				currentResolution++;
		}
		
		//GUILayout.EndHorizontal();
	}
	GUI.skin = null;
	if( resolution != currentResolution && GUI.Button(Rect(365,0,60,30),"Aplicar"))
	{
		Screen.SetResolution(resolutions[currentResolution].width,resolutions[currentResolution].height,true);
		GameControl.Instance.Resolution =  resolutions[currentResolution];
		resolution = currentResolution;
	}
	GUI.skin = otherGUISkin;
		
	
	//GUILayout.BeginHorizontal();
	GUI.Label(Rect(0,45,150,30),"QUALIDADE:");
	if(QualitySettings.currentLevel > 0)
	{
		GUI.DrawTexture(Rect(160,40,32,32),leftButton);
		if(GUI.Button(Rect(160,40,32,32),""))
			QualitySettings.DecreaseLevel();
	}
	
	//GUI.Label(Rect(210,40,150,30),names[QualitySettings.currentLevel]);
	if(names.Length-1 > QualitySettings.currentLevel)
	{
		GUI.DrawTexture(Rect(330,40,32,32),rightButton);
		if(GUI.Button(Rect(330,40,32,32),""))
			QualitySettings.IncreaseLevel();
	}
	//GUILayout.EndHorizontal();
	
	//GUILayout.BeginHorizontal();
	GUI.Label(Rect(0,85,150,30),"CONTROLES:");
	if( inputType > 0)
	{
		GUI.DrawTexture(Rect(160,80,32,32),leftButton);
		if(GUI.Button(Rect(160,80,32,32),""))
		{
			// GameControl.input_type--;
			inputType--;
			GameControl.Instance.InputType = inputType;
		}
	}
	
	
	//GUI.Label(Rect(210,40,150,30),names[QualitySettings.currentLevel]);
	//GUI.Label(Rect(210,80,150,30),controles[inputType]);
		
	if( inputType < controles.Length-1)
	{
		GUI.DrawTexture(Rect(330,80,32,32),rightButton);
		if(GUI.Button(Rect(330,80,32,32),""))
		{
			//GameControl.input_type++;
			inputType++;
			GameControl.Instance.InputType = inputType;
		}
	}
	//GUILayout.EndHorizontal();
	
	//GUILayout.BeginVertical();
	var fontSize:uint = GUI.skin.label.fontSize;
	GUI.Label(Rect(0,125,80,30),"AUDIO:");
	//GUILayout.BeginHorizontal();
	GUI.skin.label.fontSize = 12;
	
	GUI.Label(Rect(210,50,150,30),names[QualitySettings.currentLevel]);
	GUI.Label(Rect(210,90,150,30),controles[ parseInt( GameControl.Instance.InputType ) ]);
	if(currentResolution < resolutions.Length)
		GUI.Label(Rect(210,10,150,30),(""+resolutions[currentResolution].width + "x" + resolutions[currentResolution].height));
	
	GUI.Label(Rect(0,170,150,20),"Ambiente");
	var aux:float = GUI.HorizontalSlider(Rect(130,160,128,16),audioListener.volume,-.15f,1.15f);
	audioListener.volume = Mathf.Clamp(aux,.0f,1.0f);
	if( audioListener.volume != GameControl.Instance.EnviromentVolume )
	{
		GameControl.Instance.EnviromentVolume = audioListener.volume;
	}
	
	//GUILayout.EndHorizontal();
	//GUILayout.BeginHorizontal();
	
	GUI.Label(Rect(0,210,150,20),"Musica");
	aux = GUI.HorizontalSlider(Rect(130,200,128,16),audio.volume,-.15f,1.15f);
	audio.volume = Mathf.Clamp(aux,.0f,1.0f);
	if( audio.volume != GameControl.Instance.MusicVolume ) 
	{
		GameControl.Instance.MusicVolume = audio.volume;
	}
	//GUILayout.EndHorizontal();
	//GUILayout.EndVertical();
	GUI.skin.label.fontSize = fontSize;
	//GUILayout.BeginHorizontal();
//	GUI.Label(Rect(0,250,200,30),"SENSIBILIDADE DA CÂMERA");
//	GUI.HorizontalSlider(Rect(450,250,150,40),0,0,1);
	//GUILayout.EndHorizontal();
	
	//GUILayout.EndVertical ();
	
	//GUI.DrawTexture(Rect(98,300,256,32),backTexture);
	
	//GUI.color.a = 0.0001f;
	if(GUI.Button(Rect(98,300,256,32),"",GUI.skin.GetStyle("BackButton")))
	{
		//gameObject.SendMessage("SelectOption",0);
		//this.enabled = false;
		currentMenu = 1;
	}
	//GUI.color.a = 1.0f;
	
//	GUI.DrawTexture(Rect(Screen.width/2-128,Screen.height-42,256,32),backTexture);
//	
//	GUI.color.a = 0.0001f;
//	if(GUI.Button(Rect(Screen.width/2-128,Screen.height-42,256,32),""))
//	{
//		gameObject.SendMessage("SelectOption",0);
//		this.enabled = false;
//	}
//	GUI.color.a = 1.0f;
	
	GUI.EndGroup();
	
	GUI.EndGroup();
	
	
	
//	var i:uint = 0;
//		
//	    var names : String[] = QualitySettings.names;
//	    var resolutions : Resolution[] = Screen.resolutions;
//	    
//	    GUILayout.BeginHorizontal ();
//	    
//	    GUILayout.BeginVertical ();
//	    for (i = 0; i < names.Length; i++)
//	    {
//	        if (GUILayout.Button (names[i]))
//	            QualitySettings.SetQualityLevel (i, true);
//	    }
//	    
//	    GUILayout.EndVertical ();
//	    GUILayout.BeginVertical ();
//	    
//	    for (var res:Resolution in resolutions) {
//	        if(GUILayout.Button((""+res.width + "x" + res.height)))
//	        {
//	        	Screen.SetResolution (res.width, res.height, true);
//	        }
//	    }
//	    
//	    GUILayout.EndVertical ();
//	    
//	    GUILayout.BeginVertical ();
//	    
//	   	for(i = 1; i <= 3; i++)
//	   	{
//	   		if(GUILayout.Button("Input Type: "+i))
//	   		{
//	   			GameControl.input_type = i;
//	   		}
//	   	}
//	    
//	    GUILayout.EndVertical ();
//	    GUILayout.EndHorizontal ();
}