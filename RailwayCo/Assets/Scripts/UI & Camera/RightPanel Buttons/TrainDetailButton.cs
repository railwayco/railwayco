using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainDetailButton : MonoBehaviour
{
    public Button trainButton;
    public CameraSelection camScript;
    private GameObject trainToFollow;
    
    void Start()
    {
        trainButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        GameObject worldCamera = camScript.getMainCamera();
            if (worldCamera == null)
        {
            Debug.LogError("No World Camera in Scene!");
        }

        worldCamera.GetComponent<WorldCameraMovement>().followtrain(trainToFollow);
    }

    public void setTrainGameObject(GameObject train)
    {
        trainToFollow = train;
    }
}
