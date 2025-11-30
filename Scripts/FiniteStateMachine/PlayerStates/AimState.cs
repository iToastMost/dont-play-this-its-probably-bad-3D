using Godot;
using System;

public class AimState : PlayerState
{
    public override void Enter()
    {
        player.laser.Visible = true;
        player.PlayAnimation("Pistol_Idle");
    }

    public override void PhysicsUpdate(double delta)
    {
        if (player.IsDead)
        {
            stateMachine.ChangeState(PlayerStateTypes.Dead);
            return;
        }
        
        if (!player.AimInput())
        {
            stateMachine.ChangeState(PlayerStateTypes.Idle);
            return;
        }

        player.AimLaser(delta);

        if (Input.IsActionJustPressed("shoot"))
        {
            player.Shoot();
        }
    }

    public override void Exit()
    {
        player.laser.Visible = false;
        player.laser.Rotation = new Vector3(0,0, Mathf.DegToRad(90));
    }
}
