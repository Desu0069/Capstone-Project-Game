using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlimUI.ModernMenu
{
    public class MusicSliderIntegration : MonoBehaviour
    {
        [Header("References")]
        public Slider musicSlider; // Reference to the music slider
        public TMP_Text musicPercentageText; // Reference to the percentage text
        public Image muteIcon; // Reference to the mute icon
        public Sprite muteSprite; // Sprite for mute icon
        public Sprite unmuteSprite; // Sprite for unmute icon

        private bool isMuted = false;
        private string userKey = "DefaultUser"; // Should be set by your user management system

        // Store the last non-zero volume for restoring after unmuting
        private float lastVolumeBeforeMute = 1f;

        private void Start()
        {
            UpdateUserKey();
            LoadMusicSettingsForCurrentUser();
        }

        // Call this whenever the user changes!
        public void LoadMusicSettingsForCurrentUser()
        {
            UpdateUserKey();
            float volume = PlayerPrefs.GetFloat($"{userKey}_MusicVolume", 1f);

            // If muted, restore lastVolumeBeforeMute from PlayerPrefs
            isMuted = volume == 0f;
            if (isMuted)
            {
                lastVolumeBeforeMute = PlayerPrefs.GetFloat($"{userKey}_PreviousMusicVolume", 1f);
                musicSlider.value = 0f;
            }
            else
            {
                lastVolumeBeforeMute = volume;
                musicSlider.value = volume;
            }

            UpdateMusicPercentage(musicSlider.value);
            UpdateMuteIcon();
            musicSlider.interactable = !isMuted;
        }

        public void OnMusicSliderValueChanged()
        {
            float volume = musicSlider.value;

            // Only update lastVolumeBeforeMute if not muted and volume > 0
            if (!isMuted && volume > 0f)
            {
                lastVolumeBeforeMute = volume;
                PlayerPrefs.SetFloat($"{userKey}_PreviousMusicVolume", lastVolumeBeforeMute);
            }

            PlayerPrefs.SetFloat($"{userKey}_MusicVolume", volume);

            UpdateMusicPercentage(volume);

            // Update the mute state based on volume
            isMuted = volume == 0f;
            UpdateMuteIcon();
            musicSlider.interactable = !isMuted;

            NotifyMusicVolumeListeners();
        }

        public void OnMuteButtonClicked()
        {
            isMuted = !isMuted;

            if (isMuted)
            {
                // Save the current slider value before muting if it's > 0
                if (musicSlider.value > 0f)
                {
                    lastVolumeBeforeMute = musicSlider.value;
                    PlayerPrefs.SetFloat($"{userKey}_PreviousMusicVolume", lastVolumeBeforeMute);
                }

                musicSlider.value = 0f;
                PlayerPrefs.SetFloat($"{userKey}_MusicVolume", 0f);
                musicSlider.interactable = false;
            }
            else
            {
                // Restore to last volume before mute, or default to 1f
                float restoreVolume = PlayerPrefs.GetFloat($"{userKey}_PreviousMusicVolume", lastVolumeBeforeMute > 0f ? lastVolumeBeforeMute : 1f);
                musicSlider.value = restoreVolume;
                PlayerPrefs.SetFloat($"{userKey}_MusicVolume", restoreVolume);
                isMuted = false;
                musicSlider.interactable = true;
            }

            UpdateMusicPercentage(musicSlider.value);
            UpdateMuteIcon();

            NotifyMusicVolumeListeners();
        }

        private void UpdateMusicPercentage(float volume)
        {
            int percentage = Mathf.RoundToInt(volume * 100);
            musicPercentageText.text = $"{percentage}%";
        }

        private void UpdateMuteIcon()
        {
            muteIcon.sprite = isMuted ? muteSprite : unmuteSprite;
        }

        private void NotifyMusicVolumeListeners()
        {
            CheckMusicVolume[] listeners = Object.FindObjectsByType<CheckMusicVolume>(FindObjectsSortMode.None);
            foreach (var listener in listeners)
            {
                listener.UpdateVolume();
            }
        }

        // Call this method to update the user key, e.g., on login or profile switch.
        private void UpdateUserKey()
        {
            if (UserSlotsManager.Instance != null && UserSlotsManager.Instance.GetActiveUser() != null)
            {
                userKey = UserSlotsManager.Instance.GetActiveUser().username;
            }
            else
            {
                userKey = "DefaultUser";
            }
        }

        // Optionally, call this when switching users to save the previous volume:
        public void SavePreviousVolumeForCurrentUser()
        {
            PlayerPrefs.SetFloat($"{userKey}_PreviousMusicVolume", musicSlider.value);
        }
    }
}