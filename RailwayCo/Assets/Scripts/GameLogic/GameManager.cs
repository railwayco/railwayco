using UnityEngine;

[CreateAssetMenu(fileName = "RailwayCoSO", menuName = "RailwayCo/GameManager")]
public class GameManager : ScriptableObject
{
    public GameLogic GameLogic { get; private set; }

#if UNITY_EDITOR
    private void OnEnable()
    {
        // Source: https://forum.unity.com/threads/solved-but-unhappy-scriptableobject-awake-never-execute.488468/#post-5564170
        // use platform dependent compilation so it only exists in editor, otherwise it'll break the build
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            Init();
    }
#endif

    private void Awake() => Init();

    private void Init() => GameLogic = new();
}
