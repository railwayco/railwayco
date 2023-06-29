using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_WorldButton : MonoBehaviour
{
    [SerializeField] private Button _uiWorldButton;
    [SerializeField] private CameraSelection _camScript;
    
    private GameObject worldCamera;
    private Vector3 camPos;
    void Start()
    {
        _uiWorldButton.onClick.AddListener(OnButtonClicked);

        worldCamera = _camScript.GetMainCamera();
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
