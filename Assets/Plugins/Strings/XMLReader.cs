//using System.IO;

using System.Xml;

using UnityEngine;

using System.Collections;

struct Lang
{
	//public string DEFAUT = "";
	public static string PT_BR = "PT-BR";
	public static string EN = "EN";
}

public class XMLReader : MonoBehaviour
{

    //public TextAsset inFile;
	//private string[] nodeText;
	public static string language = Lang.PT_BR;
	
	private static Hashtable nodeHash = new Hashtable();
 
	private void Awake()
    {
		Load(Lang.PT_BR);
	}
	
	private void Update()
	{
		/*
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			Load(Lang.PT_BR);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			Load(Lang.EN);
		}*/
	}
	
    void Load(string lang)

    {
		if(nodeHash.Count > 0)
			nodeHash.Clear();
		
		language = lang;
		
		//XmlReaderSettings settings = new XmlReaderSettings();
		//settings.IgnoreComments = false;
		//settings.IgnoreProcessingInstructions = true;
		//settings.IgnoreWhitespace = true;
		
		//settings.ValidationType = ValidationType.Schema;
		//settings.Schemas.Add("urn:strings-schema", "assets/Resources/StringXSD.xsd");

        //TextAsset xmlData = new TextAsset();

        TextAsset xmlData = (TextAsset)Resources.Load(lang, typeof(TextAsset));
		//Debug.Log(xmlData.text);
 
        XmlDocument xmlDoc = new XmlDocument();
		//Debug.Log(Application.dataPath);
		xmlDoc.LoadXml(xmlData.text);
		//Debug.Log(Application.persistentDataPath);
		//xmlDoc.Load(Application.dataPath+"/Resources/PT-BR.xml");
		
		//XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc);
		//XmlReader reader = XmlReader.Create(nodeReader, settings);
		// Parse the XML file.
		//while (reader.Read());
		
		//TextReader loader = new TextReader("PT-BR.xml");
		//XmlTextReader reader = new XmlTextReader("assets/Resources/PT-BR.xml");
		//reader.WhitespaceHandling = WhitespaceHandling.None;
		
    	//reader.WhitespaceHandling = WhitespaceHandling.None;
    	//reader.MoveToContent();
    	//reader.Read();
		
    	//reader.Skip(); //Skip the first book.
    	//reader.Skip(); //Skip the second book.
		
		//Debug.Log(reader.Value);
		
		//Debug.Log(reader.ToString());
		
		//xmlDoc.Load(reader);
				
        //xmlDoc.LoadXml(xmlData.text.ToString());
		//XmlNode root = xmlDoc.GetElementsByTagName("strings")[0];
		
		XmlNodeList rootChildNodes = xmlDoc.GetElementsByTagName("string");
		//nodeText = new string[rootChildNodes.Count];
		//int i = 0;
		
		foreach(XmlNode childNode in rootChildNodes)
		{
			//nodeText[i] = childNode.FirstChild.Value.ToString();
			nodeHash.Add(childNode.Attributes["id"].Value.ToString(), childNode.FirstChild.Value.ToString());
			//Debug.Log(childNode.FirstChild.Value.ToString());
			//nodeText.PadRight(nodeText.Length+1);
			//i++;
		}
		
		Resources.UnloadAsset( xmlData );
						
		//reader.Close();
		//Debug.Log(nodeText.Length);
		
		
		//Debug.Log(xmlStrings.ChildNodes[2]);
		
        //XmlNodeList Talents = xmlDoc.GetElementsByTagName("talent");

 

//        foreach (XmlNode talent in Talents)
//
//        {
//
//            //Debug.Log(talent.Attributes["caption"].Value);
//
//        }

    }
	
	private void OnGUI()
	{
		/*for(int i = 0; i < nodeHash.Count; i++)
		{
			GUI.Label(new Rect(10,i*25,300,20),nodeHash[""+i].ToString());
		}*/
		
		//GUI.Label(new Rect(10,Screen.height-60,600,20),Application.dataPath);
		//GUI.Label(new Rect(10,Screen.height-30,600,20),Application.persistentDataPath);
	}
	
	static public string GetString(string index)
	{
		if(nodeHash.Contains(index.ToString()))
			return nodeHash[index.ToString()].ToString();
		
		return "!!nao encontrado!!";
	}
	static public string GetString(int index)
	{
		return GetString(index.ToString());
	}
}

