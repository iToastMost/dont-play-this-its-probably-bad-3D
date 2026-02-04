using Godot;
using System;

public class LootingState : PlayerState
{
    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        player.PlayAnimation("PickUp_Table");
    }
}
