using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using OnScreenPointerPlugin;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button continueButton;
    public GameObject objectivePanel;
    public TMP_Text objectiveText;

    public QuizManager quizManager;

    [Header("Objectives (define each step in order)")]
    public List<ObjectiveStep> objectives = new List<ObjectiveStep>();

    [Header("Player Control References")]
    public MonoBehaviour cameraController;
    public MonoBehaviour characterController;

    [Header("Audio")]
    public AudioSource audioSource; // <-- Add this for SFX playback

    private int currentObjectiveIndex = -1;
    private ObjectiveStep currentStep;
    private bool[] markerCleared;
    private bool[] markerDialoguePlayed;
    private bool waitingForMarkerDialogue = false;

    private System.Action<OnScreenPointerObject>[] markerHandlers;

    private int currentIntroIndex = 0;
    private System.Action afterIntroSequence;

    // ==========================================
    // ObjectiveStep definition (Unity inspector)
    // ==========================================
    [System.Serializable]
    public class ObjectiveStep
    {
        public string objectiveText;
        public OnScreenPointerObject[] markers;
        public List<MarkerDialogueSet> markerDialogues;
        public GameObject[] barriers;
        public string[] introDialogues;
        public AudioClip[] introSFX; // <-- Add this for intro SFX, same length/order as introDialogues
        public GameObject[] objectiveObjects;
    }

    [System.Serializable]
    public class MarkerDialogueSet
    {
        public List<DialogueLine> dialogues;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string text;
        public AudioClip sfx; // <-- Add this for per-line SFX
    }

    void Start()
    {
        foreach (var step in objectives)
        {
            if (step.markers != null)
                foreach (var marker in step.markers)
                    if (marker != null) marker.DisableMarker();

            DisableObjectiveObjects(step);
        }

        dialoguePanel.SetActive(false);
        objectivePanel.SetActive(false);

        NextObjective();
    }

    void NextObjective()
    {
        currentObjectiveIndex++;
        // CHECK FIRST!
        if (currentObjectiveIndex >= objectives.Count)
        {
            objectivePanel.SetActive(true);
            objectiveText.text = "Objective Complete!";
            if (quizManager != null)
                quizManager.StartQuiz();
            else
                Debug.LogWarning("QuizManager not assigned in DialogueManager!");
            return;
        }

        currentStep = objectives[currentObjectiveIndex];
        markerCleared = new bool[currentStep.markers.Length];
        markerDialoguePlayed = new bool[currentStep.markers.Length];
        markerHandlers = new System.Action<OnScreenPointerObject>[currentStep.markers.Length];
        waitingForMarkerDialogue = false;

        foreach (var step in objectives)
            if (step.markers != null)
                foreach (var marker in step.markers)
                    if (marker != null) marker.DisableMarker();

        EnableObjectiveObjects(currentStep);

        if (currentStep.barriers != null)
        {
            foreach (var barrier in currentStep.barriers)
                if (barrier != null)
                {
                    var col = barrier.GetComponent<Collider>();
                    if (col != null) col.enabled = true;
                    barrier.SetActive(true);
                }
        }

        ShowIntroDialogues(currentStep.introDialogues, currentStep.introSFX, ShowObjectiveStep);
    }

    // ----------- MODIFIED: intro SFX support -----------
    void ShowIntroDialogues(string[] intros, AudioClip[] sfx, System.Action onComplete)
    {
        if (intros == null || intros.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }
        currentIntroIndex = 0;
        afterIntroSequence = onComplete;
        ShowSingleIntro(intros, sfx);
    }

    void ShowSingleIntro(string[] intros, AudioClip[] sfx)
    {
        var idx = currentIntroIndex;
        // Play SFX if present
        if (sfx != null && idx < sfx.Length && sfx[idx] != null)
            PlaySFX(sfx[idx]);

        ShowDialogue(intros[idx], () =>
        {
            currentIntroIndex++;
            if (currentIntroIndex < intros.Length)
                ShowSingleIntro(intros, sfx);
            else
                afterIntroSequence?.Invoke();
        });
    }
    // ---------------------------------------------------

    void ShowObjectiveStep()
    {
        Debug.Log($"ShowObjectiveStep [{GetInstanceID()}] on {gameObject.name}");
        ShowObjectiveText(currentStep.objectiveText);

        for (int i = 0; i < currentStep.markers.Length; i++)
        {
            var marker = currentStep.markers[i];
            if (marker != null)
            {
                if (markerHandlers[i] != null)
                    marker.OnMarkerDisabled -= markerHandlers[i];

                int index = i;
                markerHandlers[i] = (obj) => OnMarkerCleared(index, marker);
                marker.OnMarkerDisabled += markerHandlers[i];
                marker.EnableMarker();
            }
        }
    }

    void ShowDialogue(string text, System.Action onContinue)
    {
        Debug.Log($"ShowDialogue called with: {text}");
        SetPlayerControlsEnabled(false);
        LockCursor(true);

        // PAUSE GAME TIME
        Time.timeScale = 0f;

        dialoguePanel.SetActive(true);
        dialogueText.text = text;
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => {
            dialoguePanel.SetActive(false);

            // UNPAUSE GAME TIME
            Time.timeScale = 1f;

            SetPlayerControlsEnabled(true);
            LockCursor(false);
            onContinue?.Invoke();
        });
    }

    void ShowObjectiveText(string text)
    {
        objectivePanel.SetActive(true);
        objectiveText.text = text;
    }

    void HideObjectiveText()
    {
        objectivePanel.SetActive(false);
    }

    void OnMarkerCleared(int markerIndex, OnScreenPointerObject marker)
    {
        var markerDialogueSet = currentStep.markerDialogues != null
            && markerIndex < currentStep.markerDialogues.Count
            ? currentStep.markerDialogues[markerIndex]
            : null;

        if (waitingForMarkerDialogue) return;
        if (markerCleared == null || markerIndex < 0 || markerIndex >= markerCleared.Length)
        {
            Debug.LogWarning($"Invalid markerCleared array or markerIndex!");
            return;
        }
        if (markerCleared[markerIndex]) return;
        if (markerHandlers != null && markerHandlers[markerIndex] != null)
            marker.OnMarkerDisabled -= markerHandlers[markerIndex];
        markerCleared[markerIndex] = true;

        if (currentStep.markerDialogues == null ||
            markerIndex >= currentStep.markerDialogues.Count ||
            markerDialogueSet == null ||
            markerDialogueSet.dialogues == null ||
            markerDialogueSet.dialogues.Count == 0)
        {
            Debug.LogWarning($"No marker dialogue for marker {markerIndex} in objective {currentObjectiveIndex}");
            if (AllMarkersCleared())
                OnObjectiveStepComplete();
            else
                ShowObjectiveText(currentStep.objectiveText);
            return;
        }

        if (!markerDialoguePlayed[markerIndex])
        {
            markerDialoguePlayed[markerIndex] = true;
            waitingForMarkerDialogue = true;
            HideObjectiveText();
            StartCoroutine(ShowMarkerDialoguesCoroutine(markerDialogueSet.dialogues, () =>
            {
                waitingForMarkerDialogue = false;
                if (!AllMarkersCleared())
                    ShowObjectiveText(currentStep.objectiveText);
                else
                    OnObjectiveStepComplete();
            }));
        }
        else
        {
            if (AllMarkersCleared())
                OnObjectiveStepComplete();
            else
                ShowObjectiveText(currentStep.objectiveText);
        }
    }

    // ----------- MODIFIED: marker dialogue SFX support -----------
    System.Collections.IEnumerator ShowMarkerDialoguesCoroutine(List<DialogueLine> lines, System.Action onComplete)
    {
        foreach (var line in lines)
        {
            bool advanced = false;
            // Play SFX if present
            if (line.sfx != null)
                PlaySFX(line.sfx);

            ShowDialogue(line.text, () => { advanced = true; });
            yield return new WaitUntil(() => advanced);
        }
        onComplete?.Invoke();
    }
    // ------------------------------------------------------------

    void OnObjectiveStepComplete()
    {
        if (currentStep.barriers != null)
        {
            foreach (var barrier in currentStep.barriers)
                if (barrier != null)
                {
                    var col = barrier.GetComponent<Collider>();
                    if (col != null) col.enabled = false;
                    barrier.SetActive(false);
                }
        }
        DisableObjectiveObjects(currentStep);

        NextObjective();
    }

    bool AllMarkersCleared()
    {
        foreach (var cleared in markerCleared)
            if (!cleared) return false;
        return true;
    }

    void SetPlayerControlsEnabled(bool enabled)
    {
        if (cameraController != null) cameraController.enabled = enabled;
        if (characterController != null) characterController.enabled = enabled;
    }

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

    void EnableObjectiveObjects(ObjectiveStep step)
    {
        if (step.objectiveObjects == null) return;
        foreach (var obj in step.objectiveObjects)
            if (obj != null) obj.SetActive(true);
    }

    void DisableObjectiveObjects(ObjectiveStep step)
    {
        if (step.objectiveObjects == null) return;
        foreach (var obj in step.objectiveObjects)
            if (obj != null) obj.SetActive(false);
    }

    // ============= SFX PLAYBACK METHOD =============
    void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}