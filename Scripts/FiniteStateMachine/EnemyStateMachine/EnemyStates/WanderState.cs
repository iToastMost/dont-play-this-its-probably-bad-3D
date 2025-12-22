using Godot;
using System;
using System.Collections.Generic;

public class WanderState : EnemyState
{
	private Vector3 _targetLocation;
	
	private List<RayCast3D> _rayCasts;
	public override void Enter()
	{
		_rayCasts = _enemy.GetRayCasts();
		_targetLocation = GenerateRandomLocation();
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
		
		MoveToLocation(_targetLocation, delta);
	}

	public override void Exit()
	{
		
	}

	private Vector3 GenerateRandomLocation()
	{
		var rng = new Random();
		var randX = rng.NextInt64(-10, 10);
		var randY = rng.NextInt64(-10, 10);
		
		var currentLocation = _enemy.GlobalPosition;
		var newLocation = currentLocation;
		newLocation.X = newLocation.X + randX;
		newLocation.Z = newLocation.Z + randY;
		
		return newLocation;
	}

	private void MoveToLocation(Vector3 location, double delta)
	{
		if (_enemy.GlobalPosition.DistanceTo(location) < 0.5)
		{
			_targetLocation = GenerateRandomLocation();
		}
		
		var direction = Vector3.Zero;
		var playerDirection = (_enemy.GlobalPosition - location);
		
		direction = playerDirection.Normalized() / 25f;
		_enemy.Position -= direction * (float)delta * _enemy._enemyMoveSpeed;
	}
}
