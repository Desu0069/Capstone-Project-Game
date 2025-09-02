using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SafetyNetRespawn : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public MonoBehaviour playerController; // assign your movement controller (can be CharacterController, etc)
    public MonoBehaviour cameraController; // assign your camera controller
    public Transform respawnPoint;         // the point to which you want to teleport the player
    public Image fadeImage;                // full-screen UI image for fade effect

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public Color fadeColor = Color.black;

    private bool isRespawning = false;

    void Start()
    {
        if (fadeImage != null)
        {
            // Make sure fade is transparent at start
            SetFadeAlpha(0f);
            fadeImage.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isRespawning) return;
        // You may need to check for the player tag/layer instead of direct reference
        if (other.transform == playerTransform)
        {
            StartCoroutine(RespawnSequence());
        }
    }

    IEnumerator RespawnSequence()
    {
        isRespawning = true;

        // Lock controls
        if (playerController != null) playerController.enabled = false;
        if (cameraController != null) cameraController.enabled = false;

        // Fade in
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        }

        // Move player to respawn point
        playerTransform.position = respawnPoint.position;
        playerTransform.rotation = respawnPoint.rotation;

        // Optional: reset velocity/animation if needed
        var rb = playerTransform.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero; 

        // Wait a short moment at full fade
        yield return new WaitForSeconds(0.2f);

        // Fade out
        if (fadeImage != null)
        {
            yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
            fadeImage.gameObject.SetActive(false);
        }

        // Unlock controls
        if (playerController != null) playerController.enabled = true;
        if (cameraController != null) cameraController.enabled = true;

        isRespawning = false;
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;
        SetFadeAlpha(startAlpha);
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            SetFadeAlpha(alpha);
            yield return null;
        }
        SetFadeAlpha(endAlpha);
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeColor;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}