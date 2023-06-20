using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainDetailButton : MonoBehaviour
{
    [SerializeField] private Button trainButton;
    [SerializeField] private CameraSelection camScript;

    private RightPanelManager rightPanelMgrScript;
    private GameObject trainToFollow;

    void Start()
    {
        GameObject RightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
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


        TrainMovement trainMovement = trainToFollow.transform.GetComponent<TrainMovement>();
        if (trainMovement.isStationary())
        {
            rightPanelMgrScript.loadCargoPanelInStation(trainToFollow);
        }
    }

    public void setTrainGameObject(GameObject train)
    {
        trainToFollow = train;
    }
}
