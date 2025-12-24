using Godot;
using System;
using System.Collections.Generic;

public class WanderState : EnemyState
{
	private Vector3 _targetLocation = Vector3.Zero;
	private long _wanderDistance = 5;
	private double _wanderWaitTime;
	private float _wanderSpeed = 1f;
	
	private List<RayCast3D> _rayCasts;
	public override void Enter()
	{
		_rayCasts = _enemy.GetRayCasts();
		_targetLocation = GenerateRandomLocation();
		_wanderWaitTime = 0;
		_enemy.CanWander = true;
		_enemy.EnemyAnimationPlayer.CurrentAnimation = "Walk_Formal";
		_enemy.EnemyAnimationPlayer.Play();
	}

	public override void PhysicsUpdate(double delta)
	{
		foreach (Node node in _rayCasts)
		{
			if (node is RayCast3D ray)
			{
				if (ray.GetCollider() is PlayerRE)
				{ 
					_enemyStateMachine.ChangeState(EnemyStateTypes.Chase);
				}
			}
		}

		if (!_enemy.CanWander)
			return;
		
		MoveToLocation(_targetLocation, delta);
		_enemy.MoveAndSlide();
	}

	public override void Exit()
	{
		
	}

	private Vector3 GenerateRandomLocation()
	{
		var rng = new Random();
		var randX = rng.NextInt64(-_wanderDistance, _wanderDistance);
		var randZ = rng.NextInt64(-_wanderDistance, _wanderDistance);
		
		var currentLocation = _enemy.GlobalPosition;
		Vector3 newLocation =  new Vector3(currentLocation.X + randX, currentLocation.Y, currentLocation.Z + randZ);
		
		return newLocation;
	}

	private void MoveToLocation(Vector3 location, double delta)
	{
		Vector3 direction = Vector3.Zero;
		direction = location - _enemy.GlobalPosition;
		//GD.Print(_enemy.GlobalPosition - location);
		//GD.Print(location - _enemy.GlobalPosition);
		if (_enemy.GlobalPosition.DistanceTo(location) < 1)
		{
			_enemy.CanWander = false;
			_enemy.EnemyAnimationPlayer.CurrentAnimation = "Idle";
			_targetLocation = GenerateRandomLocation();
			RandomWaitTime();
			return;
		}

		//var direction = Vector3.Back;
		//var playerDirection = (_enemy.GlobalPosition - location);
		
		//direction = playerDirection.Normalized() / 25f;
		//direction = direction.Normalized() / 25f;
		//_enemy.Position -= direction * (float)delta * _enemy._enemyMoveSpeed;
		
		//_enemy.Position -=  _enemy.Transform.Basis.Z * _wanderSpeed * (float)delta;
		_enemy.Velocity = direction * _wanderSpeed;
		_enemy.LookAt(location);
		
		//LookAtInterpolation(_enemy.EnemyTurnSpeed);
	}

	private void LookAtInterpolation(float turnSpeed)
	{
		_targetLocation.Y = _enemy.GlobalPosition.Y;
		Transform3D transform = _enemy.Transform;
		transform = transform.LookingAt(_targetLocation, Vector3.Up);
		_enemy.Transform = _enemy.Transform.InterpolateWith(transform, turnSpeed);
	}

	private void RandomWaitTime()
	{
		var rng = new Random();
		_wanderWaitTime = rng.NextDouble() * 3;
		
		_enemy.WanderTimer.WaitTime = _wanderWaitTime;
		
		_enemy.WanderTimer.Start();
	}
}
