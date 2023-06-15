using PlayFab;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayfabScriptableObject", menuName = "ScriptableObjects/Playfab")]
public class PlayfabManager : ScriptableObject
{
    private AuthManager authManager;
    private GameDataManager gameDataManager;

    public AuthManager AuthManager { get => authManager; private set => authManager = value; }
    public GameDataManager GameDataManager { get => gameDataManager; private set => gameDataManager = value; }

    private void OnEnable()
    {
#if UNITY_EDITOR
        // Source: https://forum.unity.com/threads/solved-but-unhappy-scriptableobject-awake-never-execute.488468/#post-5564170
        // use platform dependent compilation so it only exists in editor, otherwise it'll break the build
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            Init();
#endif       
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        PlayFabSettings.TitleId = "357DE";
        AuthManager = new();
        GameDataManager = new();
    }
}
