using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("LOADING SCREEN")]
    public GameObject loadingMenu; // The loading screen panel
    public Slider loadingBar; // Loading bar slider
    public TMP_Text loadPromptText; // Loading progress text

    /// <summary>
    /// Starts loading the specified scene asynchronously with a loading screen.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(LoadAsynchronously(sceneName));
        }
    }

    /// <summary>
    /// Coroutine to handle asynchronous scene loading with a loading screen.
    /// </summary>
    private IEnumerator LoadAsynchronously(string sceneName)
    {
        if (loadingMenu == null || loadingBar == null || loadPromptText == null)
        {
            Debug.LogError("Loading screen references are not assigned in the Inspector!");
            yield break;
        }

        // Show loading screen
        loadingMenu.SetActive(true);

        // Reset loading visuals
        loadingBar.value = 0f;
        loadPromptText.text = "0%";

        float minimumLoadTime = 3f; // Minimum time to show the loading screen
        float smoothSpeed = 1f; // Speed of the progress bar animation
        float currentProgress = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Prevent automatic scene activation
        float startTime = Time.time;

        while (!operation.isDone)
        {
            // Calculate raw loading progress (ranges from 0.0 to 0.9)
            float rawProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Calculate time-based progress to ensure minimum load time
            float timeProgress = Mathf.Clamp01((Time.time - startTime) / minimumLoadTime);

            // Use the smaller of rawProgress and timeProgress to determine progress
            float targetProgress = Mathf.Min(rawProgress, timeProgress);

            // Smoothly animate the current progress
            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress,
                                                 smoothSpeed * Time.deltaTime);

            // Update loading UI
            float displayProgress = Mathf.Clamp01(currentProgress);
            loadingBar.value = displayProgress;
            loadPromptText.text = Mathf.RoundToInt(displayProgress * 100) + "%";

            // Once loading is complete, activate the scene
            if (displayProgress >= 0.99f && operation.progress >= 0.9f)
            {
                loadingBar.value = 1f;
                loadPromptText.text = "100%";
                yield return new WaitForSeconds(0.25f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}