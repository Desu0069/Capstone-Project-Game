using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInOnSceneLoad : MonoBehaviour
{
    public Image blackFadeImage;
    public float fadeDuration = 1.5f;
    public float disableCanvasDelay = 5f; // New delay before disabling canvas

    private Canvas parentCanvas; // Reference to parent canvas

    void Start()
    {
        // Get reference to parent canvas
        parentCanvas = GetComponentInParent<Canvas>();

        if (blackFadeImage != null)
        {
            PlayFade();
        }
    }

    public void PlayFade()
    {
        // Ensure canvas is enabled at start
        if (parentCanvas != null)
        {
            parentCanvas.enabled = true;
        }

        blackFadeImage.gameObject.SetActive(true);
        StopAllCoroutines(); // prevent overlapping fades
        StartCoroutine(FadeFromBlack());
    }

    IEnumerator FadeFromBlack()
    {
        float t = 0f;
        Color c = blackFadeImage.color;
        c.a = 1f;
        blackFadeImage.color = c;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            blackFadeImage.color = c;
            yield return null;
        }

        blackFadeImage.gameObject.SetActive(false);

        // Wait additional time before disabling canvas
        yield return new WaitForSeconds(disableCanvasDelay);

        if (parentCanvas != null)
        {
            parentCanvas.enabled = false;
        }
    }
}