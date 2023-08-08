using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance { get; set; }

    [SerializeField] private GameLogic _gameLogic;
    private Coroutine _sendDataToPlayfabCoroutine;
    private GameObject _collisionPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        if (!Instance._gameLogic) Debug.LogError("Game Logic is not attached to the Game Manager!");
        _sendDataToPlayfabCoroutine = StartCoroutine(SendDataToPlayfabRoutine(60f));

        _collisionPanel = GameObject.Find("UI").transform.Find("CollisionPopupCanvas").Find("CollisionPopupPanel").gameObject;
        if (!_collisionPanel) Debug.LogWarning("Collision Panel Cannot be found");
    }

    //////////////////////////////////////////////////////
    /// PLAYFAB RELATED
    //////////////////////////////////////////////////////

    private static IEnumerator SendDataToPlayfabRoutine(float secondsTimeout)
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsTimeout);
            Instance._gameLogic.SendDataToPlayfab();
        }

        // TODO: Graceful termination when signalled by
        // OnApplicationPause or OnApplicationQuit
        // that will be implemented using StopCoroutine
    }

    //////////////////////////////////////////////////////
    /// COLLISION POPUP RELATED
    //////////////////////////////////////////////////////
    
    public static void ActivateCollisionPopup(TrainController train1Ctr, TrainController train2Ctr)
    {
        if (Instance._collisionPanel.activeInHierarchy) return;
        Instance._collisionPanel.SetActive(true);
        CollisionButton collisionBtn = Instance._collisionPanel.transform.Find("OKButton")
                                                                         .GetComponent<CollisionButton>();
        collisionBtn.SetCaller(train1Ctr, train2Ctr);
    }

    public static void DeactivateCollisionPopup()
    {
        if (!Instance._collisionPanel.activeInHierarchy) return;
        Instance._collisionPanel.SetActive(false);
    }
}
