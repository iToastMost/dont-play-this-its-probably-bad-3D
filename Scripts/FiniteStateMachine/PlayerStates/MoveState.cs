using Godot;
using System;

public class MoveState : PlayerState
{
    public override void Enter()
    {
        player.PlayAnimation("Walk");
    }
    
    public override void PhysicsUpdate(double delta)
    {
        if (Input.IsActionJustPressed("spin_back") && player.CanMove)
        {
            player.RotateY(Mathf.Pi);
            return;
        }
        
        if (player.IsDead)
        {
            stateMachine.ChangeState(PlayerStateTypes.Dead);
            return;
        }

        if (player.AimInput())
        {
            stateMachine.ChangeState(PlayerStateTypes.Aim);
            return;
        }

        if (!player.MovementInput() || !player.CanMove)
        {
            stateMachine.ChangeState(PlayerStateTypes.Idle);
            return;
        }

        if (player.CanMove && Input.IsActionJustPressed("spin_back"))
        {
            player.RotateY(Mathf.Pi);    
        }
        
        if(player.CanMove)
         player.HandleMovement(delta);
    }
    
}
