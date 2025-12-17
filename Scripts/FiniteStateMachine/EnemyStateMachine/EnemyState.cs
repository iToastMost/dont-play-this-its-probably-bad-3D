using Godot;
using System;

public abstract class EnemyState
{
	protected Enemy3D _enemy;
	protected EnemyStateMachine _enemyStateMachine;

	public void Initialize(Enemy3D enemy, EnemyStateMachine esm)
	{
		this._enemy = enemy;
		this._enemyStateMachine = esm;
	}

	public virtual void Enter();
	public virtual void Exit();
	public virtual void Update(double delta);
	public virtual void PhysicsUpdate(double delta);
}
