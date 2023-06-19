using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainDetailButton : MonoBehaviour
{
    public Button trainButton;
    public CameraSelection camScript;
    public GameObject CargoPanelPrefab;
    public GameObject CargoCellPrefab;
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


        TrainMovement trainMovement = trainToFollow.transform.GetComponent<TrainMovement>();
        if (trainMovement.TrainState == TrainState.STATION_STOPPED)
        {
            SetupCargoPanel();
        }

        // can use trainMovement.CurrentStation to get station
    }

    public void setTrainGameObject(GameObject train)
    {
        trainToFollow = train;
    }

    private void SetupCargoPanel()
    {
        GameObject RightPanel = GameObject.FindGameObjectWithTag("MainUI").transform.Find("RightPanel").gameObject;
        RightPanel.GetComponent<RightPanelManager>().resetRightPanel();

        GameObject rightSubPanel = Instantiate(CargoPanelPrefab);
        rightSubPanel.transform.SetParent(RightPanel.transform);
        rightSubPanel.transform.localPosition = new Vector3(0, 0, 0);

        Transform trainContainer = rightSubPanel.transform.Find("TrainCargoPanel").transform.Find("Container");
        Transform stationContainer = rightSubPanel.transform.Find("StationCargoPanel").transform.Find("Container");

        ButtonTrainDepart buttonTrainDepart = rightSubPanel.transform.Find("Depart Button").GetComponent<ButtonTrainDepart>();
        buttonTrainDepart.trainToDepart = trainToFollow.GetComponent<BoxCollider2D>();

        GameObject[] cargoList = GameObject.FindGameObjectsWithTag("Cargo");
        for (int i = 0; i < cargoList.Length; i++)
        {
            GameObject trainCargoDetailButton = Instantiate(CargoCellPrefab);
            trainCargoDetailButton.transform.SetParent(trainContainer);
            trainCargoDetailButton.transform.Find("IconRectangle").GetComponent<Image>().sprite = cargoList[i].GetComponent<SpriteRenderer>().sprite;
            trainCargoDetailButton.transform.Find("CargoName").GetComponent<Text>().text = cargoList[i].name;

            GameObject stationCargoDetailButton = Instantiate(CargoCellPrefab);
            stationCargoDetailButton.transform.SetParent(stationContainer);
            stationCargoDetailButton.transform.Find("IconRectangle").GetComponent<Image>().sprite = cargoList[i].GetComponent<SpriteRenderer>().sprite;
            stationCargoDetailButton.transform.Find("CargoName").GetComponent<Text>().text = cargoList[i].name;
        }
        rightSubPanel.transform.localScale = new Vector3(1, 1, 1);
    }
}
