#pragma strict

@script ExecuteInEditMode

//import MegaBook;

public var book:Transform;

public var bookalpha:float = 0F;
public var showPhoto:boolean = false;

private var speed:float = 0.5;
private var currentPage:uint = 1;
private var change:boolean = false;

function OnEnable () {
	currentPage = 1;
	bookalpha = 20;
	change = false;
}

function Update () {

	if(change && currentPage >= 1 && currentPage < 5)
	{
		if(bookalpha < currentPage*20)
		{
			bookalpha += speed;
		}
		else if(bookalpha > currentPage*20)
		{
			bookalpha -= speed;
		}
		else
		{
			change = false;
		}
	}
	
	
	if(book)
		book.SendMessage("SetBookalpha", bookalpha);
		
	if(Input.GetMouseButtonDown(0) && Input.mousePosition.y > 30)
	{
		showPhoto = !showPhoto;
		CheckRaycast();
	}
	
	
}

public function OnGUI():void
{
	/*if(!showPhoto)
	{
		bookalpha = GUI.HorizontalSlider(Rect(0,Screen.height-20,Screen.width,20),bookalpha,0,100);
		book.SendMessage("SetBookalpha", bookalpha);
		GUI.Label(Rect(0,Screen.height-40,40,20), "Capa");
		GUI.Label(Rect(Screen.width-40,Screen.height-40,40,20), "Capa");
		for(var i:uint = 1; i < 7; i++)
		{
			GUI.Label(Rect(i*(Screen.width/7),Screen.height-40,40,20), ""+i+"/"+(i+1));
		}
	}
	else
	{
		GUI.Box(Rect(0,0,Screen.width,Screen.height),"");
		GUI.Box(Rect(Screen.width/2-175,Screen.height/2-150,350,300),"");
	}*/
	
	if(currentPage > 1)
	if(GUI.Button(Rect(Screen.width/2-120,Screen.height-50,110,40),"Página Anterior"))
	{
		currentPage--;
		change = true;
	}
	if(currentPage < 4)
	if(GUI.Button(Rect(Screen.width/2+20,Screen.height-50,110,40),"Pŕoxima Página"))
	{
		currentPage++;
		change = true;
	}
}

public function CheckRaycast():void
{
	var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	var hit : RaycastHit;
	if (Physics.Raycast (ray, hit, 100)) {
	    Debug.DrawLine (ray.origin, hit.point);
	    Debug.Log(hit.transform.name);
	}
}

