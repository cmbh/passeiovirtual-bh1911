#pragma strict

/*
* @Singleton
*/

//static public var _instance:GameControl = GameControl.Instance;
/*
static public var input_type:uint = 2;

function Start () {
	if(_instance != null)
	{
		Debug.LogWarning("There is another instance of GameControl on scene!");
		Destroy(this);
		return;
	}
	
	_instance = this;
}

function Pause () {
	Time.timeScale = 0;
}

function Resume() {
	Time.timeScale = 1;
}
*/
public enum InputType
{	
	KEYBOARD,
	KEYBOARD_MOUSE,
	JOYSTICK
};

public enum ScreenAspectRatio
{
	ASPECTRATIO_4_3,
	ASPECTRATIO_16_9,
	ASPECTRATIO_16_10
};

public class GameControl
{
	private static var instance:GameControl = null;
	
	private var resolution:Resolution;
	private var inputType:InputType;
	private var enviromentVolume:float;
	private var musicVolume:float;
	private var aspectRatio:ScreenAspectRatio;

	private function GameControl()
	{
		//Se possuir configuraçao de resoluçao salva
		if( PlayerPrefs.HasKey("resolutionW") && PlayerPrefs.HasKey("resolutionW") )
		{
			for( var i:int = 0; i < Screen.resolutions.Length; i++ )
			{
				if( PlayerPrefs.GetInt("resolutionW") == Screen.resolutions[i].width &&
					PlayerPrefs.GetInt("resolutionH") == Screen.resolutions[i].height )
				{
					resolution = Screen.resolutions[i];
					Screen.SetResolution( Screen.resolutions[i].width, Screen.resolutions[i].height, true );
					i = Screen.resolutions.Length;
				}
				
			}
			
			calculateAspectRatio();
			
		}
		else
		{
			this.Resolution = Screen.currentResolution;
		}
		
		//Se possui configuraçao de input Salva
		if( PlayerPrefs.HasKey("inputType") )
		{
			inputType = PlayerPrefs.GetInt("inputType");
		}
		else
		{
			this.InputType = InputType.KEYBOARD;
		}
		
		//Se possuir configuraçao de barulho ambiente
		if( PlayerPrefs.HasKey("enviromentVolume") )
		{
			enviromentVolume = PlayerPrefs.GetFloat("enviromentVolume");
		}
		else
		{
			EnviromentVolume = 0.5f;
		}
		
		//Se possuir configuraçao de musica
		if( PlayerPrefs.HasKey("musicVolume") )
		{
			musicVolume = PlayerPrefs.GetFloat("musicVolume");
		}
		else
		{
			MusicVolume = 0.5f;
		}
	}

	public function get Resolution():Resolution
	{
		return resolution;
	}
	
	public function set Resolution( value:Resolution ):void
	{
		resolution = value;
		PlayerPrefs.SetInt("resolutionW", value.width);
		PlayerPrefs.SetInt("resolutionH", value.height);
		PlayerPrefs.Save();
		calculateAspectRatio();
	}
	
	public function get InputType():InputType
	{
		return inputType;
	}
	
	public function set InputType( value:InputType ):void
	{
		inputType = value;
		PlayerPrefs.SetInt("inputType", parseInt(value) );
		PlayerPrefs.Save();
	}
	
	public function get EnviromentVolume():float
	{
		return enviromentVolume;
	}
	
	public function set EnviromentVolume( value:float ):void
	{
		enviromentVolume = value;
		PlayerPrefs.SetFloat("enviromentVolume", value);
		PlayerPrefs.Save();
	}
	
	public function get MusicVolume():float
	{
		return musicVolume;
	}
	
	public function set MusicVolume( value:float ):void
	{
		musicVolume = value;
		PlayerPrefs.SetFloat("musicVolume", value);
		PlayerPrefs.Save();
	}
	
	public function get AspectRatio():ScreenAspectRatio
	{
		return aspectRatio;
	}
	
	private function calculateAspectRatio():void
	{
		var factor:float = (Screen.width / (Screen.height * 1.0f));
	
		if( factor < 1.5f )
		{
			aspectRatio = ScreenAspectRatio.ASPECTRATIO_4_3;
		}
		else
		{
			aspectRatio = ScreenAspectRatio.ASPECTRATIO_16_9;
		}
	}
	
	public function Pause () 
	{
		Time.timeScale = 0;
	}

	public function Resume() 
	{
		Time.timeScale = 1;
	}
	
	public static function get Instance():GameControl
	{
		if( instance == null )
			instance = new GameControl();
	
		return instance;
	}
};