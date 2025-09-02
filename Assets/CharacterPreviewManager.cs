using UnityEngine;

public class CharacterPreviewManager : MonoBehaviour
{
    [Header("Preview Setup")]
    public Camera previewCamera;          // Camera used to render the character
    public Transform previewParent;       // Parent object where the character will be instantiated
    private GameObject currentCharacter;  // Currently displayed character model

    /// <summary>
    /// Displays the selected character in the preview area.
    /// </summary>
    /// <param name="characterPrefab">The character prefab to display.</param>
    public void ShowCharacterPreview(GameObject characterPrefab)
    {
        // Destroy the current character model if it exists
        if (currentCharacter != null)
        {
            Destroy(currentCharacter);
        }

        // Instantiate the new character model in the preview area
        currentCharacter = Instantiate(characterPrefab, previewParent);
        currentCharacter.transform.localPosition = Vector3.zero;  // Reset position
        currentCharacter.transform.localRotation = Quaternion.identity;  // Reset rotation
    }
}