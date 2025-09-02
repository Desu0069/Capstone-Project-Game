using UnityEngine;
using UnityEngine.Events;

public class ConfirmationBuyPanelManager : MonoBehaviour
{
    [Header("Panel UI")]
    [SerializeField] private GameObject confirmationPanel;

    [Header("Buy Event")]
    [Tooltip("Assign your actual buy logic here. Will be called on Yes.")]
    public UnityEvent onConfirmBuy;

    private void Awake()
    {
        // Ensure panel is hidden at start
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    /// <summary>
    /// Call this when the user clicks Buy to show the confirmation panel.
    /// </summary>
    public void ShowConfirmation()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(true);

        // Optional: Pause input, unlock cursor, etc
    }

    /// <summary>
    /// Call this from the No button.
    /// </summary>
    public void OnNo()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        // Optional: Resume input, lock cursor, etc
    }

    /// <summary>
    /// Call this from the Yes button. Triggers buy logic, then closes.
    /// </summary>
    public void OnYes()
    {
        if (onConfirmBuy != null)
            onConfirmBuy.Invoke();

        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);

        // Optional: Resume input, lock cursor, etc
    }
}