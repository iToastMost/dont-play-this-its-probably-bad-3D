using Godot;
using System;
using System.Collections.Generic;

public partial class FlyingEnemy : AnimatableBody2D
{
    private float _time = 0f;

    [Export]
    private float SineAmplitude = 200;

    [Export]
    private float SineSpeed = 1f;

    [Export]
    private float UpwardSpeed = 100f;

    private Vector2 _StartPosition;
    private Area2D area;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddToGroup("Enemies");
        _StartPosition = GlobalPosition;
        area = GetNode<Area2D>("Area2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        _time += (float)delta;

        float xOffset = Mathf.Sin(_time * SineSpeed) * SineAmplitude;
        float yOffset = -(float)_time * UpwardSpeed;

        Position = _StartPosition + new Vector2(xOffset, yOffset);

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
            PlayDeathSound();
            area.QueueFree();
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
    public void Hit()
    {
        PlayDeathSound();
    }
}
