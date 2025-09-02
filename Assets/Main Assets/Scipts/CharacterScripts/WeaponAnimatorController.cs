using StarterAssets;
using UnityEngine;

public class WeaponAnimatorController : MonoBehaviour
{
    [Header("Animator Parameters")]
    [SerializeField] private string _drawWeaponTrigger = "drawWeapon";
    [SerializeField] private string _sheathWeaponTrigger = "sheathWeapon";
    [SerializeField] private string _speedParameter = "Speed";
    [SerializeField] private float _movementThreshold = 0.01f;

    [Header("Layer Settings")]
    [SerializeField] private int _combatLayerIndex = 2; // Set to your Combat layer index
    [SerializeField] private int _armsLayerIndex = 3;    // Set to your Arms layer index

    
    private Animator _animator;
    private ThirdPersonController _playerController;
    private bool _weaponDrawn = false;



    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<ThirdPersonController>();
    }

    private void Update()
    {
        HandleWeaponInput();
        UpdateLayerWeights();
    }

    private void HandleWeaponInput()
    {
        if (_playerController._input.weaponToggle) // Access input via ThirdPersonController
        {
            if (!_weaponDrawn && CanDrawWeapon())
            {
                DrawWeapon();
            }
            else if (_weaponDrawn && CanSheathWeapon())
            {
                SheathWeapon();
            }
            _playerController._input.weaponToggle = false; // Reset input after use
        }
    }

    public bool CanDrawWeapon()
    {
        // Can draw when standing still OR while moving (per your conditions)
        return true;
    }

    public bool CanSheathWeapon()
    {
        // Can sheath when standing still OR while moving (per your conditions)
        return true;
    }

    public void DrawWeapon()
    {
        if (_weaponDrawn) return;

        _animator.ResetTrigger(_sheathWeaponTrigger);
        _animator.SetTrigger(_drawWeaponTrigger);
        _weaponDrawn = true;
    }

    public void SheathWeapon()
    {
        if (!_weaponDrawn) return;

        _animator.ResetTrigger(_drawWeaponTrigger);
        _animator.SetTrigger(_sheathWeaponTrigger);
        _weaponDrawn = false;
    }

    public void UpdateLayerWeights()
    {
        // Combat layer: Full weight when weapon drawn
        _animator.SetLayerWeight(_combatLayerIndex, _weaponDrawn ? 1f : 0f);

        
    }

    // Call these from animation events if needed
    public void OnDrawWeaponComplete()
    {
        // Optional: Handle post-draw logic
    }

    public void OnSheathWeaponComplete()
    {
        // Optional: Handle post-sheath logic
    }
}