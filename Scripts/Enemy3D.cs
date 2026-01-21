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
    public AnimationPlayer EnemyAnimationPlayer;
    
    public CollisionShape3D EnemyCollider;
    public CollisionShape3D AttackCollider;
    public Area3D AttackArea;

    public bool IsDead = false;
    private int _health = 5;
    public float _enemyMoveSpeed = 1.0f;
    public float _distanceToPlayer;
    public float EnemyTurnSpeed = 0.1f;
    public Timer WanderTimer;
    public Timer HitStunTimer;
    public bool CanWander;
    
    public NavigationAgent3D  NavAgent;
    
    [Export]
    public string ZoneId { get; set; }
    
    [Export]
    public string EnemyId { get; set; }
    
    [Export]
    public int AttackDamage { get; set; }

    public override void _Ready()
    {
        if (GameStateManager.Instance.IsEnemyKilled(ZoneId, EnemyId))
        {
            QueueFree();
            return;
        }
        player = GetNode<PlayerRE>("/root/GameManager/PlayerSetup/3DPlayer");
        EnemyAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
        enemyAttackBox = GetNode<Area3D>("AttackBox");
        WanderTimer = GetNode<Timer>("WanderTimer");
        HitStunTimer = GetNode<Timer>("HitStunTimer");
        EnemyAnimationPlayer = GetNode<AnimationPlayer>("EnemyModel/AnimationPlayer");
        NavAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        
        EnemyCollider = GetNode<CollisionShape3D>("CollisionShape3D");
        AttackCollider = GetNode<CollisionShape3D>("EnemyModel/Rig/Skeleton3D/BoneAttachment3D/Boxcutter/Area3D/CollisionShape3D");
        AttackArea = GetNode<Area3D>("EnemyModel/Rig/Skeleton3D/BoneAttachment3D/Boxcutter/Area3D");
        AttackArea.BodyEntered += OnBodyEntered3D;
        
        
        WanderTimer.Timeout += WanderTimeout;
        HitStunTimer.Timeout += HitStunTimeout;

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
        esm.AddState(EnemyStateTypes.HitStun, new HitStunState());
        esm.AddState(EnemyStateTypes.Death, new EnemyDeathState());

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
        
        NavAgent.PathDesiredDistance = 0.5f;
        NavAgent.TargetDesiredDistance = 0.5f;

        // Make sure to not await during _Ready.
        Callable.From(ActorSetup).CallDeferred();
        
    }

    public override void _Process(double delta)
    {
        if (IsDead)
            return;
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
        EnemyAnimationPlayer.CurrentAnimation = "Walk_Formal";
        CanWander = true;
    }

    private void HitStunTimeout()
    {
        if(!IsDead)
            esm.ChangeState(EnemyStateTypes.Chase);
    }
    
    public void TakeDamage()
    {
        esm.ChangeState(EnemyStateTypes.HitStun);
        
        Velocity = Vector3.Zero;
        //EnemyAnimationPlayer.Stop();
        EnemyAnimationPlayer.CurrentAnimation = "Hit_Chest";
        EnemyAnimationPlayer.Play();
        
        HitStunTimer.Start();
        _health--;
        if (_health <= 0) 
        {
            IsDead = true;
            GameStateManager.Instance.MarkEnemyKilled(ZoneId, EnemyId);
            esm.ChangeState(EnemyStateTypes.Death);
        }

        if (IsDead)
            return;
        
        if (esm.GetEnemyState() == EnemyStateTypes.Chase || esm.GetEnemyState() == EnemyStateTypes.HitStun || esm.GetEnemyState() == EnemyStateTypes.Attack)
            return;
        
        esm.ChangeState(EnemyStateTypes.Chase);
    }
    
    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
    }
}
