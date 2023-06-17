using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TrainButton : MonoBehaviour
{
    public Button UI_trainsButton;
    public GameObject RightPanel;
    public GameObject RightSubPanelPrefab;
    public GameObject TrainCellPrefab;


    void Start()
    {
        UI_trainsButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        
        RightPanel.GetComponent<RightPanelManager>().resetRightPanel();

        GameObject rightSubPanel = Instantiate(RightSubPanelPrefab);
        rightSubPanel.transform.SetParent(RightPanel.transform);
        rightSubPanel.transform.localPosition = new Vector3(0, 0, 0);
        Transform container = rightSubPanel.transform.Find("Container");

        GameObject[] trainList = GameObject.FindGameObjectsWithTag("Train");

        for (int i=0; i< trainList.Length; i++)
        {
            GameObject trainDetailButton = Instantiate(TrainCellPrefab);
            trainDetailButton.transform.SetParent(container);


            trainDetailButton.transform.Find("IconRectangle").GetComponent<Image>().sprite = trainList[i].GetComponent<SpriteRenderer>().sprite;
            trainDetailButton.transform.Find("TrainName").GetComponent<Text>().text = trainList[i].name;

            trainDetailButton.GetComponent<TrainDetailButton>().setTrainGameObject(trainList[i]);
        }

        rightSubPanel.transform.localScale = new Vector3(1, 1, 1);
        //if (trainList.) Lengeth is 0 show a special panel else show all the trains and then have the camera point to them
    }
}
