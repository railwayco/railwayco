using UnityEngine;
using UnityEngine.UI;

public class CollisionButton : MonoBehaviour
{
    [SerializeField] Button _button;
    private TrainController _caller;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    public void SetCaller(TrainController caller)
    {
        _caller = caller;
    }


    private void OnButtonClicked()
    {
        _caller.TrainCollisionCleanupEnd();
    }
}
