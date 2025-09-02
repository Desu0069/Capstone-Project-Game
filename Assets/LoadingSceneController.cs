using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingSceneController : MonoBehaviour
{
    public Slider loadingBar;
    public TMP_Text loadPromptText;

    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        string sceneToLoad = LoadingHelper.NextSceneName;
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("No target scene set!");
            yield break;
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            loadingBar.value = op.progress / 0.9f;
            loadPromptText.text = Mathf.RoundToInt(loadingBar.value * 100) + "%";
            yield return null;
        }

        loadingBar.value = 1f;
        loadPromptText.text = "100%";
        yield return new WaitForSeconds(0.5f);
        op.allowSceneActivation = true;
    }
}