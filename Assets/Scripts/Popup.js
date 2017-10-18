#pragma strict

/*
* @singleton
*/

static public var _instance:Popup;

public var texture:Texture[];
public var skin:GUISkin;
public var fps:int = 12;

public var closeDelay:float = 3.0f;

public var currentIndex:int = 0;

private var initialTime:float = 0;
private var isOpen:boolean = true;
private var show:boolean = false;

private var msg1:String;
private var msg2:String;

private var w:float = .0f;
private var h:float = .0f;

private var canOpen:boolean = true;

public function Start():void
{
	if(texture.Length == 0)
	{
		this.enabled = false;
		return;
	}
	if(!_instance)
	{
		_instance = this;
	}
	else
	{
		Debug.LogWarning("There is another instance of Popup on scene!");
		Destroy(this);
	}
	
	w = texture[0].width;
	h = texture[0].height;
	
	w = w-((w/10)*1.5f);//30%
	h = h-((h/10)*1.5f);//30%
	
	//Open("Av. Augusto de lima","Av. do Paraopeba");
}

/*function Update () {
	
	if(show)
	{
		var currentTime:float = Time.time*fps-initialTime;
		Debug.Log("Entrada "+currentIndex);	
		if(isOpen)
		{
			if(currentIndex < texture.Length-1)
				currentIndex = ( currentTime ) % texture.Length;
			else if(!IsInvoking("Close"))
				Invoke("Close",closeDelay);
		}
		else
		{
			if(currentIndex > 0)
				currentIndex = (texture.Length-1) - ( currentTime ) % texture.Length;
			else
				show = false;
		}
		Debug.Log("Saida "+currentIndex);		
	}
}*/

function OnGUI():void
{	
	if(show)
	{
		Draw(msg1,msg2);
	}
}

public function Draw (txt1:String,txt2:String) {
	/*var w:float = texture[0].width;
	var h:float = texture[0].height;
	
	w = w-((w/10)*3);//30%
	h = h-((h/10)*3);//30%*/
	
	//var posX:float = (w-(w/10))/2;
	//var posY:float = h-(h/10);
	//259.2
	
	GUI.skin = skin;
	
	GUI.color.a = .85f;
	GUI.DrawTexture(Rect(Screen.width-w-40,Screen.height-h-40, w, h),texture[currentIndex]);
	GUI.color.a = 1.0f;
		
	if(currentIndex == texture.Length-1)
	{
		if(txt2 != "")
		{
			//GUI.Label(Rect(Screen.width-w+20,Screen.height-h-5, 300, 35), "Atual");
			var fontSize:float = GUI.skin.GetStyle("label").fontSize;
		
			GUI.Label(Rect(Screen.width-w+20,Screen.height-h+10, 300, 35), txt2);
			
			GUI.skin.GetStyle("label").fontSize = 18;
			GUI.Label(Rect(Screen.width-w+20,Screen.height-h+60, 300, 35), "Atual");
			GUI.Label(Rect(Screen.width-w+20,Screen.height-h+95, 300, 35), txt1);
			GUI.skin.GetStyle("label").fontSize = fontSize;
		}
		else
		{
			//GUI.Label(Rect(Screen.width-w+20,Screen.height-h+30, 300, 35), "Atual");		
			GUI.Label(Rect(Screen.width-w+20,Screen.height-h+50, 300, 35), txt1);
		}
		//GUI.Label(Rect(Screen.width-w-10,Screen.height-h+150, 300, 35), txt3);
	}
}

public function Open(txt1:String,txt2:String):void
{
	if(!show && canOpen)
	{
		msg1 = txt1;
		msg2 = txt2;
	
		show = true;
		isOpen = true;
		InvokeRepeating("UpdateIndex", 0, 1.0f/fps);
		initialTime = Mathf.RoundToInt(Time.time*fps);
		canOpen = false;
		Invoke("DelayToOpenAgain", 5);
	}
}

public function UpdateIndex(): void
{
	if(isOpen)
	{
		if(currentIndex < texture.Length-1)
			currentIndex++;
		else if(!IsInvoking("Close"))
				Invoke("Close",closeDelay);
	}
	else
	{
		if(currentIndex > 0)
			currentIndex--;;
		else
		{
			show = false;
			currentIndex = 0;
			CancelInvoke();
		}
	}
}

public function Close():void
{
	isOpen = false;
	initialTime = Time.time*fps;
}

public function DelayToOpenAgain():void
{
	canOpen = true;
}