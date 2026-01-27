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
		MovementTarget = _targetLocation;
		_wanderWaitTime = 0;
		_enemy.CanWander = true;
		_enemy.EnemyAnimationPlayer.CurrentAnimation = "Walk_Formal";
		_enemy.EnemyAnimationPlayer.Play();
	}

	public override void PhysicsUpdate(double delta)
	{
		_enemy.LookAt(MovementTarget);
		foreach (Node node in _rayCasts)
		{
			if (node is RayCast3D ray)
			{
				if (ray.GetCollider() is PlayerRE)
				{
					_enemy.CanWander = false;
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
		_enemy.WanderTimer.Stop();
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

	public Vector3 MovementTarget
	{
		get { return _enemy.NavAgent.TargetPosition; }
		set { _enemy.NavAgent.TargetPosition = value; }
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
			MovementTarget = _targetLocation;
			RandomWaitTime();
			return;
		}

		
		if (_enemy.NavAgent.IsNavigationFinished())
		{
			return;
		}

		Vector3 currentAgentPosition = _enemy.GlobalTransform.Origin;
		Vector3 nextPathPosition = _enemy.NavAgent.GetNextPathPosition();
		// var pathDirection = currentAgentPosition - nextPathPosition;
		//_enemy.LookAt(_enemy.GlobalPosition + pathDirection, Vector3.Up);

		_enemy.Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * _wanderSpeed;
		//var direction = Vector3.Back;
		//var playerDirection = (_enemy.GlobalPosition - location);
		
		//direction = playerDirection.Normalized() / 25f;
		//direction = direction.Normalized() / 25f;
		//_enemy.Position -= direction * (float)delta * _enemy._enemyMoveSpeed;
		
		//_enemy.Position -=  _enemy.Transform.Basis.Z * _wanderSpeed * (float)delta;
		//_enemy.Velocity = direction * _wanderSpeed;
		//_enemy.LookAt(location);
		
		LookAtInterpolation(nextPathPosition, _enemy.EnemyTurnSpeed, delta);
	}

	private void LookAtInterpolation(Vector3 lookPosition, float turnSpeed, double delta)
	{
		var transform = _enemy.GlobalTransform.LookingAt(new Vector3(lookPosition.X, 0, 0), Vector3.Up);
		_enemy.GlobalTransform = _enemy.GlobalTransform.InterpolateWith(transform, turnSpeed * (float)delta);
		
		
		//For reference
		//var transform = _enemy.GlobalTransform.LookingAt(_enemy.player.GlobalTransform.Origin, Vector3.Up);
		//_enemy.GlobalTransform = _enemy.GlobalTransform.InterpolateWith(transform, turnSpeed * (float) delta);
	}

	private void RandomWaitTime()
	{
		var rng = new Random();
		_wanderWaitTime = (rng.NextDouble() + 1) * 3;
		
		_enemy.WanderTimer.WaitTime = _wanderWaitTime;
		
		_enemy.WanderTimer.Start();
	}
	
}
