#pragma strict

public var pageTexture:Texture[];
public var currentPage:uint = 0;
public var page1:Renderer;
public var page2:Renderer;
public var page3:Renderer;

public var pageFlip:AudioClip;

public var skin:GUISkin;
public var target:Transform;

private var origin:Vector3;

function Awake () {
//	9.332945 0.08600903 -0.02682018 
//	10.21844 -0.09279609 -0.02866578
	/*page1.materials[0].color = Color.red;
	page1.materials[1].color = Color.blue;
	page2.materials[0].color = Color.green;
	page3.materials[1].color = Color.grey;*/
	
	//page2.materials[1].mainTexture = pageTexture[2];
	//page1.materials[0].mainTexture = pageTexture[1];
	//page1.materials[1].mainTexture = pageTexture[3];
	//page3.materials[1].mainTexture = pageTexture[4];
	
	currentPage = 2;
	
	enabled = false;
	
	/*<<
	page2.materials[1].mainTexture = page1.materials[0].mainTexture;
	page2.materials[1].mainTexture = "nova";
	
	>>
	page2.materials[1].mainTexture = page1.materials[0].mainTexture;//verde recebe vermelho
	page1.materials[1].mainTexture = page3.materials[1].mainTexture;//azul recebe preto
	page3.materials[1].mainTexture = "nova";//preto recebe nova*/
	origin = Camera.main.WorldToViewportPoint(target.position);
	origin = Camera.main.ViewportToScreenPoint(origin);
	
	//origin.x = Screen.width - origin.x;
	origin.y = Screen.height - origin.y;
}

function Update () {
	origin = Camera.main.WorldToViewportPoint(target.position);
	origin = Camera.main.ViewportToScreenPoint(origin);
	
	//origin.x = Screen.width - origin.x;
	origin.y = Screen.height - origin.y;
	Debug.Log(origin);
}

public function OnGUI():void
{
	
	if(!animation.IsPlaying("Take 001"))
	{
		if(currentPage > 1 && GUI.Button(Rect(origin.x - 110,origin.y-40,100,55),"",skin.GetStyle("Anterior")))
		{
			SetPage(currentPage-2);
		
			animation["Take 001"].speed = -Mathf.Abs(animation["Take 001"].speed);
			animation["Take 001"].time = 1.0f;
			animation.Play();
			audio.PlayOneShot(pageFlip);
		}
		if(currentPage < pageTexture.Length-2 && GUI.Button(Rect(origin.x + 10,origin.y-40,100,55),"",skin.GetStyle("Proximo")))
		{
			SetPage(currentPage+2);
		
			animation["Take 001"].speed = Mathf.Abs(animation["Take 001"].speed);
			animation["Take 001"].time = 0.0f;
			animation.Play();
			audio.PlayOneShot(pageFlip);
			
		}
	}
}

public function SetPage(index:int):void
{
	if(index < pageTexture.Length && index >= 0)
	{
		//3,4  >> 5,6 >> 7,8
		if(index > currentPage)
		{
			if(animation["Take 001"].speed > .0f)
			{
				page2.materials[0].mainTexture = page1.materials[0].mainTexture;
				page1.materials[1].mainTexture = page3.materials[1].mainTexture;
				page1.materials[0].mainTexture = pageTexture[index];
				page3.materials[1].mainTexture = pageTexture[index+1];
			}
		}
		else
		{
//			page2.material.mainTexture = pageTexture[index];
//			page1.materials[0].mainTexture = pageTexture[index-1];
//			page1.materials[1].mainTexture = pageTexture[index-2];
//			page3.material.mainTexture = pageTexture[index-3];
			if(animation["Take 001"].speed < .0f)
			{
			
				page3.materials[1].mainTexture = page1.materials[1].mainTexture;
				page1.materials[0].mainTexture = page2.materials[0].mainTexture;
				
				page1.materials[1].mainTexture = pageTexture[index+1];
				page2.materials[0].mainTexture = pageTexture[index];
			}
		}		
		currentPage = index;
	}
	Debug.Log(currentPage+" "+animation["Take 001"].time);
}