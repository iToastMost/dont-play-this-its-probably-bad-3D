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
        
        AimFlashlight(delta);
    }
    private void AimFlashlight(double delta)
    {
        if (Input.IsActionPressed("aim_right"))
        {
            player.FlashlightYaw.RotateY(-player.turnSpeed /2 * (float)delta);
        }
                    
        if (Input.IsActionPressed("aim_left"))
        {
            player.FlashlightYaw.RotateY(player.turnSpeed/2 * (float)delta);
        }
                    
        if (Input.IsActionPressed("aim_up"))
        {
            player.FlashlightPitch.RotateZ(player.turnSpeed/2 * (float)delta);
        }
                    
        if (Input.IsActionPressed("aim_down"))
        {
            player.FlashlightPitch.RotateZ(-player.turnSpeed/2 * (float)delta);
        }
                    
        float y = Mathf.Clamp(player.FlashlightYaw.Rotation.Y, Mathf.DegToRad(-135), Mathf.DegToRad(-65));
        float z = Mathf.Clamp(player.FlashlightPitch.Rotation.Z, Mathf.DegToRad(-30), Mathf.DegToRad(30));
        player.FlashlightYaw.Rotation = new Vector3(0,y,0);
        player.FlashlightPitch.Rotation =  new Vector3(0,0,z);
    }
}
