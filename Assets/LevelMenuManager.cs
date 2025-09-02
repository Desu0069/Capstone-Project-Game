using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject chapterPanel;         // Main chapter selection panel
    public GameObject[] levelPanels;        // Level panels (one per chapter)

    [Header("Loading Screen")]
    public GameObject loadingScreen;        // Loading screen panel
    public Slider loadingProgressBar;       // Slider for progress bar
    public TMP_Text loadingPromptText;      // Optional "percent loaded" text

    // Show the levels panel for the selected chapter
    public void OpenLevelPanel(int chapterIndex)
    {
        chapterPanel.SetActive(false);
        for (int i = 0; i < levelPanels.Length; i++)
            levelPanels[i].SetActive(i == chapterIndex);
    }

    // Go back to the chapter selection panel
    public void BackToChapters()
    {
        chapterPanel.SetActive(true);
        foreach (var panel in levelPanels)
            panel.SetActive(false);
    }

    // Loads a scene by name, with loading screen
    public void LoadLevelScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreen) loadingScreen.SetActive(true);
        if (loadingProgressBar) loadingProgressBar.value = 0f;
        if (loadingPromptText) loadingPromptText.text = "0%";

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingProgressBar) loadingProgressBar.value = progress;
            if (loadingPromptText) loadingPromptText.text = Mathf.RoundToInt(progress * 100f) + "%";

            if (progress >= 1f || (progress >= 0.9f && operation.progress >= 0.9f))
            {
                if (loadingProgressBar) loadingProgressBar.value = 1f;
                if (loadingPromptText) loadingPromptText.text = "100%";
                yield return new WaitForSecondsRealtime(0.2f);
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
    public void SetTextActive(GameObject textObj, bool active)
    {
        if (textObj != null)
            textObj.SetActive(active);
    }
}

// Attach this to any button that should show/hide hover text
public class HoverTextToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverText; // Assign the hidden text GameObject

    void Start()
    {
        if (hoverText) hoverText.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverText) hoverText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverText) hoverText.SetActive(false);
    }
}