using Godot;
using System;
using System.Collections.Generic;

public class WanderState : EnemyState
{
	private List<RayCast3D> _rayCasts;
	public override void Enter()
	{
		_rayCasts = _enemy.GetRayCasts();
	}

	public override void PhysicsUpdate(double delta)
	{
		foreach (Node node in _rayCasts)
		{
			if (node is RayCast3D ray)
			{
				if (!ray.IsColliding())
					return;
				
				if (ray.GetCollider() is PlayerRE)
				{ 
					_enemyStateMachine.ChangeState(EnemyStateTypes.Chase);
				}
					
			}
		}
	}

	public override void Exit()
	{
		
	}
}
