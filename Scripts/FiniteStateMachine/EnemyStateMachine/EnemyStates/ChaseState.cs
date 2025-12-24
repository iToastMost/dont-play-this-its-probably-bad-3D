using Godot;
using System;

public class ChaseState : EnemyState
{
	public override void Enter()
	{
		_enemy.Velocity = Vector3.Zero;
		_enemy.EnemyAnimationPlayer.CurrentAnimation = "Walk_Formal";
		_enemy.EnemyAnimationPlayer.Play();
	}
 
	public override void PhysicsUpdate(double delta)
	{
		_enemy.LookAt(_enemy.player.GlobalPosition, Vector3.Up);   
        _enemy._distanceToPlayer = _enemy.GlobalPosition.DistanceTo(_enemy.player.GlobalPosition);
 
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
        //_enemy.Position -=  _enemy.Transform.Basis.Z * _enemy._enemyMoveSpeed * (float)delta;
        _enemy.Position -=  direction * _enemy._enemyMoveSpeed * (float)delta;
        //LookAtInterpolation(_enemy.EnemyTurnSpeed);
    }

    private void LookAtInterpolation(float turnSpeed)
    {
	    Transform3D transform = _enemy.Transform;
	    transform = transform.LookingAt(_enemy.player.GlobalPosition, Vector3.Up);
	    _enemy.Transform = _enemy.Transform.InterpolateWith(transform, turnSpeed);
    }
}
