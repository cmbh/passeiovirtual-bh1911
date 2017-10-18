using UnityEngine;
using System.Collections;

public class GameConfig {

	private static GameConfig instance = null;
	
	private Resolution resolution;
	private InputType inputType;
	private float enviromentVolume;
	private float musicVolume;
	private ScreenAspectRatio aspectRatio;
	
	private GameConfig()
	{
		//Se possuir configuraçao de resoluçao salva
		if( PlayerPrefs.HasKey("resolutionW") && PlayerPrefs.HasKey("resolutionW") )
		{
			for( int i = 0; i < Screen.resolutions.Length; i++ )
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
			inputType = (InputType) PlayerPrefs.GetInt("inputType");
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
	
	public static GameConfig Instance {
		get {
			
			if(instance == null )
				instance = new GameConfig();
			
			return instance;
		}
	}

	public InputType InputType {
		get {
			return inputType;
		}
		set {
			inputType = value;
			PlayerPrefs.SetInt("inputType", (int)value);
			PlayerPrefs.Save();
		}
	}

	public Resolution Resolution {
		get {
			return resolution;
		}
		set {
			resolution = value;
			PlayerPrefs.SetInt("resolutionW", value.width);
			PlayerPrefs.SetInt("resolutionH", value.height);
			PlayerPrefs.Save();
			calculateAspectRatio();
		}
	}

	public float EnviromentVolume {
		get {
			return this.enviromentVolume;
		}
		set {
			enviromentVolume = value;
			PlayerPrefs.SetFloat("enviromentVolume", value);
			PlayerPrefs.Save();
		}
	}

	public float MusicVolume {
		get {
			return this.musicVolume;
		}
		set {
			musicVolume = value;
			PlayerPrefs.SetFloat("musicVolume", value);
			PlayerPrefs.Save();
		}
	}
	
	private void calculateAspectRatio()
	{
		float factor = (Screen.width / (Screen.height * 1.0f));
	
		if( factor < 1.5f )
		{
			aspectRatio = ScreenAspectRatio.ASPECTRATIO_4_3;
		}
		else
		{
			aspectRatio = ScreenAspectRatio.ASPECTRATIO_16_9;
		}
	}

	public ScreenAspectRatio AspectRatio {
		get {
			return this.aspectRatio;
		}
	}
}
