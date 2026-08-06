using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Image backgroundImage;

    [Header("Appearance Settings")]
    [SerializeField] private float backgroundOpacity = 0.5f;

    private RectTransform rectTransform;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Hide(); // Hide by default on start
    }

    /// <summary>
    /// FLOATING UI: Displays a message and moves the UI to a specific screen position.
    /// </summary>
    public void Show(string message, Vector2 screenPosition, float duration = -1f)
    {
        rectTransform.anchoredPosition = screenPosition;
        ShowDialogue(message, duration);
    }

    /// <summary>
    /// STATIC UI: Displays a message but keeps the UI exactly where it is placed in the inspector layout.
    /// </summary>
    public void Show(string message, float duration = -1f)
    {
        ShowDialogue(message, duration);
    }

    private void ShowDialogue(string message, float duration)
    {
        textMeshPro.text = message;
        SetAlpha(backgroundOpacity, 1f);

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);

        if (duration > 0f)
        {
            hideCoroutine = StartCoroutine(HideAfterDelay(duration));
        }
    }

    public void Hide()
    {
        SetAlpha(0f, 0f);
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Hide();
    }

    private void SetAlpha(float bgAlpha, float textAlpha)
    {
        if (backgroundImage != null)
        {
            Color bgColor = backgroundImage.color;
            bgColor.a = bgAlpha;
            backgroundImage.color = bgColor;
        }

        if (textMeshPro != null)
        {
            Color textColor = textMeshPro.color;
            textColor.a = textAlpha;
            textMeshPro.color = textColor;
        }
    }
}