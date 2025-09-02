using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject quizPanel;
    public TMP_Text questionText;
    public Transform choicesContainer; // Parent for choice buttons
    public Button choiceButtonPrefab;  // Prefab for answer buttons (must have TMP_Text child)
    public TMP_Text progressText;
    public GameObject missionCompletePanel;
    public GameObject quizFailPanel;
    public Button retryQuizButton;
    public Button restartLevelButton;

    [Header("Quiz Settings")]
    public List<QuizQuestion> questions;
    public int requiredCorrectToPass = 4;

    [Header("Player Control References")]
    public MonoBehaviour cameraController;
    public MonoBehaviour characterController;
    public Animator characterAnimator; // Add this for character animation control

    private int currentQuestionIndex = 0;
    private int correctCount = 0;
    private bool quizActive = false;

    [System.Serializable]
    public class QuizQuestion
    {
        [TextArea]
        public string question;
        public List<string> choices;
        public int correctChoiceIndex; // 0-based
    }

    void Start()
    {
        // Ensure panels are hidden at start
        quizPanel.SetActive(false);
        missionCompletePanel.SetActive(false);
        if (quizFailPanel != null) quizFailPanel.SetActive(false);

        // Hook up retry/restart buttons
        if (retryQuizButton != null) retryQuizButton.onClick.AddListener(RetryQuiz);
        if (restartLevelButton != null) restartLevelButton.onClick.AddListener(RestartLevel);
    }

    // Call this when DialogueManager is finished
    public void StartQuiz()
    {
        SetPlayerControlsEnabled(false);
        LockCursor(true);

        quizPanel.SetActive(true);
        missionCompletePanel.SetActive(false);
        if (quizFailPanel != null) quizFailPanel.SetActive(false);

        currentQuestionIndex = 0;
        correctCount = 0;
        quizActive = true;
        ShowCurrentQuestion();
    }

    void ShowCurrentQuestion()
    {
        // Clean up previous buttons
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        if (currentQuestionIndex >= questions.Count)
        {
            FinishQuiz();
            return;
        }

        var q = questions[currentQuestionIndex];
        questionText.text = q.question;
        progressText.text = $"Question {currentQuestionIndex + 1}/{questions.Count}";

        // Generate buttons
        for (int i = 0; i < q.choices.Count; i++)
        {
            var btnObj = Instantiate(choiceButtonPrefab, choicesContainer);
            var btnText = btnObj.GetComponentInChildren<TMP_Text>();
            btnText.text = q.choices[i];

            int capturedIndex = i;
            btnObj.onClick.RemoveAllListeners();
            btnObj.onClick.AddListener(() => OnChoiceSelected(capturedIndex));
        }
    }

    void OnChoiceSelected(int index)
    {
        if (!quizActive) return;

        var q = questions[currentQuestionIndex];
        bool correct = index == q.correctChoiceIndex;
        if (correct) correctCount++;

        currentQuestionIndex++;
        ShowCurrentQuestion();
    }

    void FinishQuiz()
    {
        quizActive = false;
        quizPanel.SetActive(false);

        // Always unlock cursor and disable controls/animator when showing any result panel
        SetPlayerControlsEnabled(false);
        LockCursor(true);

        if (correctCount >= requiredCorrectToPass)
        {
            missionCompletePanel.SetActive(true);
        }
        else
        {
            if (quizFailPanel != null)
                quizFailPanel.SetActive(true);
            else
                Debug.LogWarning("Quiz failed, but no quizFailPanel assigned.");
        }
    }

    void RetryQuiz()
    {
        SetPlayerControlsEnabled(false);
        LockCursor(true);
        currentQuestionIndex = 0;
        correctCount = 0;
        quizActive = true;
        quizFailPanel.SetActive(false);
        quizPanel.SetActive(true);
        ShowCurrentQuestion();
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Utility to enable/disable controls and animation
    void SetPlayerControlsEnabled(bool enabled)
    {
        if (cameraController != null) cameraController.enabled = enabled;
        if (characterController != null) characterController.enabled = enabled;
        if (characterAnimator != null) characterAnimator.enabled = enabled;
    }

    // Utility to lock/unlock the cursor
    void LockCursor(bool unlock)
    {
        if (unlock)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}