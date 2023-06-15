using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;

    private int levelToLoad;
    public UnityEvent<Scene> sceneChangeEvent = new();

    private void Start() => sceneChangeEvent.AddListener((scene) => FadeToScene(scene));

    public void FadeToScene(Scene scene)
    {
        levelToLoad = ((int)scene);
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete() => SceneManager.LoadScene(levelToLoad);
}
