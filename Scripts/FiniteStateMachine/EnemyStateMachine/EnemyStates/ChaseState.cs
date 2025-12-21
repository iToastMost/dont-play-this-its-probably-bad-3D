using Godot;
using System;

public class ChaseState : EnemyState
{
// 	public override void Enter()
// 	{
//  
// 	}
//  
// 	public override void PhysicsUpdate(double delta)
// 	{
// 		_enemy.LookAt(_enemy.player.GlobalPosition, Vector3.Up);   
//         _distanceToPlayer = this.GlobalPosition.DistanceTo(player.GlobalPosition);
//         EnemyAttackCooldownCheck(delta);
//  
//         if (_distanceToPlayer <= 1)
//         {   
//             esm.ChangeState(EnemyStateTypes.Attack);
//            
//         }
//         else 
//         {
//             MoveTowardsPlayer(delta);
//             enemyAnimation.Stop();
//         }
//         MoveAndSlide();
// 	}
//  
// 	public override void Exit()
// 	{
// 		
// 	}
//  
// 	 private void MoveTowardsPlayer(double delta) 
//     {
//         var direction = Vector3.Zero;
//         var transform = Transform;
//         var playerDirection = (this.GlobalPosition - player.GlobalPosition);
//         direction = playerDirection.Normalized() / 25f;
//         Position -= direction * (float)delta * _enemyMoveSpeed;
//         //this.GlobalPosition.MoveToward(direction, (float)delta * 25f);
//  
//     }
}
