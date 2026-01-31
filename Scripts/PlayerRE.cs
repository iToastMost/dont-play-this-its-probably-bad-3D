using Godot;
using System;

public partial class PlayerRE : CharacterBody3D
{
    [Signal]
    public delegate void UpdateInventoryItemsEventHandler(Node3D item);
    
    [Signal]
    public delegate void AskToLootItemEventHandler(Node3D item);

    [Signal]
    public delegate void UpdateHealthEventHandler();

    [Signal]
    public delegate void UseAmmoEventHandler(int ammo);

    [Signal]
    public delegate void ReloadCheckEventHandler();

    [Signal]
    public delegate void ReloadFinishedEventHandler();

    [Signal]
    public delegate void AcceptDialogueEventHandler();
    
    [Signal]
    public delegate void LootItemSelectedEventHandler();

    [Signal]
    public delegate void NPCDialogueEventHandler(bool showInstant, string npcName, string npcDialogue);

    [Signal]
    public delegate void PlayerDiedEventHandler();

    [Export] public PackedScene GooseJumpScene;

    [Export] public float speed { get; set; } = 1.5f;

    [Export] public float turnSpeed = 4f;
    
    [Export] public float AimSpeed = 2f;

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
    public bool SpinBack = false;

    public CollisionShape3D MeleeCollisionShape;
    public Area3D MeleeCollisionArea;


    public int Ammo = 12;

    public iEquippable HandEquipmentSlot;
    private MeshInstance3D WeaponSkin;

    //private Node3D[] _playerInventory;

    private Vector3 _aimPointDefaultPositon;

    Node GooseScene;
    CharacterBody3D player;
    private CollisionShape3D _playerCollider;
    ColorRect colorRect;
    RayCast3D rayCast;
    public RayCast3D laser;
    public AnimationPlayer playerAnimation;
    public PackedScene bullet;
    public GpuParticles3D MuzzleFlash;
    public OmniLight3D MuzzleFlashOmniLight;
    public SpotLight3D MuzzleFlashSpotLight;
    public Timer MuzzleFlashTimer;
    
    public Node3D Flashlight;
    public Node3D FlashlightPitch;
    public Node3D FlashlightYaw;
    
    public bool IsMeleeAttacking = false;
    
    
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
        _playerCollider = GetNode<CollisionShape3D>("CollisionShape3D");

        _phoneAnimation = GetNode<AnimationPlayer>("SubViewportContainer/AnimationPlayer");
        _subViewport = GetNode<SubViewport>("SubViewportContainer/SubViewport");
        _gooseJumpScene = ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/main.tscn");
        bullet = ResourceLoader.Load<PackedScene>("res://Scenes/Bullet3D.tscn");

        MeleeCollisionShape = GetNode<CollisionShape3D>("CharacterModelAnim/Rig/Skeleton3D/BoneAttachment3D/Boxcutter/Area3D/CollisionShape3D");
        MeleeCollisionArea = GetNode<Area3D>("CharacterModelAnim/Rig/Skeleton3D/BoneAttachment3D/Boxcutter/Area3D");
        MeleeCollisionShape.Disabled = true;
        MeleeCollisionArea.BodyEntered += MeleeCollisionEntered;
        
        MuzzleFlash = GetNode<GpuParticles3D>("MuzzleFlash");
        MuzzleFlashOmniLight = GetNode<OmniLight3D>("MuzzleFlash/OmniLight3D");
        MuzzleFlashSpotLight = GetNode<SpotLight3D>("MuzzleFlash/SpotLight3D");
        MuzzleFlashTimer = GetNode<Timer>("MuzzleFlashTimer");

        MuzzleFlashOmniLight.Visible = false;
        MuzzleFlashSpotLight.Visible = false;

        WeaponSkin = GetNode<MeshInstance3D>("CharacterModelAnim/Rig/Skeleton3D/BoneAttachment3D/Gun");
        WeaponSkin.Visible = false;

        Flashlight = GetNode<Node3D>("CharacterModelAnim/FlashlightYaw/FlashlightPitch/Flashlight");
        FlashlightPitch = GetNode<Node3D>("CharacterModelAnim/FlashlightYaw/FlashlightPitch");
        FlashlightYaw = GetNode<Node3D>("CharacterModelAnim/FlashlightYaw");
        FlashlightYaw.Rotation = new Vector3(0, Mathf.DegToRad(-90), 0);
        
        Flashlight.Visible = false;
        
        
        rayCast = GetNode<RayCast3D>("RayCast3D");
        laser = GetNode<RayCast3D>("Laser");
        aimPoint = GetNode<Node3D>("AimPoint");
        _aimPointDefaultPositon = aimPoint.Position;

        laser.Visible = false;
        
        //_playerInventory = InventoryManager.GetInstance();
        
        playerAnimation.AnimationFinished += OnAnimationFinish;
        MuzzleFlashTimer.Timeout += OnTimerTimeout;

        //Uncomment below code and switch _canMove and _3DStarted to false to start with GooseJump game
        _playerCollider.Disabled = true;
        _canMove = false;
        _3DStarted = false;
        if (GooseScene == null)
        {
        //GooseScene = _gooseJumpScene.Instantiate();
        //GooseScene.Connect("GameStart", new Callable(this, nameof(OnStart)));
        //_subViewport.AddChild(GooseScene);
        }


        //Uncomment below code and switch _canMove and _3DStarted to false to start with GooseJump game

        _playerCollider.Disabled = false;
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
        sm.AddState(PlayerStateTypes.PhoneState, new PhoneState());
        sm.AddState(PlayerStateTypes.Dialog, new DialogState());
        sm.AddState(PlayerStateTypes.HitStun, new PlayerHitStunState());
        sm.AddState(PlayerStateTypes.LootingState, new LootingState());
        
