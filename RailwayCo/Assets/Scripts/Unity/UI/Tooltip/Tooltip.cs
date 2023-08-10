using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI contentField;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private int characterWrapLimit;
    private RectTransform _rectTransform;

    private void Awake()
    {
        if (!headerField) Debug.LogError("Header text field not found");
        if (!contentField) Debug.LogError("Content text field not found");
        if (!layoutElement) Debug.LogError("Layout Element not found");

        _rectTransform = GetComponent<RectTransform>();
        if (!_rectTransform) Debug.LogError("Rect Transform not found");

        if (characterWrapLimit == default) characterWrapLimit = 500;
    }

    private void Update()
    {
        if (Application.isEditor)
            CheckTextLength();

        Vector3 mousePosition = Input.mousePosition;

        float pivotX = mousePosition.x / Screen.width;
        float pivotY = mousePosition.y / Screen.height;

        _rectTransform.pivot = new(pivotX, pivotY);
        transform.position = mousePosition;
    }

    public void Display(string contentText, string headerText = "")
    {
        if (string.IsNullOrEmpty(headerText))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = headerText;
        }

        contentField.gameObject.SetActive(true);
        contentField.text = contentText;

        CheckTextLength();
    }

    public void Reset()
    {
        headerField.text = "";
        contentField.text = "";
    }

    private void CheckTextLength()
    {
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = headerLength > characterWrapLimit || contentLength > characterWrapLimit;
    }
}
