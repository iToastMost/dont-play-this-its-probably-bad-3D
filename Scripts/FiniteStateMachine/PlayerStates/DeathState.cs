using Godot;
using System;

public partial class DeathState : PlayerState
{
    public override void Enter()
    {
        player.PlayAnimation("Death01");
    }
}
