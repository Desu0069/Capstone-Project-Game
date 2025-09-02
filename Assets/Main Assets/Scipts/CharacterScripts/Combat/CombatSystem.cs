using StarterAssets;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private string _attackTrigger = "attack";
    [SerializeField] private float _comboResetTime = 0.5f;

    [Header("Dependencies")]
    [SerializeField] private EquipmentSystem _equipmentSystem; // Drag your EquipmentSystem here in inspector
    public bool canAttack => MechanicStateManager.Instance.CanAttack;
    public void EnableAttack(bool enable) { MechanicStateManager.Instance.CanAttack = enable; }
    private Animator _animator;
    private StarterAssetsInputs _input;
    private float _lastAttackTime;
    private bool _canCombo;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _input = GetComponent<StarterAssetsInputs>();
        // Optionally auto-find EquipmentSystem if not assigned:
        if (_equipmentSystem == null)
            _equipmentSystem = GetComponent<EquipmentSystem>();
    }

    private void Update()
    {
        if (_animator == null || _input == null || _equipmentSystem == null) return;
        if (!canAttack) return; // <-- gating!
        if (!_animator.GetBool("Grounded")) return;
        if (!_equipmentSystem.IsWeaponDrawn()) return;

        if (_input.attack)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
       


        // If we're currently in an attack animation and can combo
        if (IsAttacking() && _canCombo)
        {
            // Trigger next attack in chain
            _animator.SetTrigger(_attackTrigger);
            _lastAttackTime = Time.time;
            _canCombo = false; // Will be set true again via animation event
        }
        // If not currently attacking
        else if (!IsAttacking())
        {
            // Start new attack chain
            _animator.SetTrigger(_attackTrigger);
            _lastAttackTime = Time.time;
            _canCombo = false;
        }
    }

    public bool IsAttacking()
    {
        var state = _animator.GetCurrentAnimatorStateInfo(1); // Combat layer
        return state.IsName("combat_attack") || state.IsName("combat_attack2");
    }

    // Call this via Animation Event at the frame where combo is possible
    public void EnableCombo()
    {
        _canCombo = true;
    }

    // Call this via Animation Event at the end of attack animations
    public void DisableCombo()
    {
        _canCombo = false;
    }
}