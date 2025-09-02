using UnityEngine;
using UnityEngine.UI;
using SlimUI.ModernMenu;

public class PanelManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject shopPanelOne; // First shop panel
    public GameObject shopPanelTwo; // Second shop panel
    public GameObject customizePanelOne; // First customize panel
    public GameObject customizePanelTwo; // Second customize panel

    [Header("Buttons")]
    public ThemedUIElement shopButtonOne; // Themed UI Button for the first shop panel
    public ThemedUIElement shopButtonTwo; // Themed UI Button for the second shop panel
    public ThemedUIElement customizeButtonOne; // Themed UI Button for the first customize panel
    public ThemedUIElement customizeButtonTwo; // Themed UI Button for the second customize panel

    private ThemedUIElement currentActiveButton; // Tracks the currently active button

    private void Start()
    {
        // Add listeners to the buttons
        shopButtonOne.GetComponent<Button>().onClick.AddListener(() => ToggleButton(shopButtonOne, ShowShopPanelOne));
        shopButtonTwo.GetComponent<Button>().onClick.AddListener(() => ToggleButton(shopButtonTwo, ShowShopPanelTwo));
        customizeButtonOne.GetComponent<Button>().onClick.AddListener(() => ToggleButton(customizeButtonOne, ShowCustomizePanelOne));
        customizeButtonTwo.GetComponent<Button>().onClick.AddListener(() => ToggleButton(customizeButtonTwo, ShowCustomizePanelTwo));

        // Set the default panel (e.g., disable all panels initially)
        HideAllPanels();
        ResetButtonThemes();
    }

    private void HideAllPanels()
    {
        // Hide all panels
        shopPanelOne.SetActive(false);
        shopPanelTwo.SetActive(false);
        customizePanelOne.SetActive(false);
        customizePanelTwo.SetActive(false);
    }

    private void ResetButtonThemes()
    {
        // Reset all buttons to their normal theme
        ResetButtonTheme(shopButtonOne);
        ResetButtonTheme(shopButtonTwo);
        ResetButtonTheme(customizeButtonOne);
        ResetButtonTheme(customizeButtonTwo);
    }

    private void ResetButtonTheme(ThemedUIElement button)
    {
        // Reset the button's appearance to its default state
        if (button != null)
        {
            if (button.hasImage)
            {
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = button.themeController.currentColor; // Default button color
                }
            }

            // Clear the outline for inactive buttons
            button.outline = Color.clear;
        }
    }

    private void ToggleButton(ThemedUIElement button, System.Action panelAction)
    {
        if (currentActiveButton == button)
        {
            // If the same button is clicked again, do nothing (already active)
            return;
        }

        // Reset all button themes
        ResetButtonThemes();

        // Set the clicked button as the active one
        currentActiveButton = button;

        // Update the appearance of the active button
        if (button.hasImage)
        {
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = button.themeController.custom1.graphic1; // Highlight color (custom1 preset)
            }
        }

        button.outline = Color.white; // Example: Set a white outline for the active button

        // Execute the associated panel action
        panelAction();
    }

    private void ShowShopPanelOne()
    {
        HideAllPanels(); // Hide all panels first
        shopPanelOne.SetActive(true); // Show the first shop panel
    }

    private void ShowShopPanelTwo()
    {
        HideAllPanels(); // Hide all panels first
        shopPanelTwo.SetActive(true); // Show the second shop panel
    }

    private void ShowCustomizePanelOne()
    {
        HideAllPanels(); // Hide all panels first
        customizePanelOne.SetActive(true); // Show the first customize panel
    }

    private void ShowCustomizePanelTwo()
    {
        HideAllPanels(); // Hide all panels first
        customizePanelTwo.SetActive(true); // Show the second customize panel
    }
}