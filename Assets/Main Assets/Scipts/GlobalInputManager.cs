using DG.Tweening;
using UnityEngine;

public class GlobalInputManager : MonoBehaviour
{
    public PauseMenu pauseMenu; // Assign in Inspector
    public GameObject missionCompletePanel; // Reference to the Mission Complete panel

    void Update()
    {
        // Prevent Pause Menu toggle if the Mission Complete panel is active
        if (missionCompletePanel != null && missionCompletePanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.TogglePause(); // Calls the pause menu
        }
    }
}