using UnityEngine;

using UnityEditor;

 

/// <summary>

///   Select the parent object and run this to set all the child objects as

///   active.

/// </summary>

public class SetAllChildObjectsActive

{

    [MenuItem ("GameObject/Set All Child Objects Active")]

    static void SetChildObjectsActive()

    {

        Selection.activeGameObject.SetActive(true);

    }

}