using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WeaponToggleInputDebouncer : MonoBehaviour
{
    [SerializeField] private EquipmentSystem equipmentSystem;
    private bool _canToggleWeapon = true;

    private void Awake()
    {
        if (equipmentSystem == null)
            equipmentSystem = GetComponent<EquipmentSystem>();
        equipmentSystem.OnWeaponTransitionComplete += OnWeaponTransitionComplete;
    }

    private void OnDestroy()
    {
        equipmentSystem.OnWeaponTransitionComplete -= OnWeaponTransitionComplete;
    }

    private void OnWeaponTransitionComplete()
    {
        _canToggleWeapon = true;
    }

#if ENABLE_INPUT_SYSTEM
    public void OnWeaponToggle(InputValue value)
    {
        if (value.isPressed && _canToggleWeapon)
        {
            equipmentSystem.TryToggleWeapon();
            _canToggleWeapon = false; // Block further toggles until transition completes
        }
    }
#else
    // For legacy input system (for Update polling)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _canToggleWeapon)
        {
            equipmentSystem.TryToggleWeapon();
            _canToggleWeapon = false;
        }
    }
#endif
}