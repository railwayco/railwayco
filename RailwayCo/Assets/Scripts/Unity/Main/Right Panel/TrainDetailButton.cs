using UnityEngine;
using UnityEngine.UI;

public class TrainDetailButton : MonoBehaviour
{
    [SerializeField] private Button _trainButton;
    private Train _train;
    private GameObject _trainToFollow;

    // Populate the Train button object with the relevant information
    public void SetTrainGameObject(Train train, GameObject trainGO)
    {
        _train = train;
        _trainToFollow = trainGO;
        this.transform.Find("IconRectangle").GetComponent<Image>().sprite = trainGO.GetComponent<SpriteRenderer>().sprite;
        this.transform.Find("TrainName").GetComponent<Text>().text = trainGO.name;
    }

    private void Awake()
    {
        if (!_trainButton) Debug.LogError("Train Detail Button not attached");
        _trainButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    { 
        _trainToFollow.transform.GetComponent<TrainManager>().LoadCargoPanelViaTrain();
        _trainToFollow.transform.GetComponent<TrainManager>().FollowTrain();
    }
}
