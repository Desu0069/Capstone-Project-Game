using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Weapon Selection")]
    public GameObject[] weaponPrefabs;
    private GameObject equippedWeapon;

    [Header("Character Selection")]
    public GameObject[] characters;
    public Transform[] weaponHolders;
    private int activeCharacterIndex = 0;
    private int equippedWeaponIndex = 0;

    private void Start()
    {
        // Load equipped selections for active user at start
        EnterCustomizeMode();
    }

    // ---------- Shop Mode ----------
    public void EnterShopMode()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null)
        {
            // Load per-user preview indices
            int weaponPreviewIndex = user.previewWeapon;
            int characterPreviewIndex = user.previewCharacter;

            if (weaponPreviewIndex < 0 || weaponPreviewIndex >= weaponPrefabs.Length)
                weaponPreviewIndex = 0;
            if (characterPreviewIndex < 0 || characterPreviewIndex >= characters.Length)
                characterPreviewIndex = 0;

            SwitchCharacter(characterPreviewIndex);
            EquipWeapon(weaponPreviewIndex);
        }
    }

    public void PreviewWeapon(int weaponIndex)
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null)
        {
            user.previewWeapon = weaponIndex;
            UserSlotsManager.Instance.SaveSlot(UserSlotsManager.Instance.activeSlotIndex);
        }
        EquipWeapon(weaponIndex);
    }

    public void PreviewCharacter(int characterIndex)
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null)
        {
            user.previewCharacter = characterIndex;
            UserSlotsManager.Instance.SaveSlot(UserSlotsManager.Instance.activeSlotIndex);
        }
        SwitchCharacter(characterIndex);
    }

    // ---------- Customize Mode ----------
    public void EnterCustomizeMode()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null)
        {
            int equippedWeaponIdx = user.selectedWeapon;
            int equippedCharacterIdx = user.selectedCharacter;

            if (equippedWeaponIdx < 0 || equippedWeaponIdx >= weaponPrefabs.Length)
                equippedWeaponIdx = 0;
            if (equippedCharacterIdx < 0 || equippedCharacterIdx >= characters.Length)
                equippedCharacterIdx = 0;

            equippedWeaponIndex = equippedWeaponIdx;
            activeCharacterIndex = equippedCharacterIdx;
            SwitchCharacter(activeCharacterIndex);
            EquipWeapon(equippedWeaponIndex);
        }
    }

    public void SaveCustomizeSelection()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        if (user != null)
        {
            user.selectedWeapon = equippedWeaponIndex;
            user.selectedCharacter = activeCharacterIndex;
            UserSlotsManager.Instance.SaveSlot(UserSlotsManager.Instance.activeSlotIndex);
        }
        Debug.Log($"Saved Customize Selection: Character {activeCharacterIndex}, Weapon {equippedWeaponIndex}");
    }

    // ---------- Existing Logic ----------
    public void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= weaponPrefabs.Length) return;

        if (equippedWeapon != null)
        {
            Destroy(equippedWeapon);
        }

        Transform currentWeaponHolder = weaponHolders[activeCharacterIndex];
        equippedWeapon = Instantiate(weaponPrefabs[weaponIndex], currentWeaponHolder);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.identity;
        equippedWeaponIndex = weaponIndex;
    }

    public void SwitchCharacter(int characterIndex)
    {
        // Disable all characters to ensure only one is active
        for (int i = 0; i < characters.Length; i++)
            characters[i].SetActive(false);

        if (characterIndex < 0 || characterIndex >= characters.Length) return;

        activeCharacterIndex = characterIndex;
        characters[activeCharacterIndex].SetActive(true);

        if (equippedWeapon != null)
        {
            Transform newWeaponHolder = weaponHolders[activeCharacterIndex];
            equippedWeapon.transform.SetParent(newWeaponHolder);
            equippedWeapon.transform.localPosition = Vector3.zero;
            equippedWeapon.transform.localRotation = Quaternion.identity;
        }
    }

    private int GetSelectedWeaponIndex()
    {
        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            if (equippedWeapon != null && equippedWeapon.name.Contains(weaponPrefabs[i].name))
            {
                return i;
            }
        }
        return 0;
    }
}