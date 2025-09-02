using UnityEngine;

public class ShopCustomizeToggle : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject shopText;
    public GameObject customizeText;
    public GameObject buyButton;
    public GameObject saveButton;
    public GameObject priceText;

    [Header("Shop Mode Buttons")]
    public GameObject[] shopWeaponButtons;    // Buttons for weapons in Shop Mode
    public GameObject[] shopCharacterButtons; // Buttons for characters in Shop Mode

    [Header("Customize Mode Buttons")]
    public GameObject[] customizeWeaponButtons;    // Buttons for weapons in Customize Mode
    public GameObject[] customizeCharacterButtons; // Buttons for characters in Customize Mode

    private int defaultWeaponIndex = 0;
    private int defaultCharacterIndex = 0;

    private int equippedWeaponIndex = 0;
    private int equippedCharacterIndex = 0;

    public bool isCustomizeMode = false;

    private PurchaseManager purchaseManager;

    private void Start()
    {
        purchaseManager = GetComponent<PurchaseManager>();
        if (purchaseManager == null)
        {
            Debug.LogError("PurchaseManager reference is missing! Please attach a PurchaseManager component to the same GameObject.");
        }
        LoadEquippedSelection();
        ShowShop();
    }

    private void LoadEquippedSelection()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null)
        {
            equippedWeaponIndex = user.selectedWeapon;
            equippedCharacterIndex = user.selectedCharacter;
        }
        else
        {
            equippedWeaponIndex = defaultWeaponIndex;
            equippedCharacterIndex = defaultCharacterIndex;
        }
    }

    /// <summary>
    /// Call this after switching user slots
    /// </summary>
    public void RefreshAllUI()
    {
        LoadEquippedSelection();
        if (IsCustomizeMode())
        {
            ShowCustomize();
        }
        else
        {
            ShowShop();
        }
    }

    public void ShowShop()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null)
        {
            // Always load user's preview indices when entering Shop mode
            int weaponPreviewIndex = user.previewWeapon;
            int characterPreviewIndex = user.previewCharacter;
            // Optionally: enforce bounds, e.g. if new user
            if (weaponPreviewIndex < 0 || weaponPreviewIndex >= shopWeaponButtons.Length)
                weaponPreviewIndex = defaultWeaponIndex;
            if (characterPreviewIndex < 0 || characterPreviewIndex >= shopCharacterButtons.Length)
                characterPreviewIndex = defaultCharacterIndex;
            user.previewWeapon = weaponPreviewIndex;
            user.previewCharacter = characterPreviewIndex;
            UserSlotsManager.Instance.SaveSlot(UserSlotsManager.Instance.activeSlotIndex);
        }
        shopText.SetActive(true);
        buyButton.SetActive(true);
        customizeText.SetActive(false);
        saveButton.SetActive(false);
        EnablePriceText(true);
        RefreshShopUI();
    }

    public void RefreshShopUI()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        int weaponPreviewIndex = defaultWeaponIndex;
        int characterPreviewIndex = defaultCharacterIndex;
        if (user != null)
        {
            weaponPreviewIndex = user.previewWeapon;
            characterPreviewIndex = user.previewCharacter;
        }
        UpdateButtonsForMode(shopWeaponButtons, purchaseManager.IsWeaponPurchased, weaponPreviewIndex, true, true);
        UpdateButtonsForMode(shopCharacterButtons, purchaseManager.IsCharacterPurchased, characterPreviewIndex, true, false);
        Debug.Log("Shop UI refreshed.");
    }

    public void ShowCustomize()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null)
        {
            // Always load user's equipped indices when entering Customize mode
            int equippedWeaponIndex = user.selectedWeapon;
            int equippedCharacterIndex = user.selectedCharacter;
            // Optionally: enforce bounds
            if (equippedWeaponIndex < 0 || equippedWeaponIndex >= customizeWeaponButtons.Length
                || !purchaseManager.IsWeaponPurchased(equippedWeaponIndex))
                equippedWeaponIndex = defaultWeaponIndex;
            if (equippedCharacterIndex < 0 || equippedCharacterIndex >= customizeCharacterButtons.Length
                || !purchaseManager.IsCharacterPurchased(equippedCharacterIndex))
                equippedCharacterIndex = defaultCharacterIndex;
            this.equippedWeaponIndex = equippedWeaponIndex;
            this.equippedCharacterIndex = equippedCharacterIndex;
        }
        customizeText.SetActive(true);
        saveButton.SetActive(true);
        shopText.SetActive(false);
        buyButton.SetActive(false);
        EnablePriceText(false);
        RefreshCustomizeUI();
    }

    private void RefreshCustomizeUI()
    {
        // Refresh weapon buttons
        for (int i = 0; i < customizeWeaponButtons.Length; i++)
        {
            bool isPurchased = purchaseManager.IsWeaponPurchased(i);
            Transform ownedText = customizeWeaponButtons[i].transform.Find("OwnedText");

            if (ownedText != null)
            {
                ownedText.gameObject.SetActive(isPurchased || i == defaultWeaponIndex);
            }

            // Disable interaction if the item is not purchased
            var buttonComponent = customizeWeaponButtons[i].GetComponent<UnityEngine.UI.Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = isPurchased || i == defaultWeaponIndex;
            }

            // Highlight the equipped weapon
            if (equippedWeaponIndex == i)
            {
                // Add logic to visually highlight the equipped button
                Debug.Log("[Customize] Highlighting equipped weapon: " + i);
            }
        }

        // Refresh character buttons
        for (int i = 0; i < customizeCharacterButtons.Length; i++)
        {
            bool isPurchased = purchaseManager.IsCharacterPurchased(i);
            Transform ownedText = customizeCharacterButtons[i].transform.Find("OwnedText");

            if (ownedText != null)
            {
                ownedText.gameObject.SetActive(isPurchased || i == defaultCharacterIndex);
            }

            // Disable interaction if the item is not purchased
            var buttonComponent = customizeCharacterButtons[i].GetComponent<UnityEngine.UI.Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = isPurchased || i == defaultCharacterIndex;
            }

            // Highlight the equipped character
            if (equippedCharacterIndex == i)
            {
                // Add logic to visually highlight the equipped button
                Debug.Log("[Customize] Highlighting equipped character: " + i);
            }
        }
    }

    private void UpdateButtonsForMode(GameObject[] buttons, System.Func<int, bool> isPurchasedFunc, int highlightIndex, bool isShopMode, bool isWeapon)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogWarning($"Button at index {i} is null. Skipping...");
                continue;
            }

            bool isPurchased = isPurchasedFunc(i);
            Transform ownedText = buttons[i].transform.Find("OwnedText");

            if (ownedText != null)
            {
                // Show "Owned" text for purchased or default items
                ownedText.gameObject.SetActive(isPurchased || i == (isWeapon ? defaultWeaponIndex : defaultCharacterIndex));
            }
            else
            {
                Debug.LogWarning($"OwnedText child not found for button {buttons[i].name} at index {i}");
            }

            // Keep the button active
            buttons[i].SetActive(true);

            // Adjust button interactivity
            var buttonComponent = buttons[i].GetComponent<UnityEngine.UI.Button>();
            if (buttonComponent != null)
            {
                if (isShopMode)
                {
                    // In Shop Mode, disable buttons for owned items
                    buttonComponent.interactable = !isPurchased && i != (isWeapon ? defaultWeaponIndex : defaultCharacterIndex);
                }
                else
                {
                    // In Customize Mode, disable buttons for unpurchased items
                    buttonComponent.interactable = isPurchased || i == (isWeapon ? defaultWeaponIndex : defaultCharacterIndex);
                }
            }

            // Highlight the previewed item in Shop Mode or equipped in Customize Mode
            if (highlightIndex == i)
            {
                // Add your highlight logic here (e.g. change button color, outline, etc.)
                Debug.Log((isShopMode ? "[Shop] Previewing " : "[Customize] Highlighting ") + (isWeapon ? "weapon: " : "character: ") + i);
            }
        }
    }

    public void OnWeaponButtonClicked(int index)
    {
        OnItemButtonClicked(true, index);
    }

    public void OnCharacterButtonClicked(int index)
    {
        OnItemButtonClicked(false, index);
    }

    public void OnItemButtonClicked(bool isWeapon, int index)
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (IsCustomizeMode())
        {
            // Ensure only purchased or default items can be equipped in Customize Mode
            if (isWeapon)
            {
                if (purchaseManager.IsWeaponPurchased(index) || index == defaultWeaponIndex)
                {
                    equippedWeaponIndex = index;
                    Debug.Log("[Customize] Equipped weapon index: " + index);
                }
                else
                {
                    Debug.Log("[Customize] Cannot equip weapon " + index + " because it is not purchased.");
                }
            }
            else
            {
                if (purchaseManager.IsCharacterPurchased(index) || index == defaultCharacterIndex)
                {
                    equippedCharacterIndex = index;
                    Debug.Log("[Customize] Equipped character index: " + index);
                }
                else
                {
                    Debug.Log("[Customize] Cannot equip character " + index + " because it is not purchased.");
                }
            }
        }
        else
        {
            // In Shop Mode, allow preview per user
            if (user != null)
            {
                if (isWeapon)
                    user.previewWeapon = index;
                else
                    user.previewCharacter = index;

                // Enhancement: Save the preview change immediately per user
                UserSlotsManager.Instance.SaveSlot(UserSlotsManager.Instance.activeSlotIndex);
            }
            purchaseManager.SelectItem(isWeapon, index);
            Debug.Log("[Shop] Previewing item: " + (isWeapon ? "Weapon " : "Character ") + index);
            // Refresh UI so the highlight updates
            RefreshShopUI();
        }
    }

    public bool IsCustomizeMode()
    {
        return customizeText.activeSelf;
    }

    public void OnSaveCustomize()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null)
        {
            user.selectedWeapon = equippedWeaponIndex;
            user.selectedCharacter = equippedCharacterIndex;
            UserSlotsManager.Instance.SaveSlot(UserSlotsManager.Instance.activeSlotIndex);
            Debug.Log($"Saved equipped weapon {equippedWeaponIndex} and character {equippedCharacterIndex} for active slot {UserSlotsManager.Instance.activeSlotIndex}");
        }
    }

    private void EnablePriceText(bool enable)
    {
        if (priceText != null)
        {
            priceText.SetActive(enable);
        }
    }
}