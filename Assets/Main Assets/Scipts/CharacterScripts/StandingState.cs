using UnityEngine.TextCore.Text;
using UnityEngine;
using StarterAssets;

public class StandingState : State
{


    bool drawWeapon;

    Vector3 cVelocity;

    public StandingState(ThirdPersonController _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();



    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (drawWeaponAction.triggered)
        {
            drawWeapon = true;
        }
       

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (drawWeapon){
            character._animator.SetTrigger("drawWeapon");
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        

    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
       

        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }

}