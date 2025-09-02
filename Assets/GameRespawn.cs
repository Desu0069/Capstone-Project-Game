using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public float threshold = -10f;
    public Transform respawnPoint;
    public AudioClip respawnSound;
    public FadeInOnSceneLoad fadeController; // Reference to your fade script

    private AudioSource audioSource;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void FixedUpdate()
    {
        if (transform.position.y < threshold && respawnPoint != null)
        {
            RespawnPlayer();
        }
    }

    // Public method to respawn the player that can be called from other scripts
    public void RespawnPlayer()
    {
        if (respawnPoint != null)
        {
            // Move player safely
            if (characterController != null)
            {
                characterController.enabled = false;
                transform.position = respawnPoint.position;
                characterController.enabled = true;
            }
            else
            {
                transform.position = respawnPoint.position;
            }

            // Play respawn sound
            if (respawnSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(respawnSound);
            }

            // Trigger fade
            if (fadeController != null)
            {
                fadeController.PlayFade();
            }
        }
    }
}
