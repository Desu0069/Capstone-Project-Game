using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

        [Header("Weapon Inputs")]
        public bool weaponToggle;
        public bool attack;

    
        [SerializeField] private EquipmentSystem equipmentSystem;
        private bool _canToggleWeapon = true;

        [Header("Movement Detection")]
        public bool isMoving; // Read-only in inspector

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

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



        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
            isMoving = move.magnitude > 0.1f; // Auto-update movement detection
        }

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

        public void OnWeaponToggle(InputValue value)
        {
            if (value.isPressed && _canToggleWeapon)
            {
                bool transitionStarted = equipmentSystem.TryToggleWeapon();
                if (transitionStarted)
                    _canToggleWeapon = false;
                // If not, don't lock out input!
            }
        }

        private void LateUpdate()
        {
            weaponToggle = false;
            attack = false;
        }

        public void OnAttack(InputValue value)
        {
            attack = value.isPressed;
        }

        public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}


#else
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && _canToggleWeapon)
            {
                equipmentSystem.TryToggleWeapon();
                _canToggleWeapon = false;
            }
        }


#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}