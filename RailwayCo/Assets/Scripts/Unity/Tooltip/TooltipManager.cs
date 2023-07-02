using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    private static TooltipManager _instance;

    [SerializeField] private Tooltip tooltip;

    public void Awake()
    {
        _instance = this;
        Hide();
    }

    public static void Show(string contentText, string headerText = "")
    {
        _instance.tooltip.Display(contentText, headerText);
        _instance.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        _instance.tooltip.Reset();
        _instance.tooltip.gameObject.SetActive(false);
    }
}
