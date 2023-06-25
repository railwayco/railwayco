using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_WorldButton : MonoBehaviour
{
    public Button UI_worldButton;
    public CameraSelection camScript;
    
    private GameObject worldCamera;
    private Vector3 camPos;
    void Start()
    {
        UI_worldButton.onClick.AddListener(OnButtonClicked);

        worldCamera = camScript.getMainCamera();
        if (worldCamera == null)
        {
            Debug.LogError("No World Camera in Scene!");
        }
        camPos = worldCamera.transform.position;
    }

    public void OnButtonClicked()
    {
        worldCamera.transform.position = camPos;
    }
}
