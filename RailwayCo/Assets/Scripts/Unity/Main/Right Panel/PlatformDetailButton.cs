using UnityEngine;
using UnityEngine.UI;

public class PlatformDetailButton : MonoBehaviour
{
    [SerializeField] private Button _platformButton;
    private GameObject _platformToFollow;

    // Populate the platform button object with the relevant information
    // Modify the button behaviour and visual based on status of the platform
    public void SetPlatformGameObject(GameObject platform)
    {
        _platformToFollow = platform;

        Image iconRectangle = this.transform.Find("IconRectangle").GetComponent<Image>();
        iconRectangle.sprite = platform.GetComponent<SpriteRenderer>().sprite;

        Text platformName = this.transform.Find("PlatformName").GetComponent<Text>();
        platformName.text = platform.name;

        if (!platform.GetComponent<PlatformController>().IsPlatformUnlocked)
        {
            Color color = this.GetComponent<Image>().color;
            this.GetComponent<Image>().color = new Color (color.r, color.g, color.b, 0.392f); // Alpha: 100/255

            color = iconRectangle.color;
            iconRectangle.color = new Color(color.r, color.g, color.b, 0.392f); // Alpha: 100/255;

            color = platformName.color;
            platformName.color = new Color(color.r, color.g, color.b, 0.392f); // Alpha: 100/255

            this.GetComponent<Button>().enabled = false;
        }
    }

    private void Awake()
    {
        if (!_platformButton) Debug.LogError("Platform Detail Button not attached");
        _platformButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        _platformToFollow.GetComponent<PlatformController>().LoadCargoPanelViaPlatform();
        _platformToFollow.GetComponent<PlatformController>().FollowPlatform();
    }
}
