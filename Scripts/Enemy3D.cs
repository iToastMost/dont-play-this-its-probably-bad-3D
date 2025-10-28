using Godot;
using System;

public partial class Enemy3D : CharacterBody3D
{
    PlayerRE player;
    Enemy3D enemy;
    AnimationPlayer enemyAnimation;
    Area3D enemyAttackBox;

    private int _health = 5;
    private float _enemyMoveSpeed = 15f;
    private float _distanceToPlayer;
    private const int ENEMY_ATTACK_CD = 5;
    private int _lastAttack = 0;
    private bool _canAttack = true;

    public override void _Ready() 
    {
        player = GetNode<PlayerRE>("/root/GameManager/3DPlayer");
        enemyAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
        enemyAttackBox = GetNode<Area3D>("AttackBox");


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
        this.LookAt(player.GlobalPosition, Vector3.Up);   
        _distanceToPlayer = this.GlobalPosition.DistanceTo(player.GlobalPosition);
        EnemyAttackCooldownCheck();

        if (_distanceToPlayer <= 1)
        {   
            if (_canAttack) 
            {
                AttackPlayer();
            }
           
        }
        else 
        {
            MoveTowardsPlayer(delta);
            enemyAnimation.Stop();
        }
        MoveAndSlide();
    }

    private void MoveTowardsPlayer(double delta) 
    {
        var direction = Vector3.Zero;
        var transform = Transform;
        var playerDirection = (this.GlobalPosition - player.GlobalPosition);
        direction = playerDirection.Normalized() / 25f;
        Position -= direction * (float)delta * _enemyMoveSpeed;
        //this.GlobalPosition.MoveToward(direction, (float)delta * 25f);

    }

    private void AttackPlayer() 
    {
        //Change attack cooldown to start on animation finished and stop animation on animation finish
        _canAttack = false;
        _lastAttack = ENEMY_ATTACK_CD;
        enemyAnimation.CurrentAnimation = "attack";
        enemyAnimation.Play();
    }

    public void OnBodyEntered3D(Node3D body) 
    {
        if(body is PlayerRE player) 
        {
            GD.Print("Dealing damage to player");
            player.TakeDamage(25);
        }
    }

    private void EnemyAttackCooldownCheck()
    {
        _lastAttack--;
        if (_lastAttack <= 0) 
        {
            _canAttack = true;
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
