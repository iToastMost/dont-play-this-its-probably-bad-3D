using Godot;
using System;

public partial class Enemy3D : CharacterBody3D
{
    PlayerRE player;
    Enemy3D enemy;
    private int _health = 5;
    private float _enemyMoveSpeed = 15f;
    public override void _Ready() 
    {
        player = GetNode<PlayerRE>("/root/GameManager/3DPlayer");

        if(player != null) 
        {
            GD.Print("Player found!");
        }
        else 
        {
            GD.Print("Player not found.");
        }
        
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveTowardsPlayer(delta);
        this.LookAt(player.GlobalPosition, Vector3.Up);
        MoveAndSlide();
    }

    private void MoveTowardsPlayer(double delta) 
    {
        var direction = Vector3.Zero;
        var transform = Transform;
        var playerDirection = (this.GlobalPosition - player.GlobalPosition);
        direction = playerDirection.Normalized() / 25f;
        Position -= direction * (float)delta * _enemyMoveSpeed;
        //this.GlobalPosition.MoveToward(direction, (float)delta * 25f);

    }

    public void TakeDamage()
    {
        _health--;
        if (_health <= 0) 
        {
            QueueFree();
        }
    }
}
