using UnityEngine;
using TMPro; // For TextMeshPro

public class CoinSystem : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Assign all coin UI TextMeshPro components here (e.g., HUD, shop, profile, etc.)")]
    public TMP_Text[] coinsTexts; // Array for multiple coin UI elements

    private void Start()
    {
        UpdateCoinsUI(); // Update all coin UIs at the start
    }

    // Get the current coin balance for the active user
    public static int GetCoins()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null) return user.coins;
        return 0;
    }

    // Add coins to the active user's balance
    public static void AddCoins(int amount)
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null)
        {
            user.coins += amount;
            UserSlotsManager.Instance.SaveSlot(UserSlotsManager.Instance.activeSlotIndex);
        }
    }

    // Deduct coins from the active user's balance
    public static bool DeductCoins(int amount)
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null && user.coins >= amount)
        {
            user.coins -= amount;
            UserSlotsManager.Instance.SaveSlot(UserSlotsManager.Instance.activeSlotIndex);
            return true;
        }
        return false;
    }

    // Update all coin UI elements
    public void UpdateCoinsUI()
    {
        int coins = GetCoins();
        if (coinsTexts != null)
        {
            foreach (var txt in coinsTexts)
            {
                if (txt != null)
                    txt.text = $"Coins: {coins}";
            }
        }
    }

    // DEBUG: Add coins for testing
    public void DebugAddCoins(int amount)
    {
        AddCoins(amount);
        UpdateCoinsUI(); // Refresh all coin UIs
    }

    // DEBUG: Deduct coins for testing
    public void DebugDeductCoins(int amount)
    {
        if (DeductCoins(amount))
        {
            UpdateCoinsUI(); // Refresh all coin UIs
        }
        else
        {
            Debug.LogWarning("Not enough coins to deduct!");
        }
    }
}