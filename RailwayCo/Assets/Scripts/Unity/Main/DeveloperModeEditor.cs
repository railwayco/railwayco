using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(DeveloperMode))]
public class DeveloperModeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        if (Application.isPlaying)
        {
            if (GUILayout.Button("Add Currencies"))
            {
                DeveloperMode devModeScript = (DeveloperMode)target;
                devModeScript.AddToUserCurrency();
            }
            if (GUILayout.Button("Set Currencies"))
            {
                Debug.Log("Not implemented");
            }
        }

    }
}

