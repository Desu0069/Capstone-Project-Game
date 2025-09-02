using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Start()
    {
        fadeImage.gameObject.SetActive(true); // ✅ Ensure FadePanel is active
        fadeImage.color = new Color(0, 0, 0, 1); // ✅ Start fully black

        StartCoroutine(FadeIn()); // ✅ Test if fade works
    }
    IEnumerator FadeIn()
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0); // ✅ Fully transparent at end
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log("Start Button Pressed"); // ✅ Log button press
        StartCoroutine(FadeOut(sceneName)); // ✅ Start fade-out
    }


    IEnumerator FadeOut(string sceneName)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
            yield return null;
        }

        Debug.Log("Fading complete. Waiting before loading scene."); // ✅ Debug log

        yield return new WaitForSeconds(1f); // ✅ Ensure a short delay before scene loads

        Debug.Log("Loading Scene: " + sceneName); // ✅ Confirm when scene switch happens
        SceneManager.LoadScene(sceneName);
    }
}
