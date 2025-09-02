using UnityEngine;
using TMPro; // For TextMeshPro components

public class ButtonPriceManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text priceText; // Text to display the price of the selected item

    [Header("Prices")]
    public int[] weaponPrices;            // Prices for each weapon
    public int[] characterPrices;         // Prices for each character

    public int selectedWeaponIndex = -1;    // Index of the selected weapon (-1 means none selected)
    public int selectedCharacterIndex = -1; // Index of the selected character (-1 means none selected)

    // Updates the button text with Locked/Unlocked indication
    public void UpdateButtonState(GameObject[] buttons, int defaultIndex, bool isWeapon)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
            bool isPurchased = IsItemPurchased(isWeapon, i);

            // Remove previous status
            string[] parts = buttonText.text.Split('\n');
            buttonText.text = parts[0];

            // Update button text with Locked/Unlocked indication
            if (i == defaultIndex || isPurchased) // Default or purchased
            {
                buttonText.text += "\nUnlocked";
            }
            else // Not purchased
            {
                buttonText.text += "\nLocked";
            }
        }
    }

    // Handles button selection
    public void OnButtonSelected(bool isWeapon, int index)
    {
        if (isWeapon)
        {
            selectedWeaponIndex = index; // Track the selected weapon index
            Debug.Log($"Selected Weapon Index: {selectedWeaponIndex}");
            ShowPrice(isWeapon, index); // Show price of the selected weapon
        }
        else
        {
            selectedCharacterIndex = index; // Track the selected character index
            Debug.Log($"Selected Character Index: {selectedCharacterIndex}");
            ShowPrice(isWeapon, index); // Show price of the selected character
        }
    }

    // Displays the price of the selected button
    private void ShowPrice(bool isWeapon, int index)
    {
        if (isWeapon)
        {
            // Show price for the selected weapon
            if (index == 0 || IsItemPurchased(true, index)) // Default or purchased
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
            // Show price for the selected character
            if (index == 0 || IsItemPurchased(false, index)) // Default or purchased
            {
                priceText.text = "Price: Unlocked";
            }
            else
            {
                priceText.text = $"Price: {characterPrices[index]} Coins";
            }
        }
    }

    // Checks if an item is purchased (uses JSON user data, not PlayerPrefs)
    private bool IsItemPurchased(bool isWeapon, int index)
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user == null)
            return index == 0; // default always unlocked

        if (isWeapon)
            return user.purchasedWeapons.Contains(index) || index == 0;
        else
            return user.purchasedCharacters.Contains(index) || index == 0;
    }

    // Get the selected weapon index
    public int GetSelectedWeaponIndex()
    {
        return selectedWeaponIndex;
    }

    // Get the selected character index
    public int GetSelectedCharacterIndex()
    {
        return selectedCharacterIndex;
    }
}