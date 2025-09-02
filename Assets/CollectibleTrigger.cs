using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleTrigger : MonoBehaviour
{
    [Tooltip("Reference to the CollectibleItem manager script.")]
    public CollectibleItem manager;

    private bool isActive = false;

    public void ActivateCollectible()
    {
        isActive = true;
        gameObject.SetActive(true);
    }
    public void DeactivateCollectible()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (other.CompareTag("Player") && manager != null)
        {
            isActive = false;
            manager.OnCollectibleCollected();
            gameObject.SetActive(false);

            // --- Coin reward integration for tutorial ---
            if (CoinRewardSystem.Instance != null)
            {
                CoinRewardSystem.Instance.RewardTutorial();
            }
        }
    }
}