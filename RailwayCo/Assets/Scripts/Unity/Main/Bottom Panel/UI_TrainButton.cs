using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TrainButton : MonoBehaviour
{
    [SerializeField] private Button _uiTrainsButton;
    [SerializeField] private GameObject _rightPanel;
    [SerializeField] private GameObject _rightSubPanelPrefab;
    [SerializeField] private GameObject _trainCellPrefab;


    void Start()
    {
        _uiTrainsButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {

        _rightPanel.GetComponent<RightPanelManager>().ResetRightPanel();

        GameObject rightSubPanel = Instantiate(_rightSubPanelPrefab);
        rightSubPanel.transform.SetParent(_rightPanel.transform);
        rightSubPanel.transform.localPosition = new Vector3(0, 0, 0);
        Transform container = rightSubPanel.transform.Find("Container");

        GameObject[] trainList = GameObject.FindGameObjectsWithTag("Train");

        for (int i = 0; i < trainList.Length; i++)
        {
            GameObject trainDetailButton = Instantiate(_trainCellPrefab);
            trainDetailButton.transform.SetParent(container);


            trainDetailButton.transform.Find("IconRectangle").GetComponent<Image>().sprite = trainList[i].GetComponent<SpriteRenderer>().sprite;
            trainDetailButton.transform.Find("TrainName").GetComponent<Text>().text = trainList[i].name;

            trainDetailButton.GetComponent<TrainDetailButton>().SetTrainGameObject(trainList[i]);
        }
        rightSubPanel.transform.localScale = new Vector3(1, 1, 1);
        //if (trainList.) Lengeth is 0 show a special panel else show all the trains and then have the camera point to them

    }
}
