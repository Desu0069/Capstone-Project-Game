using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SlimUI.ModernMenu
{
    public class UIMenuManager : MonoBehaviour
    {
        private Animator CameraObject;

        [Header("MENUS")]
        public GameObject mainMenu;
        public GameObject firstMenu;
        public GameObject playMenu;
        public GameObject exitMenu;
        public GameObject extrasMenu;

        public enum Theme { custom1, custom2, custom3 };
        [Header("THEME SETTINGS")]
        public Theme theme;
        private int themeIndex;
        public ThemedUIData themeController;

        [Header("PANELS")]
        public GameObject mainCanvas;
        public GameObject PanelControls;
        public GameObject PanelVideo;
        public GameObject PanelGame;
        public GameObject PanelKeyBindings;
        public GameObject PanelMovement;
        public GameObject PanelCombat;
        public GameObject PanelGeneral;
        public GameObject shopPanel;        // The Shop panel
        public GameObject settingsPanel;
        public GameObject customPanel;// The Settings panel

        [Header("SETTINGS SCREEN")]
        public GameObject lineGame;
        public GameObject lineVideo;
        public GameObject lineControls;
        public GameObject lineKeyBindings;
        public GameObject lineMovement;
        public GameObject lineCombat;
        public GameObject lineGeneral;

        [Header("LOADING SCREEN")]
        public bool waitForInput = true;
        public GameObject loadingMenu;
        public Slider loadingBar;
        public TMP_Text loadPromptText;
        public KeyCode userPromptKey;

        [Header("SFX")]
        public AudioSource hoverSound;
        public AudioSource sliderSound;
        public AudioSource swooshSound;

        void Start()
        {
            CameraObject = transform.GetComponent<Animator>();

            playMenu.SetActive(false);
            exitMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            firstMenu.SetActive(true);
            mainMenu.SetActive(true);

            SetThemeColors();
        }

        void SetThemeColors()
        {
            switch (theme)
            {
                case Theme.custom1:
                    themeController.currentColor = themeController.custom1.graphic1;
                    themeController.textColor = themeController.custom1.text1;
                    themeIndex = 0;
                    break;
                case Theme.custom2:
                    themeController.currentColor = themeController.custom2.graphic2;
                    themeController.textColor = themeController.custom2.text2;
                    themeIndex = 1;
                    break;
                case Theme.custom3:
                    themeController.currentColor = themeController.custom3.graphic3;
                    themeController.textColor = themeController.custom3.text3;
                    themeIndex = 2;
                    break;
                default:
                    Debug.Log("Invalid theme selected.");
                    break;
            }
        }

        public void PlayCampaign()
        {
            exitMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            playMenu.SetActive(true);
        }

        public void PlayCampaignMobile()
        {
            exitMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            playMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void ReturnMenu()
        {
            playMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            exitMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

        public void LoadScene(string scene)
        {
            if (scene != "")
            {
                StartCoroutine(LoadAsynchronously(scene));
            }
        }

        public void DisablePlayCampaign()
        {
            playMenu.SetActive(false);
        }

        public void Position4()
        {


            // Set Animator float to 2 for Position 3
            CameraObject.SetFloat("Animate", 1);
        }

        // Updated Position3 Method
        public void Position3()
        {
            

            // Set Animator float to 2 for Position 3
            CameraObject.SetFloat("Animate", 2);
        }
        public void ActivatePanel(int panelIndex)
        {
            // Disable all panels initially
            shopPanel.SetActive(false);
            settingsPanel.SetActive(false);
            customPanel.SetActive(false); // Assuming the third panel is named "thirdPanel"

            // Activate the selected panel based on the panelIndex
            switch (panelIndex)
            {
                case 1:
                    shopPanel.SetActive(true);
                    break;
                case 2:
                    settingsPanel.SetActive(true);
                    break;
                case 3:
                    customPanel.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("Invalid panel index");
                    break;
            }

            // Move the camera to Position 2
            CameraObject.SetFloat("Animate", 1);
        }
        public void Position2(bool showSettings)
        {
            DisablePlayCampaign();
            // Disable both panels initially
            shopPanel.SetActive(false);
            settingsPanel.SetActive(false);

            // Determine which panel to enable
            if (showSettings)
            {
                settingsPanel.SetActive(true);
            }
            else
            {
                shopPanel.SetActive(true);
            }

            // Move camera to Position 2
            CameraObject.SetFloat("Animate", 1);
        }

        public void Position1()
        {
            CameraObject.SetFloat("Animate", 0);
        }

        void DisablePanels()
        {
            PanelControls.SetActive(false);
            PanelVideo.SetActive(false);
            PanelGame.SetActive(false);
            PanelKeyBindings.SetActive(false);

            lineGame.SetActive(false);
            lineControls.SetActive(false);
            lineVideo.SetActive(false);
            lineKeyBindings.SetActive(false);

            PanelMovement.SetActive(false);
            lineMovement.SetActive(false);
            PanelCombat.SetActive(false);
            lineCombat.SetActive(false);
            PanelGeneral.SetActive(false);
            lineGeneral.SetActive(false);
        }

        public void GamePanel()
        {
            DisablePanels();
            PanelGame.SetActive(true);
            lineGame.SetActive(true);
        }

        public void VideoPanel()
        {
            DisablePanels();
            PanelVideo.SetActive(true);
            lineVideo.SetActive(true);
        }

        public void ControlsPanel()
        {
            DisablePanels();
            PanelControls.SetActive(true);
            lineControls.SetActive(true);
        }

        public void KeyBindingsPanel()
        {
            DisablePanels();
            MovementPanel();
            PanelKeyBindings.SetActive(true);
            lineKeyBindings.SetActive(true);
        }

        public void MovementPanel()
        {
            DisablePanels();
            PanelKeyBindings.SetActive(true);
            PanelMovement.SetActive(true);
            lineMovement.SetActive(true);
        }

        public void CombatPanel()
        {
            DisablePanels();
            PanelKeyBindings.SetActive(true);
            PanelCombat.SetActive(true);
            lineCombat.SetActive(true);
        }

        public void GeneralPanel()
        {
            DisablePanels();
            PanelKeyBindings.SetActive(true);
            PanelGeneral.SetActive(true);
            lineGeneral.SetActive(true);
        }

        public void PlayHover()
        {
            hoverSound.Play();
        }

        public void PlaySFXHover()
        {
            sliderSound.Play();
        }

        public void PlaySwoosh()
        {
            swooshSound.Play();
        }

        public void AreYouSure()
        {
            exitMenu.SetActive(true);
            if (extrasMenu) extrasMenu.SetActive(false);
            DisablePlayCampaign();
        }

        public void AreYouSureMobile()
        {
            exitMenu.SetActive(true);
            if (extrasMenu) extrasMenu.SetActive(false);
            mainMenu.SetActive(false);
            DisablePlayCampaign();
        }

        public void ExtrasMenu()
        {
            playMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(true);
            exitMenu.SetActive(false);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        IEnumerator LoadAsynchronously(string sceneName)
        {
            mainCanvas.SetActive(false);
            loadingMenu.SetActive(true);

            loadingBar.value = 0f;
            loadPromptText.text = "0%";

            float minimumLoadTime = 3f;
            float smoothSpeed = 1f;
            float currentProgress = 0f;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            float startTime = Time.time;

            while (!operation.isDone)
            {
                float rawProgress = Mathf.Clamp01(operation.progress / 0.9f);
                float timeProgress = Mathf.Clamp01((Time.time - startTime) / minimumLoadTime);
                float targetProgress = Mathf.Min(rawProgress, timeProgress);

                currentProgress = Mathf.MoveTowards(currentProgress, targetProgress,
                                                     smoothSpeed * Time.deltaTime);

                float displayProgress = Mathf.Clamp01(currentProgress);
                loadingBar.value = displayProgress;
                loadPromptText.text = Mathf.RoundToInt(displayProgress * 100) + "%";

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
}