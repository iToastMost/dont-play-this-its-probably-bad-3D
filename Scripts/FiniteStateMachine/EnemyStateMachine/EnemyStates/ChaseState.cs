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
        var direction = Vector3.Zero;
        var playerDirection = (_enemy.GlobalPosition -_enemy.player.GlobalPosition);
        direction = playerDirection.Normalized() / 25f;
        _enemy.Position -= direction * (float)delta * _enemy._enemyMoveSpeed;
        //this.GlobalPosition.MoveToward(direction, (float)delta * 25f);
 
    }
}
