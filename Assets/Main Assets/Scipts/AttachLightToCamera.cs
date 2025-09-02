using UnityEngine;

public class AttachLightToCamera : MonoBehaviour
{
    public Transform cameraTransform; // Assign your camera in the Inspector

    void Start()
    {
        if (cameraTransform != null)
        {
            transform.position = cameraTransform.position;
            transform.rotation = cameraTransform.rotation;
        }
        else
        {
            Debug.LogWarning("Camera Transform is not assigned!");
        }
    }
}
