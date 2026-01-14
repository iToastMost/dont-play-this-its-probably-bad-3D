using Godot;
using System;

public class DialogState : PlayerState
{
    [Signal]
    public delegate void EndDialogEventHandler();
    
    public override void Enter()
    {
        player.PlayAnimation("Idle");
        player.CanMove = false;
    }

    public override void PhysicsUpdate(double delta)
    {
        if (Input.IsKeyPressed(Key.Enter))
        {
            stateMachine.ChangeState(PlayerStateTypes.Idle);
        }
    }

    public override void Exit()
    {
        player.CanMove = true;
    }
}
