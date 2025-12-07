using Godot;
using System;

public partial class PlayerRE : CharacterBody3D
{
    [Signal] 
    public delegate void UpdateInventoryItemsEventHandler(Node3D item);

    [Signal]
    public delegate void UpdateHealthEventHandler();

    [Signal]
    public delegate void UseAmmoEventHandler(int ammo);
    
    [Signal]
    public delegate void ReloadCheckEventHandler();

    [Signal]
    public delegate void ReloadFinishedEventHandler();
    
    [Export]
    public PackedScene GooseJumpScene;

    [Export]
    public float speed { get; set; } = 1.5f;

    [Export]
    public float turnSpeed = 4f;

    public int _health = 100;

    public Vector3 _targetVelocity = Vector3.Zero;
    private SubViewport _subViewport;
    private PackedScene _gooseJumpScene;

    //bools without state machine
    private AnimationPlayer _phoneAnimation;
    private bool _phoneVisible = false;
    private bool _canMove = true;
    private bool _3DStarted = false;
    private bool _isAiming = false;
    private bool _isDead = false;
    private bool _isReloading = false;
    
    //bools for state machine
    //private AnimationPlayer _phoneAnimation;
    //private bool _phoneVisible = false;
    public bool CanMove = true;
    //private bool 3DStarted = false;
    public bool IsAiming = false;
    public bool IsDead = false;
    public bool IsReloading = false;
    
    
    public int Ammo = 12;

    private Node3D _handEquipmentSlot;

    //private Node3D[] _playerInventory;

    private Vector3 _aimPointDefaultPositon;
   
    Node GooseScene;
    CharacterBody3D player;
    CollisionShape3D playerCollider;
    ColorRect colorRect;
    RayCast3D rayCast;
    public RayCast3D laser;
    public AnimationPlayer playerAnimation;
    PackedScene bullet;
    Node3D aimPoint;
    private StateMachine sm;

    public bool MovementInput() => Input.IsActionPressed("walk_forward")
                                   || Input.IsActionPressed("walk_back")
                                   || Input.IsActionPressed("turn_left")
                                   || Input.IsActionPressed("turn_right")
                                   || Input.IsActionJustPressed("spin_back");

    public bool AimInput() => Input.IsActionPressed("aim");

    public bool ReloadInput() => Input.IsActionPressed("reload");
    


    public override void _Ready()
    {
        //player = GetNode<CharacterBody3D>("3DPlayer");
        playerAnimation = GetNode<AnimationPlayer>("CharacterModelAnim/AnimationPlayer");

        colorRect = GetNode<ColorRect>("SubViewportContainer/CanvasLayer/ColorRect");
        playerCollider = GetNode<CollisionShape3D>("CollisionShape3D");

        _phoneAnimation = GetNode<AnimationPlayer>("SubViewportContainer/AnimationPlayer");
        _subViewport = GetNode<SubViewport>("SubViewportContainer/SubViewport");
        _gooseJumpScene = ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/main.tscn");
        bullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullet3D.tscn");

        rayCast = GetNode<RayCast3D>("RayCast3D");
        laser = GetNode<RayCast3D>("Laser");
        aimPoint = GetNode<Node3D>("AimPoint");
        _aimPointDefaultPositon = aimPoint.Position;

        laser.Visible = false;
        
        //_playerInventory = InventoryManager.GetInstance();
        
        playerAnimation.AnimationFinished += OnAnimationFinish;       

        //Uncomment below code and switch _canMove and _3DStarted to false to start with GooseJump game
        playerCollider.Disabled = true;
        _canMove = false;
        _3DStarted = false;
        if (GooseScene == null)
        {
        //GooseScene = _gooseJumpScene.Instantiate();
        //GooseScene.Connect("GameStart", new Callable(this, nameof(OnStart)));
        //_subViewport.AddChild(GooseScene);
        }


        //Uncomment below code and switch _canMove and _3DStarted to false to start with GooseJump game

        playerCollider.Disabled = false;
        _canMove = true;
        _3DStarted = true;
        //if (GooseScene == null)
        //{
        //GooseScene = _gooseJumpScene.Instantiate();
        //GooseScene.Connect("GameStart", new Callable(this, nameof(OnStart)));
        //_subViewport.AddChild(GooseScene);
        //}
        
        sm = new StateMachine();
        
        sm.AddState(PlayerStateTypes.Idle, new IdleState());
        sm.AddState(PlayerStateTypes.Move, new MoveState());
        sm.AddState(PlayerStateTypes.Aim, new AimState());
        sm.AddState(PlayerStateTypes.Reload, new ReloadState());
        sm.AddState(PlayerStateTypes.Dead, new DeathState());
        
        sm.Initialize(this);
        sm.ChangeState(PlayerStateTypes.Idle);
    }

