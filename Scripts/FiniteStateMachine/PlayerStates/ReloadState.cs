using Godot;
using System;

public class ReloadState : PlayerState
{
    public override void Enter()
    {
        //player.PlayAnimation("Pistol_Reload");
    }

    public override void PhysicsUpdate(double delta)
    {
        if (player.IsDead)
        {
            stateMachine.ChangeState(PlayerStateTypes.Dead);
            return;
        }
        
        player.SendReloadCheck();
        if (!player.IsReloading)
        {
            stateMachine.ChangeState(PlayerStateTypes.Idle);
            return;
        }
    }
}
