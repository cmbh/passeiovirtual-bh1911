enum AnimationMode
{
	ONE_SHOT = 0,
	LOOP = 1,
	PING_PONG
}

public var path : Transform[];

public var animationMode:AnimationMode;

public var speed:float = 0.0001f;
public var percent:float = 0.0f;
public var delayToInit:float = 2.0f;
public var endDelay:float = 15.0f;
public var antecipationPercent:float = .01f;
public var offset:float = .0f;
public var playOnStart:boolean = true;
public var drawPath:boolean = false;
public var idleAnim:String = "";
public var walkAnim:String = "";

public var layerMask:LayerMask;
private var start:boolean = false;
private var pathVect:Vector3[];

public function Start()
{
	Init();
}

public function OnEnable(){
	
	percent = 0;
	if(playOnStart)
		Invoke("PlayMoveWithDelay",delayToInit);
			
	ExecutePath(path);//posiciona o player no inicio do path
}

public function Update():void
{
	if(start)
		ExecutePath(pathVect);
}

public function OnDrawGizmos():void
{
	if(drawPath)
	if(!pathVect)
		iTween.DrawPath(path);
	else
		iTween.DrawPath(pathVect);
}

public function Pause():void
{
	start = false;
	if(animation)
	{
		if(idleAnim.Length > 0)
			animation.CrossFade(idleAnim);
		else
			animation.Stop();
	}
}

public function Resume():void
{
	start = true;
	if(walkAnim.Length > 0)
		animation.CrossFade(walkAnim);
}

private function Init():void
{	
	if(animationMode == AnimationMode.LOOP && path.Length > 0 && path[0] != path[path.Length-1])
	{
		var p:Transform[] = new Transform[path.Length+1];
		path.CopyTo(p,0);
		p[path.Length] = path[0];
		path = p;
		
		//path.SetValue(path[0],path.Length);
		
	}
	percent = 0;
	pathVect = iTweenConstantSpeed.RecalculatePath(path,gameObject);
}

private function PlayMoveWithDelay():void
{
	start = true;
}

private function ExecutePath(currentPath:Transform[]):void
{
	if(pathVect)
		iTween.PutOnPath(gameObject,currentPath, percent);
	
	if(speed > 0 && percent+antecipationPercent < 1)
		transform.LookAt(iTween.PointOnPath(currentPath,percent+antecipationPercent));
	else if(speed < 0 && percent-antecipationPercent > 0)
		transform.LookAt(iTween.PointOnPath(currentPath,percent-antecipationPercent));
		
	var hit : RaycastHit;
	var distanceToGround:float = .0f;
    if (Physics.Raycast (transform.position, -Vector3.up, hit, 1000, layerMask)) {
    	if(!hit.transform.CompareTag("Character"))
    	{
        	distanceToGround = hit.distance;
        	//transform.position.y -= distanceToGround;
        	transform.position.y = hit.point.y+offset;
        }
    }
	
	//if(distanceToGround != 0)
		//transform.position.y = hit.point.y;
	
	percent += speed*Time.deltaTime;
	 
	if(percent > 1 || percent < 0)
	{
		Invoke("OnComplete",endDelay);
		if(endDelay > 0)
		{
			Pause();
		}
	}
}

private function ExecutePath(currentPath:Vector3[]):void
{
	iTween.PutOnPath(gameObject,currentPath, percent);
	
	if(speed > 0 && percent+antecipationPercent < 1)
		transform.LookAt(iTween.PointOnPath(currentPath,percent+antecipationPercent));
	else if(speed < 0 && percent-antecipationPercent > 0)
		transform.LookAt(iTween.PointOnPath(currentPath,percent-antecipationPercent));
		
	var hit : RaycastHit;
	var distanceToGround:float = .0f;
    if (Physics.Raycast (transform.position, -Vector3.up, hit, 1000, layerMask)) {
    	if(!hit.transform.CompareTag("Character"))
    	{
        	distanceToGround = hit.distance;
        	//transform.position.y -= distanceToGround;
        	transform.position.y = hit.point.y+offset;
        }
    }
	
	//if(distanceToGround != 0)
		//transform.position.y = hit.point.y;
	
	percent += speed*Time.deltaTime;
	 
	if(percent > 1 || percent < 0)
	{
		Invoke("OnComplete",endDelay);
		if(endDelay > 0)
		{
			Pause();
		}
	}
}

private function Reset():void{
	if(path.Length > 0)
		transform.position = path[0].position;
	
	start = false;
}

private function OnComplete():void{
	if(endDelay > 0)
	{
		Resume();
	}
	if(animationMode == AnimationMode.PING_PONG)
	{
		speed = -speed;
		percent += speed*Time.deltaTime;
	}
	else if(animationMode == AnimationMode.ONE_SHOT)
	{
		start = false;
		percent = 0;
		if(animation)
		{
			if(idleAnim.Length > 0)
				animation.CrossFade(idleAnim);
			else
				animation.Stop();
		}
	}else if(animationMode == AnimationMode.LOOP)
	{
		percent = 0;
	}
}