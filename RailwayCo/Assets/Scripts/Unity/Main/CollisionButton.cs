using System;
using UnityEngine;
using UnityEngine.UI;

public class CollisionButton : MonoBehaviour
{
    [SerializeField] Button _button;
    private GameObject _train1;
    private Guid _train1Guid;
    private GameObject _train2;
    private Guid _train2Guid;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    public void SetCaller(GameObject train1, GameObject train2)
    {
        _train1 = train1;
        _train1Guid = train1.GetComponent<TrainController>().TrainGuid;
        _train2 = train2;
        _train2Guid = train2.GetComponent<TrainController>().TrainGuid;
    }

    private void OnButtonClicked()
    {
        TrainCollisionCleanup(_train1Guid, _train1);
        TrainCollisionCleanup(_train2Guid, _train2);
    }

    private static void TrainCollisionCleanup(Guid trainGuid, GameObject train)
    {
        TrainManager.OnTrainCollision(trainGuid);
        Destroy(train);
        GameManager.DeactivateCollisionPopup();
    }
}
