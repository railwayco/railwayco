using System;
using UnityEngine;
using UnityEngine.UI;

public class TrainDetailButton : MonoBehaviour
{
    [SerializeField] private Button _trainCargoButton;
    [SerializeField] private Button _trainRepairButton;
    [SerializeField] private Image _trainIcon;
    [SerializeField] private Text _trainName;
    private TrainController _trainController;
    private Guid _trainGuid;

    private readonly int _coinCost = 1000;

    // Populate the Train button object with the relevant information
    public void SetTrainGameObject(GameObject trainGO)
    {
        _trainController = trainGO.transform.GetComponent<TrainController>();
        _trainGuid = _trainController.TrainGuid;
        Sprite trainSprite = trainGO.GetComponent<SpriteRenderer>().sprite;
        string trainName = trainGO.name;

        _trainIcon.sprite = trainSprite;
        _trainIcon.preserveAspect = true;
        _trainName.text = trainName;
    }

    private void Awake()
    {
        if (!_trainCargoButton) Debug.LogError("Train Cargo Button not attached");
        if (!_trainRepairButton) Debug.LogError("Train Repair Button not attached");
        _trainCargoButton.onClick.AddListener(OnCargoButtonClicked);
        _trainRepairButton.onClick.AddListener(OnRepairButtonClicked);

        if (!_trainIcon) Debug.LogError("Train Icon not attached");
        if (!_trainName) Debug.LogError("Train Name not attached");
    }

    private void OnCargoButtonClicked()
    {
        _trainController.LoadCargoPanelViaTrain();
        _trainController.FollowTrain();
    }

    private void OnRepairButtonClicked()
    {
        CurrencyManager cost = new();
        cost.AddCurrency(CurrencyType.Coin, _coinCost);

        if (!TrainManager.RepairTrain(_trainGuid, cost)) return;

        // Do something when fail
    }
}
