using UnityEngine;
using UnityEngine.UI;

public class TrainDetailButton : MonoBehaviour
{
    [SerializeField] private Button _trainCargoButton;
    [SerializeField] private Button _trainRepairButton;
    private TrainController _trainController;

    private int _coinCost = 1000;

    // Populate the Train button object with the relevant information
    public void SetTrainGameObject(GameObject trainGO)
    {
        _trainController = trainGO.transform.GetComponent<TrainController>();
        Sprite trainSprite = trainGO.GetComponent<SpriteRenderer>().sprite;
        string trainName = trainGO.name;

        Transform trainInfo = this.transform.Find("TrainInfo");
        Image trainImage = trainInfo.Find("IconRectangle").GetComponent<Image>();
        trainImage.sprite = trainSprite;
        trainImage.preserveAspect = true;
        trainInfo.Find("TrainName").GetComponent<Text>().text = trainName;
    }

    private void Awake()
    {
        if (!_trainCargoButton) Debug.LogError("Train Cargo Button not attached");
        if (!_trainRepairButton) Debug.LogError("Train Repair Button not attached");
        _trainCargoButton.onClick.AddListener(OnCargoButtonClicked);
        _trainRepairButton.onClick.AddListener(OnRepairButtonClicked);
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

        if (!_trainController.RepairTrain(cost)) return;

        // Do something when fail
    }
}
