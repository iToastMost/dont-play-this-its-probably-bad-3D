using Godot;
using System;
using System.Collections.Generic;

public class ChaseState : EnemyState
{
	private List<RayCast3D> _rayCasts;
	public override void Enter()
	{
		GD.Print("Entering ChaseState");
		_enemy.Velocity = Vector3.Zero;
		_enemy.EnemyAnimationPlayer.Stop();
		_enemy.EnemyAnimationPlayer.CurrentAnimation = "Walk_Formal";
		_enemy.EnemyAnimationPlayer.Play();
		MovementTarget = _enemy.player.GlobalPosition;
		_rayCasts = _enemy.GetRayCasts();
	}
 
	public Vector3 MovementTarget
	{
		get { return _enemy.NavAgent.TargetPosition; }
		set { _enemy.NavAgent.TargetPosition = value; }
	}
	
	public override void PhysicsUpdate(double delta)
	{
        _enemy._distanceToPlayer = _enemy.GlobalPosition.DistanceTo(_enemy.player.GlobalPosition);
		//_enemy.LookAt(_enemy.player.GlobalPosition);
		LookAtInterpolation(_enemy.EnemyTurnSpeed, delta);
        if (_enemy._distanceToPlayer <= 1)
        {   
	        foreach (Node node in _rayCasts)
	        {
		        if (node is RayCast3D ray)
		        {
			        if (ray.GetCollider() is PlayerRE)
			        {
				        GD.Print(ray.GetName());
				        //This is scuffed as hell please fix later
				        if(ray.GetName().Equals("RayCast3D3") || ray.GetName().Equals("RayCast3D4") || ray.GetName().Equals("RayCast3D"))
							_enemyStateMachine.ChangeState(EnemyStateTypes.Attack);
			        }
		        }
	        }
            
        }
        else 
        {
	        if (_enemy.EnemyAnimation.CurrentAnimation != "Walk_Formal")
	        {
		        //_enemy.EnemyAnimationPlayer.Stop();
		        _enemy.EnemyAnimationPlayer.CurrentAnimation = "Walk_Formal";
		        //_enemy.EnemyAnimationPlayer.Play();
	        }
		        
            MoveTowardsPlayer(delta);
            _enemy.EnemyAnimation.Stop();
        }
        _enemy.MoveAndSlide();
	}
 
	public override void Exit()
	{
		
	}
 
	 private void MoveTowardsPlayer(double delta)
	 {
		 var direction = Vector3.Zero;
		 var playerDirection = (_enemy.GlobalPosition - _enemy.player.GlobalPosition);
		 direction = playerDirection.Normalized();
		 MovementTarget = _enemy.player.GlobalPosition;
        //_enemy.Position -=  _enemy.Transform.Basis.Z * _enemy._enemyMoveSpeed * (float)delta;
        //_enemy.Position -=  direction * _enemy._enemyMoveSpeed * (float)delta;
        
        if (_enemy.NavAgent.IsNavigationFinished())
        {
	        return;
        }

        Vector3 currentAgentPosition = _enemy.GlobalTransform.Origin;
        Vector3 nextPathPosition = _enemy.NavAgent.GetNextPathPosition();
        // var pathDirection = currentAgentPosition - nextPathPosition;
        // _enemy.LookAt(_enemy.GlobalPosition + pathDirection, Vector3.Up);   
        
        _enemy.Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * _enemy._enemyMoveSpeed;
        //LookAtInterpolation(_enemy.EnemyTurnSpeed);
    }

    private void LookAtInterpolation(float turnSpeed, double delta)
    {
	    //Transform3D transform = _enemy.Transform;
	    var transform = _enemy.GlobalTransform.LookingAt(_enemy.player.GlobalTransform.Origin, Vector3.Up);
	    _enemy.GlobalTransform = _enemy.GlobalTransform.InterpolateWith(transform, turnSpeed * (float) delta);
    }
}
