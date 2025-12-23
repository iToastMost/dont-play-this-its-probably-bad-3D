using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy3D : CharacterBody3D
{
    public PlayerRE player;
    Enemy3D enemy;
    public AnimationPlayer EnemyAnimation;
    Area3D enemyAttackBox;
    EnemyStateMachine esm;
    private List<RayCast3D> _rayCasts;
    public MeshInstance3D WanderMesh;

    private int _health = 5;
    public float _enemyMoveSpeed = 1.0f;
    public float _distanceToPlayer;
    public float EnemyTurnSpeed = 0.1f;
    public Timer WanderTimer;
    public bool CanWander;
    
    [Export]
    public int AttackDamage { get; set; }

    public override void _Ready()
    {
        player = GetNode<PlayerRE>("/root/GameManager/3DPlayer");
        EnemyAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
        enemyAttackBox = GetNode<Area3D>("AttackBox");
        WanderTimer = GetNode<Timer>("WanderTimer");
        
        WanderTimer.Timeout += WanderTimeout;

        _rayCasts = new();
        

        foreach(Node node in GetNode<Node3D>("LineOfSight").GetChildren())
        {
            if(node is RayCast3D raycast)
                _rayCasts.Add(raycast);
        }

        esm = new EnemyStateMachine();

        esm.AddState(EnemyStateTypes.Wander, new WanderState());
        esm.AddState(EnemyStateTypes.Chase, new ChaseState());
        esm.AddState(EnemyStateTypes.Attack, new AttackState());

        esm.Initialize(this);
        esm.ChangeState(EnemyStateTypes.Wander);
        //esm.ChangeState(EnemyStateTypes.Chase);

        if(player != null) 
        {
            GD.Print("Player found!");
        }
        else 
        {
            GD.Print("Player not found.");
        }
        
    }

    public override void _Process(double delta)
    {
        esm.PhysicsUpdate(delta);
    }
    
    public void OnBodyEntered3D(Node3D body) 
    {
        if(body is PlayerRE player) 
        {
            GD.Print("Dealing damage to player");
            player.TakeDamage(AttackDamage);
        }
    }

    public List<RayCast3D> GetRayCasts()
    {
        return _rayCasts;
    }

    private void WanderTimeout()
    {
        CanWander = true;
    }
    
    public void TakeDamage()
    {
        _health--;
        if (_health <= 0) 
        {
            QueueFree();
        }
    }
}
