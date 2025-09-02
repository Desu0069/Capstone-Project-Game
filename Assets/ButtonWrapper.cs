using UnityEngine;

public class ButtonWrapper : MonoBehaviour
{
    public PurchaseManager purchaseManager;        // Reference to PurchaseManager
    public ShopCustomizeToggle shopCustomizeToggle; // Reference to ShopCustomizeToggle
    public bool isWeapon;                          // Is this button for a weapon?
    public int index;                              // Index of the weapon or character

    public void OnButtonClick()
    {
        if (shopCustomizeToggle != null && shopCustomizeToggle.IsCustomizeMode())
        {
            // In customize mode, update equipped index
            shopCustomizeToggle.OnItemButtonClicked(isWeapon, index);
        }
        else if (purchaseManager != null)
        {
            // In shop mode, select for purchase
            purchaseManager.SelectItem(isWeapon, index);
        }
        else
        {
            Debug.LogError("PurchaseManager or ShopCustomizeToggle reference is missing in ButtonWrapper.");
        }
    }
}