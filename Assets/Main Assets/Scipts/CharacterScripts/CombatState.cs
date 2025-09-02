using UnityEngine;
using UnityEngine.TextCore.Text;
using StarterAssets;
using UnityEngine.InputSystem;
public class CombatState : State
{
    
    bool sheathWeapon;
    bool attack;

    Vector3 cVelocity;

    public CombatState(ThirdPersonController _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        sheathWeapon = false;
  
        attack = false;

       
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (drawWeaponAction.triggered)
        {
            sheathWeapon = true;
        }

        if (attackAction.triggered)
        {
            attack = true;
        }

       

        
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        

        if (sheathWeapon)
        {
            character._animator.SetTrigger("sheathWeapon");
           
        }

        if (attack)
        {
            character._animator.SetTrigger("attack");
           
        }
    }

    

    

}