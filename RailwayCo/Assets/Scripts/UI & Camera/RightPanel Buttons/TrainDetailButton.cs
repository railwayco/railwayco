using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainDetailButton : MonoBehaviour
{
    [SerializeField] private Button trainButton;
    

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
        


        TrainMovement trainMovement = trainToFollow.transform.GetComponent<TrainMovement>();
        GameObject assocStation = trainToFollow.GetComponent<TrainMovement>().CurrentStation;
        rightPanelMgrScript.loadCargoPanel(trainToFollow, assocStation);
        trainMovement.followTrain();

    }

    public void setTrainGameObject(GameObject train)
    {
        trainToFollow = train;
    }
}
