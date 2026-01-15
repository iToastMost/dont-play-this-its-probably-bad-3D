using Godot;
using System;

public class EnemyDeathState : EnemyState
{
	public override void Enter()
	{
		_enemy.EnemyCollider.Disabled = true;
		_enemy.AttackCollider.Disabled = true;
		_enemy.EnemyAnimationPlayer.Stop();
		_enemy.EnemyAnimationPlayer.CurrentAnimation = "Death01";
		_enemy.EnemyAnimationPlayer.Play();
	}
}
