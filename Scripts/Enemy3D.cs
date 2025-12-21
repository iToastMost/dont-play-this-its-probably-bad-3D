using Godot;
using System;

public partial class Enemy3D : CharacterBody3D
{
    public PlayerRE player;
    Enemy3D enemy;
    public AnimationPlayer EnemyAnimation;
    Area3D enemyAttackBox;
    EnemyStateMachine esm;

    private int _health = 5;
    public float _enemyMoveSpeed = 15f;
    public float _distanceToPlayer;
    
    [Export]
    public int AttackDamage { get; set; }

    public override void _Ready() 
    {
        player = GetNode<PlayerRE>("/root/GameManager/3DPlayer");
        EnemyAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
        enemyAttackBox = GetNode<Area3D>("AttackBox");

        esm = new EnemyStateMachine();

        esm.AddState(EnemyStateTypes.Wander, new WanderState());
        esm.AddState(EnemyStateTypes.Chase, new ChaseState());
        esm.AddState(EnemyStateTypes.Attack, new AttackState());

        esm.Initialize(this);
        //esm.ChangeState(EnemyStateTypes.Wander);
        esm.ChangeState(EnemyStateTypes.Chase);

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

    public void TakeDamage()
    {
        _health--;
        if (_health <= 0) 
        {
            QueueFree();
        }
    }
}
