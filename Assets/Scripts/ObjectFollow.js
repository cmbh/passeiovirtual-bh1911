#pragma strict

public var target:Transform;
public var smoothTime:float = 5.0f;
public var minDistance:float = 10.0f;

public var andandoAnim:String = "";
public var paradoAnim:String = "";

function Start () {

}

function Update () {
	if(Vector3.Distance(this.transform.position,target.position) > minDistance)
	{
		//this.transform.position = Vector3.Lerp(this.transform.position,target.position, Time.time*smoothTime);
		this.transform.LookAt(target.position);
		this.transform.Translate(0,0,smoothTime*Time.deltaTime);
		if(!animation.IsPlaying(andandoAnim))
		{
			animation.Play(andandoAnim);
		}
	}
	else if(!animation.IsPlaying(paradoAnim))
	{
		animation.Play(paradoAnim);
	}
}