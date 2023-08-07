using UnityEngine;
using UnityEngine.UI;

public class PlatformDetailButton : MonoBehaviour
{
    [SerializeField] private Button _platformButton;
    [SerializeField] private Image _platformIcon;
    [SerializeField] private Text _platformName;
    private PlatformController _platform;

    // Populate the platform button object with the relevant information
    // Modify the button behaviour and visual based on status of the platform
    public void SetPlatformGameObject(GameObject platform)
    {
        _platform = platform.GetComponent<PlatformController>();
        _platformIcon.sprite = platform.GetComponent<SpriteRenderer>().sprite;
        _platformName.text = platform.name;

        if (!_platform.IsPlatformUnlocked)
        {
            Color color = GetComponent<Image>().color;
            GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.392f); // Alpha: 100/255

            color = _platformIcon.color;
            _platformIcon.color = new Color(color.r, color.g, color.b, 0.392f); // Alpha: 100/255;

            color = _platformName.color;
            _platformName.color = new Color(color.r, color.g, color.b, 0.392f); // Alpha: 100/255

            GetComponent<Button>().enabled = false;
        }
    }

    private void Awake()
    {
        if (!_platformButton) Debug.LogError("Platform Detail Button not attached");
        _platformButton.onClick.AddListener(OnButtonClicked);

        if (!_platformIcon) Debug.LogError("Platform Icon not attached");
        if (!_platformName) Debug.LogError("Platform Name not attached");
    }

    private void OnButtonClicked()
    {
        _platform.LoadCargoPanelViaPlatform();
        _platform.FollowPlatform();
    }
}
