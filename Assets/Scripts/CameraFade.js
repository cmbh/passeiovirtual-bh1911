#pragma strict

public var fadeTexture:Texture;

private var show:boolean = false;

private var alpha:float = .0f;
private var alphaTo:float = 1.0f;

function Start () {

}

function Update () {
	alpha = Mathf.Lerp(alpha, alphaTo, Time.time);
	
	if(alpha > 1)
	{
		show = false;
	}
}

public function OnGUI():void
{
	if(show)
	{
		GUI.color.a = alpha;
		GUI.DrawTexture(Rect(0,0,Screen.width,Screen.height), fadeTexture);
	}
}

public function StartFade():void
{
	show = true;
}