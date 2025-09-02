using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    public bool isPaused = false;

    [Header("Loading Screen")]
    public GameObject loadingScreen; // The loading screen panel
    public Slider loadingProgressBar; // Loading bar slider
    public TMP_Text loadPromptText; // Loading progress text (Optional if you want to show percentage)
    public float loadingDelay = 1f; // Minimum time to show loading screen
    public MonoBehaviour cameraScript; // Assign this in the Inspector

    [Header("Volume Control")]
    public Slider volumeSlider; // Slider to control volume
    public TMP_Text volumePercentageText; // Text to display volume percentage
    public AudioSource gameAudioSource; // Reference to the game's audio source

    [Header("Mute Toggle")]
    public Button muteToggleButton; // Button component for mute toggle
    public Image muteToggleImage; // Image for the mute toggle
    public Sprite muteSprite; // Sprite for mute state
    public Sprite unmuteSprite; // Sprite for unmute state
    private bool isMuted = false; // Tracks the mute state
    public bool blockPause = false;

    [Header("SFX")]
    [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
    public AudioSource hoverSound;
    [Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
    public AudioSource sliderSound;
    [Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching screens")]
    public AudioSource swooshSound;

    void Start()
    {
        // Hide loading screen at the start
        loadingScreen.SetActive(false);

        
       

        // Initialize volume slider and percentage text
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f); // Default volume is 100%
        gameAudioSource.volume = savedVolume;
        volumeSlider.value = savedVolume;
        UpdateVolumePercentage(savedVolume);

        // Initialize mute toggle
        isMuted = savedVolume == 0f;
        UpdateMuteToggleUI();
    }

    public void TogglePause()
    {
        if (blockPause) return; // Prevent pausing when blocked (e.g. during tutorial panel)

        if (isPaused) Resume();
        else Pause();
    }

    void Pause()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cameraScript.enabled = false;
    }

    void Resume()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraScript.enabled = true;
    }

    public void LoadMenu()
    {
        UserSlotsManager.Instance.SetTutorialComplete(UserSlotsManager.Instance.activeSlotIndex);
        PlaySwoosh();
        Time.timeScale = 1f;
        StartCoroutine(LoadAsynchronously("LobbyMenu"));
    }

    private IEnumerator LoadAsynchronously(string sceneName)
    {
        // Show loading screen and disable the pause menu
        pauseMenuUI.SetActive(false);
        loadingScreen.SetActive(true);

        // Reset visuals
        loadingProgressBar.value = 0f;
        loadPromptText.text = "0%";

        // Configuration
        float minimumLoadTime = loadingDelay; // Show loading for minimum delay
        float smoothSpeed = 1f; // Animation speed
        float currentProgress = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        float startTime = Time.unscaledTime;

        while (!operation.isDone)
        {
            // Calculate raw loading progress (0-0.9 → 0-1)
            float rawProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Calculate time-based progress (ensures minimum display time)
            float timeProgress = Mathf.Clamp01((Time.unscaledTime - startTime) / minimumLoadTime);

            // Determine target progress (use whichever is further behind)
            float targetProgress = Mathf.Min(rawProgress, timeProgress);

            // Smoothly update the progress bar
            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress,
                                                 smoothSpeed * Time.unscaledDeltaTime);

            // Update UI
            float displayProgress = Mathf.Clamp01(currentProgress);
            loadingProgressBar.value = displayProgress;
            loadPromptText.text = Mathf.RoundToInt(displayProgress * 100) + "%";

            // Completion check
            if (displayProgress >= 0.99f && operation.progress >= 0.9f)
            {
                // Finalize at 100%
                loadingProgressBar.value = 1f;
                loadPromptText.text = "100%";

                // Brief pause at completion
                yield return new WaitForSecondsRealtime(0.25f);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // SFX Methods
    public void PlayHover()
    {
        if (hoverSound != null)
            hoverSound.Play();
    }

    public void PlaySFXHover()
    {
        if (sliderSound != null)
            sliderSound.Play();
    }

    public void PlaySwoosh()
    {
        if (swooshSound != null)
            swooshSound.Play();
    }

    // Volume Control Methods
    public void OnVolumeSliderChanged()
    {
        float newVolume = volumeSlider.value;
        gameAudioSource.volume = newVolume;

        // Save the volume to PlayerPrefs
        PlayerPrefs.SetFloat("GameVolume", newVolume);

        // Update percentage text
        UpdateVolumePercentage(newVolume);

        // Update mute state based on volume
        isMuted = newVolume == 0f;
        UpdateMuteToggleUI();

        // Play slider interaction sound
        PlaySFXHover();
    }

    private void UpdateVolumePercentage(float volume)
    {
        int percentage = Mathf.RoundToInt(volume * 100);
        volumePercentageText.text = $"{percentage}%";
    }

    // Mute Toggle Methods
    public void OnMuteToggleClicked()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            gameAudioSource.volume = 0f;
            volumeSlider.value = 0f;
        }
        else
        {
            float previousVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
            gameAudioSource.volume = previousVolume;
            volumeSlider.value = previousVolume;
        }

        // Save the updated volume to PlayerPrefs
        PlayerPrefs.SetFloat("GameVolume", gameAudioSource.volume);

        // Update mute toggle UI
        UpdateMuteToggleUI();
    }

    private void UpdateMuteToggleUI()
    {
        if (isMuted)
        {
            muteToggleImage.sprite = muteSprite;
        }
        else
        {
            muteToggleImage.sprite = unmuteSprite;
        }
    }
}