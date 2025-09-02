using UnityEngine;

public class PlayerCombatHandler : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Combat State")]
    public bool inCombat = false;
    private bool canAttack = true;
    private int attackIndex = 0;

    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;

    public void ToggleWeapon()
    {
        if (inCombat)
        {
            animator.ResetTrigger("drawWeapon");
            animator.SetTrigger("sheathWeapon");
            inCombat = false;
            ResetAttackCycle();
        }
        else
        {
            animator.ResetTrigger("sheathWeapon");
            animator.SetTrigger("drawWeapon");
            inCombat = true;
            ResetAttackCycle();
        }
    }

    public void TryAttack()
    {
        if (!inCombat || !canAttack)
            return;

        canAttack = false;
        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");

        // Alternate logic between attack1 and attack2 handled by animator state transitions
        attackIndex = 1 - attackIndex;

        Invoke(nameof(ResetAttack), attackCooldown); // Allow another attack after cooldown
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void ResetAttackCycle()
    {
        canAttack = true;
        attackIndex = 0;
    }
}
