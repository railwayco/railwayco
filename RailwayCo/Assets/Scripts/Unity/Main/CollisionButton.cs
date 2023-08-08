using UnityEngine;
using UnityEngine.UI;

public class CollisionButton : MonoBehaviour
{
    [SerializeField] Button _button;
    private TrainController _train1;
    private TrainController _train2;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    public void SetCaller(TrainController train1, TrainController train2)
    {
        _train1 = train1;
        _train2 = train2;
    }

    private void OnButtonClicked()
    {
        _train1.TrainCollisionCleanupEnd();
        _train2.TrainCollisionCleanupEnd();
    }
}
