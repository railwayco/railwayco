using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This shall be called as a "Button On Click" method
public class UIWorld : MonoBehaviour
{
    // For now, just set the camera to the base state when world is clicked
    public Camera WorldCamera;
    private Vector3 camPos;
    void Start()
    {
        camPos = WorldCamera.transform.position;
    }

    public void renderWorld()
    {
        WorldCamera.transform.position = camPos;
    }
}
