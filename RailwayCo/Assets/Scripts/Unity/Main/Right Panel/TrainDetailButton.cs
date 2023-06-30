using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainDetailButton : MonoBehaviour
{
    [SerializeField] private Button _trainButton;
    

    private RightPanelManager _rightPanelMgrScript;
    private GameObject _trainToFollow;

    void Start()
    {
        GameObject RightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        _rightPanelMgrScript = RightPanel.GetComponent<RightPanelManager>();
        _trainButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        
        GameObject assocStation = _trainToFollow.GetComponent<TrainMovement>().CurrentStation;
        _rightPanelMgrScript.LoadCargoPanel(_trainToFollow, assocStation);


        TrainManager trainMgr = _trainToFollow.transform.GetComponent<TrainManager>();
        trainMgr.FollowTrain();

    }

    // Populate the Train button object with the relevant information
    public void SetTrainGameObject(GameObject train)
    {
        _trainToFollow = train;
        this.transform.Find("IconRectangle").GetComponent<Image>().sprite = train.GetComponent<SpriteRenderer>().sprite;
        this.transform.Find("TrainName").GetComponent<Text>().text = train.name;
    }
}
