using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float health = 100;

    [Header("Fall Damage Settings")]
    [Tooltip("Minimum fall height to start taking damage")]
    [SerializeField] private float fallDamageThreshold = 4f;
    [Tooltip("Damage multiplier per unit over threshold")]
    [SerializeField] private float fallDamageMultiplier = 10f;

    private float highestPointDuringFall;
    private bool isFalling = false;

    [Header("Game Over")]
    [SerializeField] private GameOverManager gameOverManager; // Assign in Inspector


    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthPercentText;

    private Animator animator;
    private CharacterController characterController;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetLayerWeight(3, 1f);

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("HealthSystem requires a CharacterController on the player!");
        }

        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }

    void Update()
    {
        HandleFallDetection();
    }

    private void HandleFallDetection()
    {
        if (characterController == null) return;

        if (!isFalling && !characterController.isGrounded)
        {
            // Start of fall
            isFalling = true;
            highestPointDuringFall = transform.position.y;
        }
        else if (isFalling && characterController.isGrounded)
        {
            // End of fall
            float landedY = transform.position.y;
            float fallDistance = highestPointDuringFall - landedY;

            if (fallDistance > fallDamageThreshold)
            {
                float damage = (fallDistance - fallDamageThreshold) * fallDamageMultiplier;
                TakeDamage(damage);
            }

            isFalling = false;
            highestPointDuringFall = transform.position.y;
        }
        // If standing on ground, update highest point for next fall (optional, for edge cases)
        if (characterController.isGrounded && !isFalling)
        {
            highestPointDuringFall = transform.position.y;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        animator.SetTrigger("damage");

        UpdateHealthUI();

        if (health <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }

        if (healthPercentText != null)
        {
            float percent = (maxHealth > 0) ? (health / maxHealth) * 100f : 0f;
            healthPercentText.text = $"{percent:0}%";
        }
    }

    private void Die()
    {
        // Show game over panel instead of reloading the scene
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("No GameOverManager assigned! Reloading scene as fallback.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}