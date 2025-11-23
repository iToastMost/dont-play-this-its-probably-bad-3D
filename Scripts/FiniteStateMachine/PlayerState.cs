using Godot;
using System;

public abstract class PlayerState
{
    protected PlayerRE player;
    protected StateMachine stateMachine;

    public void Initialize(PlayerRE player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update(double delta) { }
    public virtual void PhysicsUpdate(double delta) { }
}
