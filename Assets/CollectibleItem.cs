using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CollectibleItem : MonoBehaviour
{
    [Header("Enemy Tracking")]
    public List<GameObject> enemies = new List<GameObject>();
    public bool autoFindEnemies = true;

    [Header("Collectible Object")]
    public GameObject collectibleObject; // Assign the collectible object (must have CollectibleTrigger)
    public AudioClip collectSound;

    [Header("Mission Complete UI")]
    public GameObject missionCompletePanel;

    [Header("Loading Screen")]
    public GameObject loadingScreen;
    public Slider loadingProgressBar;
    public TMP_Text loadingPromptText;

    [Header("Player Control Scripts")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour cameraScript;

    private void Awake()
    {
        if (autoFindEnemies)
        {
            enemies.Clear();
            enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        }

        if (collectibleObject)
        {
            var trigger = collectibleObject.GetComponent<CollectibleTrigger>();
            if (trigger)
                trigger.DeactivateCollectible();
            else
                collectibleObject.SetActive(false);
        }
    }

    public void NotifyEnemyDefeated(GameObject enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);

        if (enemies.Count == 0)
            ShowCollectible();
    }

    public void ShowCollectible()
    {
        if (collectibleObject)
        {
            var trigger = collectibleObject.GetComponent<CollectibleTrigger>();
            if (trigger)
                trigger.ActivateCollectible();
            else
                collectibleObject.SetActive(true); // fallback
        }
    }

    public void OnCollectibleCollected()
    {
        if (collectSound)
            AudioSource.PlayClipAtPoint(collectSound, collectibleObject.transform.position);

        if (missionCompletePanel)
        {
            missionCompletePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Time.timeScale = 0f;

        if (playerMovementScript) playerMovementScript.enabled = false;
        if (cameraScript) cameraScript.enabled = false;

        // No need to deactivate here; CollectibleTrigger handles it
        // collectibleObject.SetActive(false);
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        UserSlotsManager.Instance.SetTutorialComplete(UserSlotsManager.Instance.activeSlotIndex);
        StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().buildIndex));
    }

    public void ContinueToMainMenu()
    {
        Time.timeScale = 1f;
        UserSlotsManager.Instance.SetTutorialComplete(UserSlotsManager.Instance.activeSlotIndex);
        StartCoroutine(LoadSceneAsync("LobbyMenu"));
    }

    private IEnumerator LoadSceneAsync(object scene)
    {
        if (loadingScreen) loadingScreen.SetActive(true);
        if (loadingProgressBar) loadingProgressBar.value = 0f;
        if (loadingPromptText) loadingPromptText.text = "0%";

        AsyncOperation op = (scene is int)
            ? SceneManager.LoadSceneAsync((int)scene)
            : SceneManager.LoadSceneAsync((string)scene);

        op.allowSceneActivation = false;
        Time.timeScale = 1f;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            if (loadingProgressBar) loadingProgressBar.value = progress;
            if (loadingPromptText) loadingPromptText.text = Mathf.RoundToInt(progress * 100f) + "%";

            if (progress >= 1f || (progress >= 0.9f && op.progress >= 0.9f))
            {
                yield return new WaitForSecondsRealtime(0.2f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}