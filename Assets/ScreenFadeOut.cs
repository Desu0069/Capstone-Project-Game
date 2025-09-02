using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeOut : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private Image fadeImage;
    private Canvas fadeCanvas;
    private Color fadeColor = Color.black;

    void Awake()
    {
        // Create Canvas
        fadeCanvas = new GameObject("FadeCanvas").AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999;

        // Create Image
        fadeImage = new GameObject("FadeImage").AddComponent<Image>();
        fadeImage.transform.SetParent(fadeCanvas.transform, false);
        fadeImage.rectTransform.anchorMin = Vector2.zero;
        fadeImage.rectTransform.anchorMax = Vector2.one;
        fadeImage.rectTransform.offsetMin = Vector2.zero;
        fadeImage.rectTransform.offsetMax = Vector2.zero;
        fadeImage.color = fadeColor;

        DontDestroyOnLoad(fadeCanvas.gameObject);
    }

    void Start()
    {
        StartCoroutine(FadeOutUnscaled());
    }

    private System.Collections.IEnumerator FadeOutUnscaled()
    {
        float timer = 0f;
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // unaffected by pause/timeScale changes
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;
        fadeCanvas.gameObject.SetActive(false); // Hide after fade
    }
}