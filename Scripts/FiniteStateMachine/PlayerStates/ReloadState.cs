using Godot;
using System;

public partial class ReloadState : PlayerState
{
    public override void Enter()
    {
        player.PlayAnimation("Pistol_Reload");
    }

    public override void PhysicsUpdate(double delta)
    {
        player.OnAnimationFinish("Pistol_Reload");
    }
}
