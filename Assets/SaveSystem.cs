using UnityEngine;

public static class SaveSystem
{
    // Save selected character and weapon indices
    public static void SaveSelections(int characterIndex, int weaponIndex)
    {
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        PlayerPrefs.SetInt("SelectedWeapon", weaponIndex);
        PlayerPrefs.Save();
    }

    // Load selected character index
    public static int LoadCharacterIndex()
    {
        return PlayerPrefs.GetInt("SelectedCharacter", 0); // Default to 0
    }

    // Load selected weapon index
    public static int LoadWeaponIndex()
    {
        return PlayerPrefs.GetInt("SelectedWeapon", 0); // Default to 0
    }
}