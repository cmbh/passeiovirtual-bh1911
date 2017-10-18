#pragma strict

public var texture:Texture[];

public var animationMode:AnimationMode = AnimationMode.LOOP;
public var reverse:boolean = false;
public var playOnAwake:boolean = false;

public var fps:int = 12;

public var drawOnGUI:boolean = false;

private var isPlaying:boolean = false;
private var isReverse:boolean = false;

private var currentIndex:uint = 0;

private var initialTime:float = 0;

public function Awake():void
{
	if(texture.Length == 0)
	{
		this.enabled = false;
		return;
	}
	
	if(playOnAwake)
		Play();

}

function Update () {
			
	if(isPlaying)
	{
		var currentTime:float = Time.time*fps-initialTime;
		
		if(!isReverse)
		{
			if(currentIndex <= texture.Length-1)
			{
				currentIndex = ( currentTime ) % texture.Length;
			}
			else if(animationMode == AnimationMode.LOOP)
			{
				currentIndex = 0;
				initialTime = Time.time*fps;
			}
			else if(animationMode == AnimationMode.PING_PONG)
			{
				isReverse = true;
				currentIndex = (texture.Length-1);
				initialTime = Time.time*fps;
			}
			else
			{
				Pause();
			}				
		}
		else
		{
			if(currentIndex >= 0)
			{
				currentIndex = (texture.Length-1) - ( currentTime ) % texture.Length;
				
			}
			else if(animationMode == AnimationMode.LOOP)
			{
				currentIndex = (texture.Length-1);
				initialTime = Time.time*fps;
			}
			else if(animationMode == AnimationMode.PING_PONG)
			{
				isReverse = false;
				currentIndex = 0;
				initialTime = Time.time*fps;
			}
			else
			{
				Pause();
			}
		}
		
		if(!drawOnGUI)	
			renderer.material.mainTexture = texture[currentIndex];
	
	}
}

public function Play():void
{
	if(!isPlaying)
	{
		isPlaying = true;
		
		initialTime = Time.time*fps;
		isReverse = reverse;
		
		if(!reverse)
		{
			currentIndex = 0;
		}
		else {
			currentIndex = texture.Length-1;
			initialTime -= texture.Length-currentIndex;
		}
		
		if(!drawOnGUI)
			renderer.material.mainTexture = texture[currentIndex];
	}
}

public function Stop():void
{
	isPlaying = false;
	currentIndex = 0;
	
	if(!drawOnGUI)
		renderer.material.mainTexture = texture[currentIndex];
}

public function Pause():void
{
	isPlaying = false;
}

public function Resume():void
{
	isPlaying = true;
	initialTime = Time.time*fps;
	initialTime -= texture.Length-currentIndex;
	
	if(!drawOnGUI)
		renderer.material.mainTexture = texture[currentIndex];
}

public function GotoAndStop(frame:uint)
{
	Pause();
	
	currentIndex = frame;
		
	if(!drawOnGUI)
		renderer.material.mainTexture = texture[currentIndex];
}

public function GetCurrentFrame():uint
{
	return currentIndex;
}

public function IsPlaying():boolean
{
	return isPlaying;
}

public function OnGUI():void
{
	if(drawOnGUI)
	{
		var native_width:float = 1920;
		var native_height:float = 1080;
		
		var factor:float = Screen.width / native_width; 
		//var resY:float = Screen.height / native_height;
		
		//GUI.matrix = Matrix4x4.TRS (Vector3(0, 0, 0), Quaternion.identity, Vector3 (resX, resY, 1));
		var rectText:Rect = Rect( ((native_width - (texture[currentIndex].width *0.7) )/2) * factor, ( ( native_height - (texture[currentIndex].height*0.7) )/2 ) * factor, (texture[currentIndex].width * 0.7) * factor, (texture[currentIndex].height * 0.7) * factor  );
		
		GUI.DrawTexture( rectText, texture[currentIndex]);
		//GUI.DrawTexture(Rect( native_width/2-texture[currentIndex].width/2*0.7,native_height/2-texture[currentIndex].height/2*0.7-50,texture[currentIndex].width*0.7, texture[currentIndex].height*0.7),texture[currentIndex]);
	}
}


