#pragma strict

//private var show:boolean = false;
//
//function Update () {
//	if(Input.GetKeyDown(KeyCode.I))
//	{
//		show = !show;
//	}
//	//Screen.SetResolution (800, 600, true);
//	
//	/*var resolutions : Resolution[] = Screen.resolutions;
//    // Print the resolutions
//    for (var res:Resolution in resolutions) {
//        print(res.width + "x" + res.height);
//    }
//    // Switch to the lowest supported fullscreen resolution
//    Screen.SetResolution (resolutions[0].width, resolutions[0].height, true);*/
//}
//
//function OnGUI ()
//{
//	if(show)
//	{
//		var i:uint = 0;
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
//	}
//    
//}

public var otherGUISkin:GUISkin;

public var configMenuBg:Texture;
public var configMenuOptionsBg:Texture;
public var configText:Texture;
public var rightButton:Texture;
public var leftButton:Texture;
public var margenPagina:Texture;
public var backTexture:Texture;
private var currentResolution:uint = 0;
public var inputType:uint = 0;
private var resolution:uint = 0;

public var audioListener:AudioListener;

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

public function OnGUI():void
{
	ConfigMenu();
}

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
	if( inputType > 0 )
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
		
	if(inputType < controles.Length - 1)
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
		gameObject.SendMessage("SelectOption",0);
		this.enabled = false;
		//currentMenu = 1;
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