    public override void _PhysicsProcess(double delta)
    {
        sm.PhysicsUpdate(delta);
        MoveAndSlide();

        if (_health <= 0 && !IsDead)
        {
            PlayerDie();
        }
        
        if (Input.IsActionJustPressed("accept_dialog") && _3DStarted == true) 
        {
            CanMove = true;
        }
        
        if (Input.IsActionJustPressed("interaction_check"))
        {
            InteractCheck();
        }
        
        if (Input.IsActionJustPressed("exit_game")) 
        {
            GetTree().Quit();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (_phoneVisible) 
        {
            _subViewport?.PushInput(@event);
            GetViewport().SetInputAsHandled();
        }

        if(_isAiming && @event is InputEventMouseMotion movement) 
        {
            //Vector3 motion = new Vector3(movement.Relative.X, movement.Relative.Y, 0);
            //aimPoint.Translate(motion);
            //laser.Rotate(Vector3.Up, -Mathf.DegToRad(motion.X) * turnSpeed);
            //laser.Rotate(Vector3.Right, -Mathf.DegToRad(motion.Y) * turnSpeed);
            //float y = Mathf.Clamp(laser.Rotation.Y, Mathf.DegToRad(-45), Mathf.DegToRad(45));
            //float z = Mathf.Clamp(laser.Rotation.Z, Mathf.DegToRad(80), Mathf.DegToRad(100));
            //laser.Rotation = new Vector3(0,y,z);
        }
    }

    private void PhoneInteract() 
    {
        if (Input.IsActionJustPressed("open_phone")) 
        {
            if (_phoneVisible == true) 
            {
                //_subViewportContainer.Visible = false;
                _phoneVisible = false;
                _phoneAnimation.CurrentAnimation = "slide_out";
                _phoneAnimation.Play();
                
                
                //GooseScene.QueueFree();
                //GooseScene = null;
            }
            else 
            {
                //_subViewportContainer.Visible = true;
                _phoneVisible = true;
                if(GooseScene == null) 
                {
                    GooseScene = _gooseJumpScene.Instantiate();
                    _subViewport.AddChild(GooseScene);
                    //GooseScene.Connect("GameStart", new Callable(this, nameof(OnStart)));
                }
                //GooseScene = _gooseJumpScene.Instantiate();
                //_subViewport.AddChild(GooseScene);
                _phoneAnimation.CurrentAnimation = "slide_in";
                _phoneAnimation.Play();
            }
        }
    }
    
    private void Aim() 
    {
        playerAnimation.CurrentAnimation = "Pistol_Idle";
        laser.Visible = true;
    }

    public void SendReloadCheck()
    {
        EmitSignalReloadCheck();
    }
    
    public void Reload()
    {
        playerAnimation.CurrentAnimation = "Pistol_Reload";
        CanMove = false;
        IsReloading = true;
    }

    public void Shoot() 
    {
        if(Ammo > 0) 
        {
            Ammo--;

            Vector3 direction;
            
            var bulletSpawn = bullet.Instantiate<Bullet3D>();
            
            if (laser.IsColliding())
            {
                direction = (laser.GlobalPosition - laser.GetCollisionPoint()).Normalized();
                bulletSpawn.LookAtFromPosition(laser.GlobalPosition, laser.GetCollisionPoint());
            }
            else
            {
                direction = (laser.GlobalPosition - laser.TargetPosition).Normalized();
                var globalTargetPosition = laser.ToGlobal(laser.TargetPosition);
                bulletSpawn.LookAtFromPosition(laser.GlobalPosition, globalTargetPosition);
            }
            bulletSpawn.SetDireciton(-direction);
            GetParent().AddChild(bulletSpawn);
            UpdateAmmo(Ammo);
            GD.Print("Pew pew");
        }

    }

    private void InteractCheck()
    {
        if (!rayCast.IsColliding())
            return;
        
        var collider = rayCast.GetCollider();
        
        if (collider is Node3D node) 
        {
            if (node is iInteractable interactable) 
            {
                GD.Print("Interactable found");
                interactable.Interact();
                return;
            }

            if (node is iLootable)
            {
                //Signals to game manager that an item has been looted and inventory needs to be updated
                //lootable.Loot(_playerInventory);
                UpdateInventory(node);
                return;
            }
        }

        
    }

    public void DisableMovement() 
    {
        CanMove = false;
    }

    private void UpdateInventory(Node3D item)
    {
        EmitSignal(SignalName.UpdateInventoryItems, item);
    }

    private void UpdateAmmo(int ammo)
    {
        EmitSignal(SignalName.UseAmmo, ammo);
    }

    private void PlayerDie() 
    {
        CanMove = false;
        IsDead = true;
    }

    public void PlayAnimation(string  animationName)
    {
        playerAnimation.CurrentAnimation = animationName;
        playerAnimation.Play();
    }

    public void AimLaser(double delta)
    {
            if (Input.IsActionPressed("aim_right"))
            {
                laser.RotateY(-turnSpeed /2 * (float)delta);
            }
                    
            if (Input.IsActionPressed("aim_left"))
            {
                laser.RotateY(turnSpeed/2 * (float)delta);
            }
                    
            if (Input.IsActionPressed("aim_up"))
            {
                laser.RotateZ(turnSpeed/2 * (float)delta);
            }
                    
            if (Input.IsActionPressed("aim_down"))
            {
                laser.RotateZ(-turnSpeed/2 * (float)delta);
            }
                    
            float y = Mathf.Clamp(laser.Rotation.Y, Mathf.DegToRad(-45), Mathf.DegToRad(45));
            float z = Mathf.Clamp(laser.Rotation.Z, Mathf.DegToRad(80), Mathf.DegToRad(100));
            laser.Rotation = new Vector3(0,y,z);
    }

    public void OnStart() 
    {
        GD.Print("Start 3D game");
        _3DStarted = true;
        _phoneVisible = false;
        _phoneAnimation.CurrentAnimation = "slide_out";
        _phoneAnimation.Play();
        playerCollider.Disabled = false;
    }

    public void TakeDamage(int dmg) 
    {
        _health -= dmg;
        EmitSignal(SignalName.UpdateHealth);
    }

    public void EquipItem(Node3D item)
    {
        _handEquipmentSlot = item;
        //item.Visible = true;
    }

    public void OnAnimationFinish(StringName animName) 
    {
        GD.Print("animation finished");
        if (animName.Equals("slide_out")) 
        {
            GD.Print("anim finished");
            GooseScene.QueueFree();
            GooseScene = null;
            GetTree().Paused = false;
            colorRect.Visible = false;
        }

        if (animName.Equals("Death01")) 
        {
            playerAnimation.Stop();
        }

        if (animName.Equals("Pistol_Reload")) 
        {
            
            IsReloading = false;
            CanMove = true;
            EmitSignalReloadFinished();
        }
        
    }
}
