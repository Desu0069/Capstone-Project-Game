using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health = 3;
    [SerializeField] GameObject hitVFX;
    [SerializeField] GameObject ragdoll;

    [Header("Combat")]
    [SerializeField] float attackCD = 3f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float aggroRange = 4f;

    public EnemyHealthBar healthBar; // Assign in inspector or in Start()
    [SerializeField] float maxHealth = 3;

    [Header("Collectible Manager Reference")]
    public CollectibleItem collectibleManager; // Drag & assign in Inspector or auto-find

    public TutorialPanelManager tutorialPanelManager;
    public int panelIndexToShow = 6;

    GameObject player;
    NavMeshAgent agent;
    Animator animator;
    float timePassed;
    float newDestinationCD = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (collectibleManager == null)
            collectibleManager = Object.FindFirstObjectByType<CollectibleItem>();

        // Auto-find healthBar if not assigned
        if (healthBar == null)
            healthBar = GetComponentInChildren<EnemyHealthBar>();

        // Set initial health UI
        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);
    }

    void Update()
    {
        animator.SetFloat("speed", agent.velocity.magnitude / agent.speed);

        if (player == null) return;

        if (timePassed >= attackCD)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                animator.SetTrigger("attack");
                timePassed = 0;
            }
        }
        timePassed += Time.deltaTime;

        if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
        {
            newDestinationCD = 0.5f;
            agent.SetDestination(player.transform.position);
        }
        newDestinationCD -= Time.deltaTime;
        transform.LookAt(player.transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
        }
    }

    void Die()
    {
        // Coin reward integration
        if (CoinRewardSystem.Instance != null)
            CoinRewardSystem.Instance.RewardEnemyDefeat(true);

        // Notify collectible manager BEFORE destroy!
        if (collectibleManager != null)
            collectibleManager.NotifyEnemyDefeated(this.gameObject);
        
        
        var tutorialPanelManager = FindFirstObjectByType<TutorialPanelManager>();
        if (tutorialPanelManager != null)
            tutorialPanelManager.ShowPanelOnEnemyDefeat(panelIndexToShow);
        else
            Debug.LogWarning("TutorialPanelManager not found in scene.");


        if (healthBar != null)
            healthBar.gameObject.SetActive(false);
        // Destroy enemy
        Destroy(this.gameObject);
        
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        animator.SetTrigger("damage");

        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);

        if (health <= 0)
            Die();
    }
    public void StartDealDamage()
    {
        GetComponentInChildren<EnemyDamageDealer>().StartDealDamage();
    }
    public void EndDealDamage()
    {
        GetComponentInChildren<EnemyDamageDealer>().EndDealDamage();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}