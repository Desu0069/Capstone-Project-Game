using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class UserSlotsManager : MonoBehaviour
{
    public static UserSlotsManager Instance;

    public int maxSlots = 3;
    public int activeSlotIndex = -1; // -1 = no slot selected

    private UserData[] userSlots;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        userSlots = new UserData[maxSlots];
        LoadAllSlots();
    }

    // Create new user and save to slot
    public void CreateUser(string username, int slot)
    {
        UserData newUser = new UserData
        {
            username = username,
            coins = 0,
            selectedCharacter = 0,
            selectedWeapon = 0,
            previewWeapon = 0,    // important for per-user preview!
            previewCharacter = 0,
            purchasedCharacters = new List<int> { 0 },
            purchasedWeapons = new List<int> { 0 },
            hasCompletedTutorial = false
        };
        userSlots[slot] = newUser;
        activeSlotIndex = slot;
        SaveSlot(slot);
    }

    // Delete user slot
    public void DeleteUser(int slot)
    {
        userSlots[slot] = null;
        if (activeSlotIndex == slot) activeSlotIndex = -1;
        DeleteSlotFile(slot);
    }

    // Get currently active user's data
    public UserData GetActiveUser()
    {
        if (activeSlotIndex >= 0 && activeSlotIndex < maxSlots)
            return userSlots[activeSlotIndex];
        return null;
    }

    // Save a single slot as JSON
    public void SaveSlot(int slot)
    {
        if (userSlots[slot] == null) return;
        string json = JsonUtility.ToJson(userSlots[slot], true);
        File.WriteAllText(GetSlotPath(slot), json);
    }

    // Load a single slot from JSON
    public void LoadSlot(int slot)
    {
        string path = GetSlotPath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            userSlots[slot] = JsonUtility.FromJson<UserData>(json);
        }
        else
        {
            userSlots[slot] = null;
        }
    }

    // Save all slots
    public void SaveAllSlots()
    {
        for (int i = 0; i < maxSlots; i++) SaveSlot(i);
    }

    // Load all slots
    public void LoadAllSlots()
    {
        for (int i = 0; i < maxSlots; i++) LoadSlot(i);
    }

    // Helper: get the file path for a slot
    private string GetSlotPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"userSlot_{slot}.json");
    }

    // Helper: delete a slot file
    private void DeleteSlotFile(int slot)
    {
        string path = GetSlotPath(slot);
        if (File.Exists(path)) File.Delete(path);
    }

    // Access slot data directly, for UI display etc.
    public UserData GetSlotData(int slot)
    {
        if (slot >= 0 && slot < maxSlots)
            return userSlots[slot];
        return null;
    }

    // Mark tutorial as complete for a slot and save immediately
    public void SetTutorialComplete(int slot)
    {
        if (slot >= 0 && slot < maxSlots && userSlots[slot] != null)
        {
            userSlots[slot].hasCompletedTutorial = true;
            SaveSlot(slot);
        }
    }

    // Utility: Check if a slot is valid and occupied
    public bool IsSlotOccupied(int slot)
    {
        return slot >= 0 && slot < maxSlots && userSlots[slot] != null;
    }

    // Utility: Reset all slots (for development/testing)
    public void ResetAllSlots()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            DeleteUser(i);
        }
        LoadAllSlots();
    }
}