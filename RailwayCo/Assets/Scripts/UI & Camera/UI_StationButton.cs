using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StationButton : MonoBehaviour
{
    public Button UI_StationsButton;
    public GameObject RightPanel;
    public GameObject RightSubPanelPrefab;
    public GameObject StationCellPrefab;

    void Start()
    {
        UI_StationsButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        RightPanel.GetComponent<RightPanelManager>().resetRightPanel();

        GameObject rightSubPanel = Instantiate(RightSubPanelPrefab);
        rightSubPanel.transform.SetParent(RightPanel.transform);
        rightSubPanel.transform.localPosition = new Vector3(0, 0, 0);
        Transform container = rightSubPanel.transform.Find("Container");

        GameObject[] stationList = GameObject.FindGameObjectsWithTag("Station");
        for (int i = 0; i < stationList.Length; i++)
        {
            GameObject stationDetailButton = Instantiate(StationCellPrefab);
            stationDetailButton.transform.SetParent(container);

            stationDetailButton.transform.Find("IconRectangle").GetComponent<Image>().sprite = stationList[i].GetComponent<SpriteRenderer>().sprite;
            stationDetailButton.transform.Find("StationName").GetComponent<Text>().text = stationList[i].name;
            stationDetailButton.GetComponent<StationDetailButton>().setStationGameObject(stationList[i]);
        }
        rightSubPanel.transform.localScale = new Vector3(1, 1, 1);

    }
}
