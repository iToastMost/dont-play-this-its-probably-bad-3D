using Godot;
using System;

public partial class Enemy3D : CharacterBody3D
{
    PlayerRE player;
    Enemy3D enemy;
    public override void _Ready() 
    {
        player = GetNode<PlayerRE>("/root/GameManager/3DPlayer");
        
    }

    public override void _PhysicsProcess(double delta)
    {
        this.GlobalPosition.MoveToward(player.GlobalPosition, (float)delta);
        MoveAndSlide();
    }
}
