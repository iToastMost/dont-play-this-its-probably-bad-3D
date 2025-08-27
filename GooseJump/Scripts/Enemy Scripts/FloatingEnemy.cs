using Godot;
using System;

public partial class FloatingEnemy : AnimatableBody2D
{
	[Export]
	private float _sineAmplitudeX = 200;

	[Export]
	private float _sineSpeedX = 1f;

	[Export]
	private float _sineSpeedY = 5f;

	[Export]
	private float _sineAmplitudeY = 25;

	private float _time = 0f;

	private float _tau;

	private Vector2 _startPosition;
	private Area2D _area2D;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("Enemies");
		_startPosition = Position;
		_area2D = GetNode<Area2D>("Area2D");
		_tau = Mathf.Tau * 2;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		_time += (float)delta;

		float xOffset = Mathf.Sin(_time * _sineSpeedX) * _sineAmplitudeX;
		float yOffset = Mathf.Sin(_time * _sineSpeedY) * _sineAmplitudeY;
		Position = _startPosition + new Vector2(xOffset, yOffset);
	}

	public void OnBodyEnter(Node2D body)
	{
		if (body is Player player)
		{
			player.Die();
		}
	}

	public void OnAreaEnter(Area2D area)
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
		//QueueFree();
	}
}
