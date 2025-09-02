using UnityEngine;

public class YourTutorialOrLobbyManager : MonoBehaviour
{
    int equippedCharacter;
    int equippedWeapon;

    void Start()
    {
        // Always use the current slot index
        string slotPrefix = "UserSlot_" + UserSlotState.CurrentSlotIndex + "_";
        equippedCharacter = PlayerPrefs.GetInt(slotPrefix + "SelectedCharacter", 0);
        equippedWeapon = PlayerPrefs.GetInt(slotPrefix + "SelectedWeapon", 0);

        Debug.Log("Loaded equipped character: " + equippedCharacter + " and weapon: " + equippedWeapon + " for slot: " + UserSlotState.CurrentSlotIndex);

        // TODO: Use equippedCharacter and equippedWeapon to set up your player, visuals, etc.
    }
}