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
            DeveloperMode devModeScript = (DeveloperMode)target;
            if (GUILayout.Button("Add Currencies"))
            {
                devModeScript.AddToUserCurrency();
            }
            //if (GUILayout.Button("Set Currencies"))
            //{
            //    Debug.Log("Not implemented");
            //}

            if (GUILayout.Button("Trigger Train Collision Event"))
            {
                devModeScript.TriggerTrainCollisionEvent();
            }
        }

    }
}

