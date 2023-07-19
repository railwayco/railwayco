using UnityEngine;
using UnityEngine.UI;

public class CollisionButton : MonoBehaviour
{
    [SerializeField] Button _button;
    private TrainManager _caller;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    public void SetCaller(TrainManager caller)
    {
        _caller = caller;
    }


    private void OnButtonClicked()
    {
        _caller.TrainCollisionCleanupEnd();
    }
}
