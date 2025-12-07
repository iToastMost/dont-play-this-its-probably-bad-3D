using Godot;
using System;

public partial class Bullet3D : Node3D
{
    [Export] private float _bulletSpeed = 15f;
    
	private Vector3 _velocity;
	RayCast3D bulletHitPoint;

	private GpuParticles3D particleSystem;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bulletHitPoint = GetNode<RayCast3D>("RayCast3D");
		particleSystem = GetNode<GpuParticles3D>("GPUParticles3D");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Position += _velocity * (float)delta;
		bulletHitPoint.ForceRaycastUpdate();
		if (bulletHitPoint.IsColliding())
		{
			particleSystem.Emitting = false;
			RemoveChild(particleSystem);
			GetParent().AddChild(particleSystem);
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
