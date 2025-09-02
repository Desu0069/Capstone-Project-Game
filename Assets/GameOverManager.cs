using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over UI References")]
    [Tooltip("Assign the Game Over Panel or Canvas here")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Optional: Player Controller to disable on Game Over")]
    [SerializeField] private MonoBehaviour playerController; // Drag your controller here if you want to disable it

    private bool isGameOver = false;

    private void Awake()
    {
        // Ensure the Game Over panel starts hidden
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Show Game Over UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Disable player controller if assigned (optional)
        if (playerController != null)
            playerController.enabled = false;

        // Pause the game
        Time.timeScale = 0f;

        // Unlock cursor for menu interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideGameOver()
    {
        // Hide Game Over UI (if you want to allow retry/restart)
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Optionally re-enable player controller (if restarting)
        if (playerController != null)
            playerController.enabled = true;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isGameOver = false;
    }
}