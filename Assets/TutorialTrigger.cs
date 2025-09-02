using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialPanelManager tutorialManager;
    public int panelIndexToShow;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            tutorialManager.TriggerShowPanel(panelIndexToShow);
            gameObject.SetActive(false); // Optional: disables trigger after use
        }
    }
}