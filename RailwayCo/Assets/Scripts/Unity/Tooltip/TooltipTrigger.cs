using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach as a component to GameObject that requires this tooltip functionality
/// </summary>
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string contentText;
    public string headerText;

    public void OnPointerEnter(PointerEventData eventData) => TooltipManager.Show(contentText, headerText);

    public void OnPointerExit(PointerEventData eventData) => TooltipManager.Hide();
}
