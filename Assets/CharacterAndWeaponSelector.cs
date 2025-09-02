using UnityEngine;
using UnityEngine.UI;

public class CharacterAndWeaponSelector : MonoBehaviour
{
    [Header("Panels")]
    public GameObject characterPanel;
    public GameObject weaponPanel;

    [Header("Characters")]
    public GameObject[] characters;
    private int currentCharacterIndex = 0;

    [Header("Weapons")]
    public GameObject[] weapons;
    private int currentWeaponIndex = 0;

    [Header("Tab Buttons")]
    public Button characterTabButton;
    public Button weaponTabButton;

    [Header("Character Navigation Buttons")]
    public Button characterNextButton;
    public Button characterPreviousButton;

    [Header("Weapon Navigation Buttons")]
    public Button weaponNextButton;
    public Button weaponPreviousButton;

    private enum SelectionMode { Character, Weapon }
    private SelectionMode currentMode = SelectionMode.Character;


    private void Start()
    {
        // Assign tab switching
        characterTabButton.onClick.AddListener(SwitchToCharacterSelection);
        weaponTabButton.onClick.AddListener(SwitchToWeaponSelection);

        // Assign character navigation
        characterNextButton.onClick.AddListener(NextCharacter);
        characterPreviousButton.onClick.AddListener(PreviousCharacter);

        // Assign weapon navigation
        weaponNextButton.onClick.AddListener(NextWeapon);
        weaponPreviousButton.onClick.AddListener(PreviousWeapon);

        // Always start with character tab
        SwitchToCharacterSelection();
    }

    private void SwitchToCharacterSelection()
    {
        currentMode = SelectionMode.Character;
        characterPanel.SetActive(true);
        weaponPanel.SetActive(false);

        UpdateCharacterDisplay();

        // Hide all weapons when in character mode
        foreach (var weapon in weapons)
        {
            weapon.SetActive(false);
        }
    }

    private void SwitchToWeaponSelection()
    {
        currentMode = SelectionMode.Weapon;
        characterPanel.SetActive(false);
        weaponPanel.SetActive(true);

        UpdateWeaponDisplay();

        // Hide all characters when in weapon mode
        foreach (var character in characters)
        {
            character.SetActive(false);
        }
    }

    private void NextCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Length;
        UpdateCharacterDisplay();
    }

    private void PreviousCharacter()
    {
        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
            currentCharacterIndex = characters.Length - 1;
        UpdateCharacterDisplay();
    }

    private void NextWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        UpdateWeaponDisplay();
    }

    private void PreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
            currentWeaponIndex = weapons.Length - 1;
        UpdateWeaponDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == currentCharacterIndex);
        }

        // Hide all weapons in case they were visible
        foreach (var weapon in weapons)
        {
            weapon.SetActive(false);
        }
    }

    private void UpdateWeaponDisplay()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == currentWeaponIndex);
        }

        // Hide all characters in case they were visible
        foreach (var character in characters)
        {
            character.SetActive(false);
        }
    }
}