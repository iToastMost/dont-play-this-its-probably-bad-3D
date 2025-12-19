using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyStateMachine : Node
{
	private EnemyState _currentEnemyState;
	private Dictionary<EnemyStateTypes, EnemyState> _enemyStates = new (); 

	public void AddState(EnemyStateTypes enemyStateType, EnemyState enemyState)
	{
		_enemyStates[enemyStateType] = enemyState;
	}

	public void Initialize(Enemy3D enemy)
	{
		foreach(var state in _enemyStates)
		{
			state.Value.Initialize(enemy, this);
		}
	}

	public void ChangeState(EnemyStateTypes stateType)
	{
		if(_currentEnemyState == _enemyStates[stateType])
			return;

		_currentEnemyState?.Exit();
		_currentEnemyState = _enemyStates[stateType];
		_currentEnemyState?.Enter();
	}

	public void Update(double delta) => _currentEnemyState?.Update(delta);
	public void PhysicsUpdate(double delta) => _currentEnemyState?.PhysicsUpdate(delta);
}
