using Godot;
using System;

public class IdleState : PlayerState
{
    public override void Enter()
    {
        //animation code here
        player.PlayAnimation("Idle");
    }

    public override void PhysicsUpdate(double delta)
    {
        if (player.IsDead)
        {
            stateMachine.ChangeState(PlayerStateTypes.Dead);
            return;
        }

        if (player.IsAiming)
        {
            stateMachine.ChangeState(PlayerStateTypes.Aim);
            return;
        }
    }
}
