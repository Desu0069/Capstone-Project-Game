using UnityEngine;
using TMPro;

public class PurchaseManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text priceText;         // Text to display the price
    public TMP_Text coinText;          // Text to display current coins

    [Header("Prices")]
    public int[] weaponPrices;         // Prices for each weapon
    public int[] characterPrices;      // Prices for each character

    private int selectedWeaponIndex = -1;  // Currently selected weapon index
    private int selectedCharacterIndex = -1; // Currently selected character index

    private ShopCustomizeToggle shopCustomizeToggle; // Reference to ShopCustomizeToggle

    private void Start()
    {
        shopCustomizeToggle = GetComponent<ShopCustomizeToggle>();
        UpdateCoinText();
    }

    public void SelectItem(bool isWeapon, int index)
    {
        if (isWeapon)
        {
            selectedWeaponIndex = index;
            selectedCharacterIndex = -1;
            ShowPrice(true, index);
        }
        else
        {
            selectedCharacterIndex = index;
            selectedWeaponIndex = -1;
            ShowPrice(false, index);
        }
    }

    public void ShowPrice(bool isWeapon, int index)
    {
        if (isWeapon)
        {
            if (index == 0 || IsWeaponPurchased(index))
            {
                priceText.text = "Price: Unlocked";
            }
            else
            {
                priceText.text = $"Price: {weaponPrices[index]} Coins";
            }
        }
        else
        {
            if (index == 0 || IsCharacterPurchased(index))
            {
                priceText.text = "Price: Unlocked";
            }
            else
            {
                priceText.text = $"Price: {characterPrices[index]} Coins";
            }
        }
    }

    public void BuySelectedItem()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user == null)
        {
            Debug.LogError("No active user slot!");
            return;
        }

        if (selectedWeaponIndex != -1)
        {
            int price = weaponPrices[selectedWeaponIndex];
            if (!IsWeaponPurchased(selectedWeaponIndex) && user.coins >= price)
            {
                user.coins -= price;
                if (!user.purchasedWeapons.Contains(selectedWeaponIndex))
                    user.purchasedWeapons.Add(selectedWeaponIndex);

                UserSlotsManager.Instance.SaveSlot(UserSlotsManager.Instance.activeSlotIndex);
                UpdateCoinText();
                shopCustomizeToggle.ShowShop();

                Debug.Log($"Weapon {selectedWeaponIndex} purchased!");
            }
            else
            {
                Debug.Log("Not enough coins or weapon already purchased.");
            }
        }
        else if (selectedCharacterIndex != -1)
        {
            int price = characterPrices[selectedCharacterIndex];
            if (!IsCharacterPurchased(selectedCharacterIndex) && user.coins >= price)
            {
                user.coins -= price;
                if (!user.purchasedCharacters.Contains(selectedCharacterIndex))
                    user.purchasedCharacters.Add(selectedCharacterIndex);

                UserSlotsManager.Instance.SaveSlot(UserSlotsManager.Instance.activeSlotIndex);
                UpdateCoinText();
                shopCustomizeToggle.ShowShop();

                Debug.Log($"Character {selectedCharacterIndex} purchased!");
            }
            else
            {
                Debug.Log("Not enough coins or character already purchased.");
            }
        }
        else
        {
            Debug.Log("No item selected to buy.");
        }
    }

    public bool IsWeaponPurchased(int index)
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user == null) return index == 0; // default always unlocked
        return user.purchasedWeapons.Contains(index) || index == 0;
    }

    public bool IsCharacterPurchased(int index)
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user == null) return index == 0;
        return user.purchasedCharacters.Contains(index) || index == 0;
    }

    private void UpdateCoinText()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (coinText != null && user != null)
        {
            coinText.text = $"Coins: {user.coins}";
        }
        else if (coinText != null)
        {
            coinText.text = $"Coins: 0";
        }
    }
}