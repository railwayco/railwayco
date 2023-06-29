using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelection : MonoBehaviour
{
    public GameObject GetMainCamera()
    {
        GameObject[] cameraList = GameObject.FindGameObjectsWithTag("MainCamera");
        if (cameraList.Length == 0)
        {
            Debug.LogError("No Main Camera detected!");
            return null;
        }
        if (cameraList.Length > 1)
        {
            Debug.LogWarning("More than 1 game objects with the tag MainCamera found! Taking the First one");
        }

        return cameraList[0];
    }
}
