using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{
    private PlayerState _currentPlayerState;
    private Dictionary<PlayerStateTypes, PlayerState> _playerStates = new();

    public void AddState(PlayerStateTypes stateType, PlayerState playerState)
    {
        _playerStates[stateType] = playerState;
    }

    public void Initialize(PlayerRE player)
    {
        foreach (var state in _playerStates.Values)
        {
            state.Initialize(player, this);
        }
    }
    
    public void ChangeState(PlayerStateTypes stateType)
    {
        if (_currentPlayerState == _playerStates[stateType])
            return;
        
        _currentPlayerState?.Exit();
        _currentPlayerState = _playerStates[stateType];
        _currentPlayerState.Enter();
    }

    public void Update(double delta) => _currentPlayerState?.Update(delta);
    public void PhysicsUpdate(double delta) => _currentPlayerState?.PhysicsUpdate(delta);
}
