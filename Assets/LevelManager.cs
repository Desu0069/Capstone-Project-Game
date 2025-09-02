using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Player Setup")]
    public GameObject playerArmature; // The main player object containing all scripts
    public Transform charactersParent; // Parent object containing all character models
    public RuntimeAnimatorController playerAnimatorController; // Shared animator controller for all characters
    public Avatar[] characterAvatars; // Array of avatars for each character (match the order of characters)
    public GameObject[] weaponPrefabs; // Array of all weapon prefabs

    private EquipmentSystem _equipmentSystem;
    private Animator _animator;

    private void Start()
    {
        // Load saved selections (or use defaults for new users)
        int characterIndex = LoadCharacterIndex();
        int weaponIndex = LoadWeaponIndex();

        // Enable the selected character and set up the avatar
        EnableSelectedCharacter(characterIndex);

        // Setup the weapon
        SetupWeapon(weaponIndex);
    }

    private int LoadCharacterIndex()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        int characterIndex = (user != null) ? user.selectedCharacter : 0;
        Debug.Log($"Loaded character index: {characterIndex}");
        if (characterIndex < 0 || characterIndex >= charactersParent.childCount)
        {
            Debug.LogWarning($"Invalid or missing character index: {characterIndex}. Defaulting to 0.");
            characterIndex = 0; // Default to the first character
        }
        return characterIndex;
    }

    private int LoadWeaponIndex()
    {
        var user = UserSlotsManager.Instance.GetActiveUser();
        int weaponIndex = (user != null) ? user.selectedWeapon : 0;
        Debug.Log($"Loaded weapon index: {weaponIndex}");
        if (weaponIndex < 0 || weaponIndex >= weaponPrefabs.Length)
        {
            Debug.LogWarning($"Invalid or missing weapon index: {weaponIndex}. Defaulting to 0.");
            weaponIndex = 0; // Default to the first weapon
        }
        return weaponIndex;
    }

    private void EnableSelectedCharacter(int characterIndex)
    {
        for (int i = 0; i < charactersParent.childCount; i++)
        {
            Transform character = charactersParent.GetChild(i);
            bool isActive = (i == characterIndex);
            character.gameObject.SetActive(isActive);

            if (isActive)
            {
                Debug.Log($"Activated character: {character.name}");
                UpdateAnimator(characterIndex); // Set up the animator with the correct avatar
                UpdateEquipmentSystemReferences(character); // Update EquipmentSystem for the active character
            }
        }
    }

    private void UpdateAnimator(int characterIndex)
    {
        _animator = playerArmature.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator component not found on the PlayerArmature.");
            return;
        }

        if (characterIndex >= 0 && characterIndex < characterAvatars.Length)
        {
            _animator.runtimeAnimatorController = playerAnimatorController; // Assign shared controller
            _animator.avatar = characterAvatars[characterIndex]; // Assign the avatar for the selected character
            Debug.Log($"Animator updated with avatar: {_animator.avatar.name} for character index {characterIndex}");
        }
        else
        {
            Debug.LogError($"Invalid character index: {characterIndex}. Cannot assign avatar.");
        }
    }

    private void UpdateEquipmentSystemReferences(Transform character)
    {
        _equipmentSystem = playerArmature.GetComponent<EquipmentSystem>();
        if (_equipmentSystem == null)
        {
            Debug.LogError("EquipmentSystem is not found on the PlayerArmature.");
            return;
        }

        // Find the weapon-related transforms in the character's hierarchy
        Transform sheathHolder = FindChildRecursive(character, "SheathHolder");
        Transform weaponHolder = FindChildRecursive(character, "WeaponHolder");

        if (sheathHolder == null || weaponHolder == null)
        {
            Debug.LogError($"Character {character.name} is missing SheathHolder and/or WeaponHolder references! Ensure they are correctly named and present in the hierarchy.");
            return;
        }

        // Update the EquipmentSystem with the new references
        _equipmentSystem.weaponSheath = sheathHolder;
        _equipmentSystem.weaponHand = weaponHolder;

        Debug.Log($"Updated EquipmentSystem for character: {character.name}");
    }

    private void SetupWeapon(int weaponIndex)
    {
        if (_equipmentSystem == null)
        {
            Debug.LogError("EquipmentSystem is not set. Cannot setup weapon.");
            return;
        }

        if (weaponIndex < 0 || weaponIndex >= weaponPrefabs.Length)
        {
            Debug.LogWarning($"Invalid weapon index: {weaponIndex}. Defaulting to 0.");
            weaponIndex = 0; // Default to the first weapon
        }

        _equipmentSystem.weaponPrefab = weaponPrefabs[weaponIndex];
        _equipmentSystem.SpawnWeaponInSheath(); // Ensure weapon is instantiated
        Debug.Log($"Weapon setup completed for weapon index {weaponIndex}: {_equipmentSystem.weaponPrefab.name}");
    }

    // Recursive function to find a child object by name
    private Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform found = FindChildRecursive(child, childName);
            if (found != null)
                return found;
        }
        return null; // Return null if not found
    }
}