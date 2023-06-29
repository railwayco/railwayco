using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StationButton : MonoBehaviour
{
    [SerializeField] private Button _uiStationsButton;
    [SerializeField] private GameObject _rightPanel;
    [SerializeField] private GameObject _rightSubPanelPrefab;
    [SerializeField] private GameObject _stationCellPrefab;

    void Start()
    {
        _uiStationsButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        _rightPanel.GetComponent<RightPanelManager>().ResetRightPanel();

        GameObject rightSubPanel = Instantiate(_rightSubPanelPrefab);
        rightSubPanel.transform.SetParent(_rightPanel.transform);
        rightSubPanel.transform.localPosition = new Vector3(0, 0, 0);
        Transform container = rightSubPanel.transform.Find("Container");

        GameObject[] stationList = GameObject.FindGameObjectsWithTag("Station");
        for (int i = 0; i < stationList.Length; i++)
        {
            GameObject stationDetailButton = Instantiate(_stationCellPrefab);
            stationDetailButton.transform.SetParent(container);

            stationDetailButton.transform.Find("IconRectangle").GetComponent<Image>().sprite = stationList[i].GetComponent<SpriteRenderer>().sprite;
            stationDetailButton.transform.Find("StationName").GetComponent<Text>().text = stationList[i].name;
            stationDetailButton.GetComponent<StationDetailButton>().SetStationGameObject(stationList[i]);
        }
        rightSubPanel.transform.localScale = new Vector3(1, 1, 1);

    }
}
