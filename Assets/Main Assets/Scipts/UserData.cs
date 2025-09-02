using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public string username;
    public int coins = 0;
    public int selectedCharacter = 0;
    public int selectedWeapon = 0;
    public int previewWeapon = 0;
    public int previewCharacter = 0;
    public List<int> purchasedCharacters = new List<int> { 0 };
    public List<int> purchasedWeapons = new List<int> { 0 };
    public bool hasCompletedTutorial = false;
}