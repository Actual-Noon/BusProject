using System.Collections;
using UnityEngine;
using TMPro;

public class TextBubbleUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private CanvasGroup canvasGroup; // Re-added CanvasGroup for overall fading

    private Coroutine fadeCoroutine;
    private const float FadeSpeed = 5f;

    private void Awake()
    {
        // Force hide on launch
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Displays a message inside the bubble at a specific screen position.
    /// </summary>
    public void DisplayBubble(string message, Vector2 screenPosition, float duration = 3.0f)
    {
        // Set the text content
        textMeshPro.text = message;

        // Position the bubble on the UI Canvas
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = screenPosition;

        // Manage display coroutines
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeBubbleRoutine(duration));
    }

    private IEnumerator FadeBubbleRoutine(float duration)
    {
        // Fade In to fully visible (1.0f)
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * FadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Wait on screen
        yield return new WaitForSeconds(duration);

        // Fade Out
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * FadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}