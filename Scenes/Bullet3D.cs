using Godot;
using System;

public partial class Bullet3D : Node3D
{
	private float _bulletSpeed = 25f;
	private Vector3 _velocity;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Position += _velocity * (float)delta;
	}

	public void SetDireciton(Vector3 direction) 
	{
		_velocity = direction * _bulletSpeed;
	}
}
