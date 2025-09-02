using StarterAssets;
using UnityEngine;

public class TutorialPanelManager : MonoBehaviour
{
    [Header("Panel Setup")]
    public GameObject[] panels;           // Assign instruction panels in order in Inspector
    public Canvas tutorialCanvas;         // Assign your UI Canvas
    public MonoBehaviour cameraScript;    // Assign your camera script
    public PauseMenu pauseMenu; // Assign in Inspector

    public ThirdPersonController playerController;   // Assign in Inspector
    public EquipmentSystem equipmentSystem;          // Assign in Inspector
    public CombatSystem combatSystem;

    private int currentPanelIndex = -1;

    // (Optional) Track shown panels to prevent repeats
    private bool[] panelShown;

    private void Start()
    {
        panelShown = new bool[panels.Length];
        ShowPanel(0); // Show the intro panel at game start
    }

    private void EnableAbilitiesForPanel(int panelIndex)
    {
        // Movement
        if (playerController != null)
        {
            playerController.EnableSprint(panelIndex >= 2);
            playerController.EnableJump(panelIndex >= 3);
        }

        // Weapon mechanics — always set singleton
        MechanicStateManager.Instance.CanEquipWeapon = panelIndex >= 4;
        MechanicStateManager.Instance.CanUnequipWeapon = panelIndex >= 4;
        MechanicStateManager.Instance.CanAttack = panelIndex >= 5;

        // Still call equipmentSystem methods for consistency, if assigned
        if (equipmentSystem != null)
        {
            equipmentSystem.EnableEquipWeapon(panelIndex >= 4);
            equipmentSystem.EnableUnequipWeapon(panelIndex >= 4);
        }

        if (combatSystem != null)
        {
            combatSystem.EnableAttack(panelIndex >= 5);
        }
    }

    // Show a panel by index and pause the game
    public void ShowPanel(int index)
    {
        if (tutorialCanvas != null && !tutorialCanvas.gameObject.activeSelf)
            tutorialCanvas.gameObject.SetActive(true);

        for (int i = 0; i < panels.Length; i++)
            panels[i].SetActive(i == index);

        currentPanelIndex = index;
        panelShown[index] = true;

        EnableAbilitiesForPanel(index);

        PauseForTutorial(true);

        
    }

    public void EnableAllMechanics()
    {
        if (MechanicStateManager.Instance != null)
        {
            MechanicStateManager.Instance.CanRun = true;
            MechanicStateManager.Instance.CanJump = true;
            MechanicStateManager.Instance.CanEquipWeapon = true;
            MechanicStateManager.Instance.CanUnequipWeapon = true;
            MechanicStateManager.Instance.CanAttack = true;
        }
    }

    // Hide the current panel and resume the game
    public void HideCurrentPanel()
    {
        if (currentPanelIndex >= 0 && currentPanelIndex < panels.Length)
            panels[currentPanelIndex].SetActive(false);

        // Hide canvas if this is not the intro (for later triggers)
        if (tutorialCanvas != null)
            tutorialCanvas.gameObject.SetActive(false);

        PauseForTutorial(false);
    }

    // Call this from triggers to show a specific panel (and pause game)
    public void TriggerShowPanel(int index)
    {
        ShowPanel(index);
    }

    /// <summary>
    /// Call this from your enemy death logic and pass the desired tutorial panel index.
    /// Example: TutorialPanelManager.Instance.ShowPanelOnEnemyDefeat(2);
    /// </summary>
    /// <param name="panelIndex">Index of the panel to show (set in inspector)</param>
    public void ShowPanelOnEnemyDefeat(int panelIndex)
    {

        // Only show if not already shown
        if (panelIndex >= 0 && panelIndex < panels.Length && !panelShown[panelIndex])
        {
            ShowPanel(panelIndex);

            // If this is the last panel (6), enable all mechanics globally
            if (panelIndex == 6)
            {
                EnableAllMechanics();
            }
        }

    }

    private void PauseForTutorial(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pause;
        if (cameraScript != null)
            cameraScript.enabled = !pause;

        if (pauseMenu != null)
            pauseMenu.blockPause = pause;
    }
}