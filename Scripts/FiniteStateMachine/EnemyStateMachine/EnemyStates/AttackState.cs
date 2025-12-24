using Godot;
using System;

public class AttackState : EnemyState
{
	private const double ENEMY_ATTACK_CD = 2.5;
	private double _lastAttack = 0;
	private bool _canAttack;
	public override void Enter()
	{
		_canAttack = true;
	}
 
	public override void PhysicsUpdate(double delta)
	{
		
		EnemyAttackCooldownCheck(delta);
		
		_enemy._distanceToPlayer = _enemy.GlobalPosition.DistanceTo(_enemy.player.GlobalPosition);
		
		if(_enemy._distanceToPlayer > 1 && _canAttack)
			_enemyStateMachine.ChangeState(EnemyStateTypes.Chase);
		
		if(_canAttack)
			AttackPlayer();
	}
 
	public override void Exit()
	{
		
	}
 
	private void AttackPlayer() 
    {
        _canAttack = false;
        _lastAttack = ENEMY_ATTACK_CD;
        _enemy.EnemyAnimation.CurrentAnimation = "attack";
        _enemy.EnemyAnimationPlayer.CurrentAnimation = "Sword_Attack";
        _enemy.EnemyAnimation.Play();
    }
 
	private void EnemyAttackCooldownCheck(double delta)
    {
        _lastAttack -= delta;
        if (_lastAttack <= 0) 
        {
            _canAttack = true;
        }
        else if (_lastAttack <= 1.5)
        {
	        _enemy.EnemyAnimationPlayer.CurrentAnimation = "Idle";
        }
    }
}
