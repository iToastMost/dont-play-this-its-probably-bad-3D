using Godot;
using System;

public partial class Enemy : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        AddToGroup("Enemies");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnBodyEntered(Node2D body) 
	{
		if(body is Player player) 
		{
			player.Die();
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
