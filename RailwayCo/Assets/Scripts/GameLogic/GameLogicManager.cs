using UnityEngine;

[CreateAssetMenu(fileName = "GamelogicScriptableObject", menuName = "ScriptableObjects/Gamelogic")]
public class GameLogicManager : ScriptableObject
{
    private User user;
    private StationMaster stationMaster;
    private PlayfabManager playfabManager;

    public User User { get => user; private set => user = value; }
    public StationMaster StationMaster { get => stationMaster; private set => stationMaster = value; }
    public PlayfabManager PlayfabManager { get => playfabManager; private set => playfabManager = value; }

    public void InitGameData(User user, StationMaster stationMaster)
    {
        User = user;
        StationMaster = stationMaster;
    }
}