        sm.Initialize(this);
        sm.ChangeState(PlayerStateTypes.Idle);
    }

    public override void _PhysicsProcess(double delta)
    {
        sm.PhysicsUpdate(delta);
        MoveAndSlide();
        
        if (Input.IsActionJustPressed("accept_dialog") && _3DStarted == true) 
        {
            //CanMove = true;
            if (sm.GetPlayerState() == PlayerStateTypes.Dialog)
            {
                 EmitSignal(SignalName.AcceptDialogue);
            }
            
            
            // if (sm.GetPlayerState() == PlayerStateTypes.LootingState)
            // {
            //     EmitSignal(SignalName.LootItemSelected);
            // }
           
        }
        
        if (Input.IsActionJustPressed("interaction_check"))
        {
            InteractCheck();
        }
        
        if (Input.IsActionJustPressed("exit_game")) 
        {
            GetTree().Quit();
        }
        
        if (sm.GetPlayerState() == PlayerStateTypes.Move && Input.IsActionJustPressed("spin_back"))
        {
            //Rotates 180 degrees.
            SpinBack = true;
        }
        
        if(Input.IsActionJustPressed("flashlight_toggle"))
            ToggleFlashlight();
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
    
    public void InteractCheck()
    {
        if (!rayCast.IsColliding())
            return;
        
        var collider = rayCast.GetCollider();
        
        if (collider is Node3D node) 
        {
            if (node is iInteractable interactable) 
            {
                GD.Print("Interactable found");
                playerAnimation.CurrentAnimation = "Interact";
                interactable.Interact();
                return;
            }

            if (node is iLootable)
            {
                //Signals to game manager that an item has been looted and inventory needs to be updated
                //lootable.Loot(_playerInventory);
                //playerAnimation.CurrentAnimation = "PickUp_Table";
                //UpdateInventory(node);
                EmitSignalAskToLootItem(node);
                return;
            }

            if (node is Npc npc)
            {
                EmitSignal(SignalName.NPCDialogue, false, npc.NPCName, npc.NPCDialogue);
            }
        }

        
    }

    private void OnTimerTimeout()
    {
        MuzzleFlashOmniLight.Visible = false;
        MuzzleFlashSpotLight.Visible = false;
    }
    
    public void DisableMovement() 
    {
        CanMove = false;
    }

    private void UpdateInventory(Node3D item)
    {
        EmitSignal(SignalName.UpdateInventoryItems, item);
    }

    public void UpdateAmmo(int ammo)
    {
        EmitSignal(SignalName.UseAmmo, ammo);
    }

    private void PlayerDie() 
    {
        CanMove = false;
        IsDead = true;
        CallDeferred("DisablePlayerCollision");
        sm.ChangeState(PlayerStateTypes.Dead);
    }

    private void DisablePlayerCollision()
    {
        _playerCollider.Disabled = true;
    }
    
    public void PlayAnimation(string  animationName)
    {
        playerAnimation.CurrentAnimation = animationName;
        playerAnimation.Play();
    }
    
    public void OnStart() 
    {
        GD.Print("Start 3D game");
        _3DStarted = true;
        _phoneVisible = false;
        _phoneAnimation.CurrentAnimation = "slide_out";
        _phoneAnimation.Play();
        _playerCollider.Disabled = false;
    }

    public void TakeDamage(int dmg) 
    {
        PlayAnimation("Hit_Chest");
        sm.ChangeState(PlayerStateTypes.HitStun);
        _health -= dmg;
        EmitSignal(SignalName.UpdateHealth);
        
        if (_health <= 0 && !IsDead)
        {
            PlayerDie();
        }
    }

    public void EquipItem(iEquippable weapon)
    {
        HandEquipmentSlot = weapon;
        WeaponSkin.Visible = false;
        if (HandEquipmentSlot != null && HandEquipmentSlot.GetName() == "Handgun")
        {
            WeaponSkin = GetNode<MeshInstance3D>("CharacterModelAnim/Rig/Skeleton3D/BoneAttachment3D/Gun");
        }
        else
        {
            WeaponSkin = GetNode<MeshInstance3D>("CharacterModelAnim/Rig/Skeleton3D/BoneAttachment3D/Boxcutter");
        }
        WeaponSkin.Visible = true;
    }

    public void UnEquipItem(iEquippable item)
    {
        if (HandEquipmentSlot == item)
        {
            HandEquipmentSlot = null;
            WeaponSkin.Visible = false;
        }
    }

    public void UpdateState(PlayerStateTypes state)
    {
        sm.ChangeState(state);
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
            EmitSignal(SignalName.PlayerDied);
        }

        if (animName.Equals("Pistol_Reload")) 
        {
            
            IsReloading = false;
            CanMove = true;
            EmitSignalReloadFinished();
        }

        if (animName.Equals("Hit_Chest"))
        {
            sm.ChangeState(PlayerStateTypes.Idle);
        }

        if (animName.Equals("Sword_Attack"))
        {
            IsMeleeAttacking = false;
            sm.ChangeState(PlayerStateTypes.Aim);
        }

    }
    
    private void ToggleFlashlight()
    {
        Flashlight.Visible = !Flashlight.Visible;
        FlashlightYaw.Rotation = new Vector3(0, Mathf.DegToRad(-90), 0);
        FlashlightPitch.Rotation = new Vector3(0, 0, Mathf.DegToRad(0));
    }

    private void MeleeCollisionEntered(Node3D body)
    {
        GD.Print("Hit!!");
        if (body is Enemy3D enemy)
        {
            enemy.TakeDamage();
        }
    }
}
