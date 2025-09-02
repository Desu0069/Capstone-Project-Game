using UnityEngine;

public class InstructionTrigger : MonoBehaviour
{
    public InstructionsManager instructionsManager;
    public int panelIndexToShow;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            instructionsManager.ShowPanel(panelIndexToShow);
            // Optional: deactivate this GameObject so it can't ever trigger again
            // gameObject.SetActive(false);
        }
    }
}