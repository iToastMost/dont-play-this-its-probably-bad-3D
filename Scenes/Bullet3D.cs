using Godot;
using System;

public partial class Bullet3D : Node3D
{
	private float _bulletSpeed = 15f;
	private Vector3 _velocity;
	RayCast3D bulletHitPoint;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bulletHitPoint = GetNode<RayCast3D>("RayCast3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Position += _velocity * (float)delta;
		bulletHitPoint.ForceRaycastUpdate();
		if (bulletHitPoint.IsColliding()) 
		{
			var hitInfo = bulletHitPoint.GetCollider();
			if(hitInfo is Enemy3D enemy) 
			{
				enemy.TakeDamage();
			}
			GD.Print(hitInfo.GetClass());
			QueueFree();
		}
	}

	public void SetDireciton(Vector3 direction) 
	{
		_velocity = direction * _bulletSpeed;
	}
}
