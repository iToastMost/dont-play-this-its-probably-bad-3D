using Godot;
using System;

public class IdleState : PlayerState
{
    public override void Enter()
    {
        player.PlayAnimation("Idle");
    }

    public override void PhysicsUpdate(double delta)
    {
        if (player.IsDead)
        {
            stateMachine.ChangeState(PlayerStateTypes.Dead);
            return;
        }
        
        if (player.AimInput() && player.HandEquipmentSlot != null)
        {
            stateMachine.ChangeState(PlayerStateTypes.Aim);
            return;
        }

        if (player.ReloadInput() && (player.Ammo < 12 && !player.IsReloading))
        {
            stateMachine.ChangeState(PlayerStateTypes.Reload);
            return;
        }

        if (player.MovementInput() && player.CanMove)
        {
            stateMachine.ChangeState(PlayerStateTypes.Move);
            return;
        }
    }
}
