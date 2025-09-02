using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class UserSlotManager : MonoBehaviour
{
    [Header("User Slot Buttons")]
    public Button[] userSlotButtons;
    public Button[] deleteButtons;

    [Header("Panels")]
    public GameObject userSlotsPanel;
    public GameObject userInputPanel;
    public GameObject confirmationPanel;

    [Header("Confirmation Panel Buttons")]
    public Button confirmDeleteButton;
    public Button cancelDeleteButton;

    [Header("User Input")]
    public TMP_InputField userNameInputField;
    public Button saveButton;
    public Button cancelButton;

    [Header("Loading Screen")]
    public GameObject loadingMenu;
    public Slider loadingBar;
    public TMP_Text loadPromptText;

    [Header("Game Scene")]
    public string gameSceneName = "Tutorial";
    public string LobbyScene = "LobbyMenu";

    private int selectedSlotIndex = -1;
    private int slotToDeleteIndex = -1;

    void Start()
    {
        // Set up listeners
        for (int i = 0; i < userSlotButtons.Length; i++)
        {
            int index = i;
            userSlotButtons[i].onClick.AddListener(() => OnUserSlotButtonClicked(index));
        }
        for (int i = 0; i < deleteButtons.Length; i++)
        {
            int index = i;
            deleteButtons[i].onClick.AddListener(() => OnDeleteButtonClicked(index));
        }
        saveButton.onClick.AddListener(OnSaveButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
        confirmDeleteButton.onClick.AddListener(ConfirmDeleteUserSlot);
        cancelDeleteButton.onClick.AddListener(CancelDeleteUserSlot);

        // Load names from JSON
        RefreshSlotUI();

        userInputPanel.SetActive(false);
        confirmationPanel.SetActive(false);
        loadingMenu.SetActive(false);
        SetDeleteButtonsInteractable(true);
    }

    void RefreshSlotUI()
    {
        for (int i = 0; i < userSlotButtons.Length; i++)
        {
            var data = UserSlotsManager.Instance.GetSlotData(i);
            string display = data != null ? data.username : "User Slot";
            userSlotButtons[i].GetComponentInChildren<TMP_Text>().text = display;
        }
    }

    public void OnUserSlotButtonClicked(int slotIndex)
    {
        var data = UserSlotsManager.Instance.GetSlotData(slotIndex);
        if (data != null)
        {
            UserSlotsManager.Instance.activeSlotIndex = slotIndex;

            // Ensure ShopCustomizeToggle UI refreshes for the newly active user
            var shopToggle = Object.FindFirstObjectByType<ShopCustomizeToggle>();
            if (shopToggle != null)
            {
                shopToggle.RefreshAllUI();
            }

            // NEW LOGIC
            if (!data.hasCompletedTutorial)
            {
                // First time: go to tutorial
                LoadingHelper.NextSceneName = gameSceneName; // After tutorial, go to LobbyMenu
                SceneManager.LoadScene("LoadingScene");   // Scene loader should load Tutorial
                                                          // In your tutorial completion logic, be sure to call:
                                                          // UserSlotsManager.Instance.SetTutorialComplete(slotIndex);
            }
            else
            {
                // Already completed tutorial: go to Lobby
                LoadingHelper.NextSceneName = LobbyScene;
                SceneManager.LoadScene("LoadingScene");
            }
        }
        else
        {
            // Slot is empty: prompt for username
            selectedSlotIndex = slotIndex;
            userSlotsPanel.SetActive(false);
            userInputPanel.SetActive(true);
            userNameInputField.text = "";
            SetDeleteButtonsInteractable(false);
        }
    }

    public void OnSaveButtonClicked()
    {
        string userName = userNameInputField.text.Trim();
        if (string.IsNullOrEmpty(userName) || userName == "User Slot")
        {
            StartCoroutine(ShowInvalidNamePlaceholder());
            return;
        }

        UserSlotsManager.Instance.CreateUser(userName, selectedSlotIndex); // JSON-based creation

        userInputPanel.SetActive(false);
        userSlotsPanel.SetActive(true);
        SetDeleteButtonsInteractable(true);
        RefreshSlotUI();

        // Do NOT go to tutorial immediately anymore.
        // Previously: StartCoroutine(LoadAsynchronously(gameSceneName));
        // Now: just return to user slot selection UI.
        selectedSlotIndex = -1;
    }

    private void OnCancelButtonClicked()
    {
        userInputPanel.SetActive(false);
        userSlotsPanel.SetActive(true);
        selectedSlotIndex = -1;
        userNameInputField.text = "";
        SetDeleteButtonsInteractable(true);
    }

    private void OnDeleteButtonClicked(int slotIndex)
    {
        var data = UserSlotsManager.Instance.GetSlotData(slotIndex);
        // Only allow deletion if the slot is NOT empty
        if (data != null)
        {
            slotToDeleteIndex = slotIndex;
            userSlotsPanel.SetActive(false);
            confirmationPanel.SetActive(true);
        }
        // Otherwise, ignore the delete (you can add feedback here if desired)
    }

    private void ConfirmDeleteUserSlot()
    {
        if (slotToDeleteIndex >= 0 && slotToDeleteIndex < userSlotButtons.Length)
        {
            UserSlotsManager.Instance.DeleteUser(slotToDeleteIndex);
            RefreshSlotUI();
        }
        slotToDeleteIndex = -1;
        confirmationPanel.SetActive(false);
        userSlotsPanel.SetActive(true);
        SetDeleteButtonsInteractable(true);
    }

    private void CancelDeleteUserSlot()
    {
        slotToDeleteIndex = -1;
        confirmationPanel.SetActive(false);
        userSlotsPanel.SetActive(true);
        SetDeleteButtonsInteractable(true);
    }

    private IEnumerator ShowInvalidNamePlaceholder()
    {
        string originalPlaceholder = userNameInputField.placeholder.GetComponent<TMP_Text>().text;
        userNameInputField.placeholder.GetComponent<TMP_Text>().text = "Invalid Name";
        yield return new WaitForSeconds(1.5f);
        userNameInputField.placeholder.GetComponent<TMP_Text>().text = originalPlaceholder;
    }

    // You can keep LoadAsynchronously if needed elsewhere

    private IEnumerator LoadAsynchronously(string sceneName)
    {
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
            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, smoothSpeed * Time.deltaTime);
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

    // Utility function to enable/disable all delete buttons
    private void SetDeleteButtonsInteractable(bool interactable)
    {
        foreach (var btn in deleteButtons)
        {
            btn.interactable = interactable;
        }
    }
}