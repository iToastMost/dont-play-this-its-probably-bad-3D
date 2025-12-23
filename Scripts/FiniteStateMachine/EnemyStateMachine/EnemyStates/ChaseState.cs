using Godot;
using System;

public class ChaseState : EnemyState
{
	public override void Enter()
	{
			
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
        _enemy.Position -=  _enemy.Transform.Basis.Z * _enemy._enemyMoveSpeed * (float)delta;
		
        LookAtInterpolation(_enemy.EnemyTurnSpeed);
    }

    private void LookAtInterpolation(float turnSpeed)
    {
	    Transform3D transform = _enemy.Transform;
	    transform = transform.LookingAt(_enemy.player.GlobalPosition, Vector3.Up);
	    _enemy.Transform = _enemy.Transform.InterpolateWith(transform, turnSpeed);
    }
}
