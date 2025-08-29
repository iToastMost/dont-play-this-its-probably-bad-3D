using Godot;
using System;

public partial class BigEnemy : AnimatableBody2D
{
    [Export]
    private int _health = 3;

    public override void _Ready()
    {
        AddToGroup("Enemies");
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            player.Die();
        }
    }

    public void OnAreaEntered(Area2D area)
    {
        if (area.IsInGroup("Bullets"))
        {
            Hit();
            area.QueueFree();
        }
    }

    public void Hit() 
    {
        _health--;

        if(_health <= 0) 
        {
            PlayDeathSound();
        }
    }

    private void PlayDeathSound() 
    {
        var deathSound = GetNode<AudioStreamPlayer2D>("DeathSound");

        RemoveChild(deathSound);
        GetTree().Root.AddChild(deathSound);
        deathSound.Position = GlobalPosition;

        deathSound.Play();
        deathSound.Finished += () => deathSound.QueueFree();

        QueueFree();
    }
}
