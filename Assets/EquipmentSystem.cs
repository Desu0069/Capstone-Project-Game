using System;
using System.Collections;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    public enum WeaponState { Sheathed, Drawing, Drawn, Sheathing }

    [Header("Weapon References")]
    public Transform weaponHand;
    public Transform weaponSheath;
    public GameObject weaponPrefab;

    [Header("Animation Timing")]
    [SerializeField] private float drawWeaponDelay = 0.7f;
    [SerializeField] private float sheathWeaponDelay = 1.0f;

    [Header("Optional Dependencies")]
    [SerializeField] private CombatSystem _combatSystem;

    // For external control, e.g. cutscenes, stuns, etc.
    public bool canEquipWeapon => MechanicStateManager.Instance.CanEquipWeapon;
    public bool canUnequipWeapon => MechanicStateManager.Instance.CanUnequipWeapon;
    public void EnableEquipWeapon(bool enable) { MechanicStateManager.Instance.CanEquipWeapon = enable; }
    public void EnableUnequipWeapon(bool enable) { MechanicStateManager.Instance.CanUnequipWeapon = enable; }

    public event Action OnDrawComplete;
    public event Action OnSheathComplete;
    public event Action OnWeaponTransitionComplete;

    private GameObject _currentWeapon;
    private WeaponState _weaponState = WeaponState.Sheathed;
    private Animator _animator;
    private bool _isTransitioning = false;
    private bool _queuedSheathRequest = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_combatSystem == null)
            _combatSystem = GetComponent<CombatSystem>();
    }

    private void Start()
    {
        SpawnWeaponInSheath();
        UpdateAnimatorParameters();
    }

    private void Update()
    {
        // Process queued sheath after attack
        if (_queuedSheathRequest && _weaponState == WeaponState.Drawn && (_combatSystem == null || !_combatSystem.IsAttacking()))
        {
            _queuedSheathRequest = false;
            SheathWeapon();
        }
        UpdateAnimatorParameters();
    }

    public WeaponState CurrentWeaponState => _weaponState;
    public bool CanAttackWithWeapon() => _weaponState == WeaponState.Drawn;
    public bool IsWeaponDrawn() => _weaponState == WeaponState.Drawn;
    public bool IsTransitioning => _isTransitioning;

    public void SpawnWeaponInSheath()
    {
        if (_currentWeapon != null)
            Destroy(_currentWeapon);

        if (weaponPrefab == null || weaponSheath == null)
        {
            Debug.LogError("Weapon prefab or sheath not assigned!");
            return;
        }

        _currentWeapon = Instantiate(weaponPrefab, weaponSheath);
        ResetWeaponTransform(_currentWeapon.transform);
        _weaponState = WeaponState.Sheathed;
    }

    public bool TryToggleWeapon()
    {
        if (_isTransitioning) return false;
        if (_weaponState == WeaponState.Sheathed)
        {
            if (!canEquipWeapon) return false;
            DrawWeapon();
            return true;
        }
        else if (_weaponState == WeaponState.Drawn)
        {
            if (!canUnequipWeapon) return false;
            SheathWeapon();
            return true;
        }
        return false;
    }

    public void DrawWeapon()
    {
        if (_isTransitioning || _weaponState != WeaponState.Sheathed) return;
        if (!canEquipWeapon) return;
        if (_combatSystem != null && _combatSystem.IsAttacking()) return;

        _isTransitioning = true;
        _weaponState = WeaponState.Drawing;
        _animator.SetTrigger("drawWeapon");
        StartCoroutine(DelayedDrawWeapon());
    }

    private IEnumerator DelayedDrawWeapon()
    {
        yield return new WaitForSeconds(drawWeaponDelay);
        CompleteDraw();
    }

    private void CompleteDraw()
    {
        if (_weaponState == WeaponState.Drawn) return;

        _currentWeapon.transform.SetParent(weaponHand);
        ResetWeaponTransform(_currentWeapon.transform);
        _weaponState = WeaponState.Drawn;
        _isTransitioning = false;
        _queuedSheathRequest = false; // Always clear any queued sheath requests
        UpdateAnimatorParameters();
        OnDrawComplete?.Invoke();
        OnWeaponTransitionComplete?.Invoke();
    }

    public void SheathWeapon()
    {
        if (_isTransitioning || _weaponState != WeaponState.Drawn) return;
        if (!canUnequipWeapon) return;
        if (_combatSystem != null && _combatSystem.IsAttacking())
        {
            _queuedSheathRequest = true;
            return;
        }

        _isTransitioning = true;
        _weaponState = WeaponState.Sheathing;
        _animator.SetTrigger("sheathWeapon");
        StartCoroutine(DelayedSheathWeapon());
    }

    private IEnumerator DelayedSheathWeapon()
    {
        yield return new WaitForSeconds(sheathWeaponDelay);

       
            CompleteSheath();
        
    }

    private void CompleteSheath()
    {
        if (_weaponState == WeaponState.Sheathed) return;

        _currentWeapon.transform.SetParent(weaponSheath);
        ResetWeaponTransform(_currentWeapon.transform);
        _weaponState = WeaponState.Sheathed;
        _isTransitioning = false;
        _queuedSheathRequest = false; // Always clear after completion
        UpdateAnimatorParameters();
        OnSheathComplete?.Invoke();
        OnWeaponTransitionComplete?.Invoke();
    }

    private void ResetWeaponTransform(Transform weapon)
    {
        weapon.localPosition = Vector3.zero;
        weapon.localRotation = Quaternion.identity;
        weapon.localScale = Vector3.one;
    }

    private void UpdateAnimatorParameters()
    {
        if (_animator != null)
            _animator.SetBool("isWeaponDrawn", _weaponState == WeaponState.Drawn);
        if (_combatSystem != null)
            _animator.SetBool("isAttacking", _combatSystem.IsAttacking());
    }

    // Animation events (optional)
    public void OnDrawAnimationStart() { }
    public void OnSheathAnimationEnd() { }

    public void StartDealDamage() => _currentWeapon?.GetComponentInChildren<DamageDealer>()?.StartDealDamage();
    public void EndDealDamage() => _currentWeapon?.GetComponentInChildren<DamageDealer>()?.EndDealDamage();
}