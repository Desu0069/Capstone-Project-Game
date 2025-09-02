using UnityEngine;

public class InstructionsManager : MonoBehaviour
{
    public GameObject[] instructionPanels;
    public Canvas instructionsCanvas;

    private int currentPanelIndex = -1;

    private void Start()
    {
        ShowPanel(0); // Show intro on start
    }

    public void ShowPanel(int index)
    {
        if (instructionsCanvas != null && !instructionsCanvas.gameObject.activeSelf)
            instructionsCanvas.gameObject.SetActive(true);

        for (int i = 0; i < instructionPanels.Length; i++)
            instructionPanels[i].SetActive(i == index);

        currentPanelIndex = index;

        PauseGame(true);
    }

    public void OnPanelClicked()
    {
        int nextIndex = currentPanelIndex + 1;
        if (nextIndex < instructionPanels.Length)
        {
            ShowPanel(nextIndex);
        }
        else
        {
            if (instructionsCanvas != null)
                instructionsCanvas.gameObject.SetActive(false);

            PauseGame(false);
        }
    }

    private void PauseGame(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
    }
}