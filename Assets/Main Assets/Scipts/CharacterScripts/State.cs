using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class State
{
    public ThirdPersonController character;
    public StateMachine stateMachine;

    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 input;

    public InputAction drawWeaponAction;
    public InputAction attackAction;

    public State(ThirdPersonController _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;


        drawWeaponAction = character._playerInput.actions["Crouch"];
        attackAction = character._playerInput.actions["Sprint"];

    }

    public virtual void Enter()
    {
        Debug.Log("enter state: " + this.ToString());
    }

    public virtual void HandleInput()
    {
    }

    public virtual void LogicUpdate()
    {
    }

    public virtual void PhysicsUpdate()
    {
    }

    public virtual void Exit()
    {
    }
}
