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
        
        if (!player.MovementInput() || !player.CanMove)
        {
            stateMachine.ChangeState(PlayerStateTypes.Idle);
            return;
        }
        
        if (player.ReloadInput() && (player.Ammo < 12 && !player.IsReloading))
        {
            stateMachine.ChangeState(PlayerStateTypes.Reload);
            return;
        }
        
        if(player.CanMove)
         HandlePlayerMovement(delta);
    }

    private void HandlePlayerMovement(double delta)
    {
        var direction = Vector3.Zero;
        var transform = player.Transform;

        if (Input.IsActionJustPressed("spin_back"))
        {
            //Rotates 180 degrees.
            player.SpinBack = true;
        }

        if (player.SpinBack)
        {
            player.RotateY(Mathf.Pi);
            player.SpinBack = false;
        }
            

    if (Input.IsActionPressed("walk_forward")) 
        {
            if (Input.IsActionPressed("sprint")) 
            {
                player.Position += transform.Basis.X * (player.speed * 2) * (float)delta;
                player.playerAnimation.CurrentAnimation = "Jog_Fwd";
            }
            else 
            {
                player.Position += transform.Basis.X * player.speed * (float)delta;
                player.playerAnimation.CurrentAnimation = "Walk";
            }
        }

        if (Input.IsActionPressed("walk_back") || Input.IsActionPressed("sprint")) 
        {
            player.Position -= transform.Basis.X * player.speed / 2 * (float)delta;
        }

        if (Input.IsActionPressed("turn_left")) 
        {
            player.RotateY(player.turnSpeed * (float)delta);
        }
        if (Input.IsActionPressed("turn_right"))
        {
            player.RotateY(-player.turnSpeed * (float)delta);
        }
        
        if(direction != Vector3.Zero) 
        {
            direction = direction.Normalized();
            player.GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
        }

        player._targetVelocity.X = direction.X * player.speed;
        player._targetVelocity.Z = direction.Z * player.speed;

        player.Velocity = player._targetVelocity;
    }
    
}
