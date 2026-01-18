using Godot;
using System;

public class ChaseState : EnemyState
{
	public override void Enter()
	{
		GD.Print("Entering ChaseState");
		_enemy.Velocity = Vector3.Zero;
		_enemy.EnemyAnimationPlayer.Stop();
		_enemy.EnemyAnimationPlayer.CurrentAnimation = "Walk_Formal";
		_enemy.EnemyAnimationPlayer.Play();
		MovementTarget = _enemy.player.GlobalPosition;
	}
 
	public Vector3 MovementTarget
	{
		get { return _enemy.NavAgent.TargetPosition; }
		set { _enemy.NavAgent.TargetPosition = value; }
	}
	
	public override void PhysicsUpdate(double delta)
	{
        _enemy._distanceToPlayer = _enemy.GlobalPosition.DistanceTo(_enemy.player.GlobalPosition);
		_enemy.LookAt(_enemy.player.GlobalPosition);
        if (_enemy._distanceToPlayer <= 1)
        {   
            _enemyStateMachine.ChangeState(EnemyStateTypes.Attack);
           
        }
        else 
        {
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

    private void LookAtInterpolation(float turnSpeed)
    {
	    Transform3D transform = _enemy.Transform;
	    transform = transform.LookingAt(_enemy.player.GlobalPosition, Vector3.Up);
	    _enemy.Transform = _enemy.Transform.InterpolateWith(transform, turnSpeed);
    }
}
