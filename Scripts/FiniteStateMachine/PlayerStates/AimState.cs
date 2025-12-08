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

        if (player.ReloadInput() && (player.Ammo < 12 && !player.IsReloading))
        {
            stateMachine.ChangeState(PlayerStateTypes.Reload);
            return;
        }
        
        HandlePlayerAim(delta);

        if (Input.IsActionJustPressed("shoot"))
        {
            Shoot();
        }
    }

    public void HandlePlayerAim(double delta)
    {
        if (Input.IsActionPressed("aim_right"))
        {
            player.laser.RotateY(-player.turnSpeed /2 * (float)delta);
        }
                    
        if (Input.IsActionPressed("aim_left"))
        {
            player.laser.RotateY(player.turnSpeed/2 * (float)delta);
        }
                    
        if (Input.IsActionPressed("aim_up"))
        {
            player.laser.RotateZ(player.turnSpeed/2 * (float)delta);
        }
                    
        if (Input.IsActionPressed("aim_down"))
        {
            player.laser.RotateZ(-player.turnSpeed/2 * (float)delta);
        }
                    
        float y = Mathf.Clamp(player.laser.Rotation.Y, Mathf.DegToRad(-45), Mathf.DegToRad(45));
        float z = Mathf.Clamp(player.laser.Rotation.Z, Mathf.DegToRad(80), Mathf.DegToRad(100));
        player.laser.Rotation = new Vector3(0,y,z);
    }
    
    public void Shoot() 
    {
        if(player.Ammo > 0) 
        {
            player.Ammo--;

            Vector3 direction;
            
            var bulletSpawn = player.bullet.Instantiate<Bullet3D>();
            
            if (player.laser.IsColliding())
            {
                direction = (player.laser.GlobalPosition - player.laser.GetCollisionPoint()).Normalized();
                bulletSpawn.LookAtFromPosition(player.laser.GlobalPosition, player.laser.GetCollisionPoint());
            }
            else
            {
                direction = (player.laser.GlobalPosition - player.laser.TargetPosition).Normalized();
                var globalTargetPosition = player.laser.ToGlobal(player.laser.TargetPosition);
                bulletSpawn.LookAtFromPosition(player.laser.GlobalPosition, globalTargetPosition);
            }
            bulletSpawn.SetDireciton(-direction);
            player.GetParent().AddChild(bulletSpawn);
            player.UpdateAmmo(player.Ammo);
            GD.Print("Pew pew");
        }

    }

    public override void Exit()
    {
        player.laser.Visible = false;
        player.laser.Rotation = new Vector3(0,0, Mathf.DegToRad(90));
    }
}
