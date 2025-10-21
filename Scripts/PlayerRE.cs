using Godot;
using System;

public partial class PlayerRE : CharacterBody3D
{
    [Export]
    public PackedScene GooseJumpScene;

    [Export]
    public float speed { get; set; } = 1.5f;

    [Export]
    public float turnSpeed = 4f;

    private int _health = 100;

    private Vector3 _targetVelocity = Vector3.Zero;
    private SubViewport _subViewport;
    private PackedScene _gooseJumpScene;

    private AnimationPlayer _phoneAnimation;
    private bool _phoneVisible = false;
    private bool _canMove = true;
    private bool _3DStarted = false;
    private bool _isAiming = false;
    private bool _isDead = false;
   
    Node GooseScene;
    CharacterBody3D player;
    CollisionShape3D playerCollider;
    ColorRect colorRect;
    RayCast3D rayCast;
    AnimationPlayer playerAnimation;
    PackedScene bullet;

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
        playerAnimation.CurrentAnimation = "Idle";
        playerAnimation.Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        if(_health <= 0) 
        {
            _isDead = true;
        }

        if (_canMove && !_isDead)
        {
            if (Input.IsAnythingPressed()) 
            {
                Move(delta);
            }
            else 
            {
                playerAnimation.CurrentAnimation = "Idle";
            }
            

            if (Input.IsActionJustPressed("interaction_check"))
            {
                InteractCheck();
            }

            MoveAndSlide();
        }
        else 
        {
            playerAnimation.CurrentAnimation = "Idle";
            if (Input.IsActionJustPressed("accept_dialog") && _3DStarted == true) 
            {
                _canMove = true;
            }
        }
        
    }

    public override void _Input(InputEvent @event)
    {
        if (_phoneVisible) 
        {
            _subViewport?.PushInput(@event);
            GetViewport().SetInputAsHandled();
        }
    }

    private void Move(double delta) 
    {
        var direction = Vector3.Zero;
        var transform = Transform;

        if (Input.IsActionPressed("aim"))
        {
            _isAiming = true;
            Aim();
            if (_isAiming && Input.IsActionJustPressed("shoot_3d")) 
            {
                Shoot();
            }
        }
        else
        {
            _isAiming = false;
        }

        if (Input.IsActionPressed("walk_forward") && !_isAiming) 
        {
            if (Input.IsActionPressed("sprint")) 
            {
                Position += transform.Basis.X * (speed * 2) * (float)delta;
                playerAnimation.CurrentAnimation = "Jog_Fwd";
            }
            else 
            {
                Position += transform.Basis.X * speed * (float)delta;
                playerAnimation.CurrentAnimation = "Walk";
            }
        }

        if (Input.IsActionPressed("walk_back") && !_isAiming) 
        {
            Position -= transform.Basis.X * speed / 2 * (float)delta;
        }

        if (Input.IsActionPressed("turn_left")) 
        {
            RotateY(turnSpeed * (float)delta);
        }
        if (Input.IsActionPressed("turn_right"))
        {
            RotateY(-turnSpeed * (float)delta);
        }

        if (Input.IsActionJustPressed("spin_back") && !_isAiming)
        {
            //Rotates 180 degrees.
            RotateY(Mathf.Pi);
        }

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

        if (Input.IsActionJustPressed("exit_game")) 
        {
            GetTree().Quit();
        }


        if(direction != Vector3.Zero) 
        {
            direction = direction.Normalized();
            GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
        }

        _targetVelocity.X = direction.X * speed;
        _targetVelocity.Z = direction.Z * speed;

        Velocity = _targetVelocity;
    }

    private void Aim() 
    {
        playerAnimation.CurrentAnimation = "Pistol_Idle";
    }

    private void Shoot() 
    {
        var bulletSpawn = bullet.Instantiate<Bullet3D>();
        bulletSpawn.Position = GetNode<RayCast3D>("Laser").GlobalPosition;
        bulletSpawn.SetDireciton(this.Transform.Basis.X);
        GetParent().AddChild(bulletSpawn);
        GD.Print("Pew pew");

    }

    private void InteractCheck() 
    {
        if (rayCast.IsColliding()) 
        {
            var collision = rayCast.GetCollider();

            if (collision is iInteractable interactable) 
            {
                GD.Print("Interactable found");
                interactable.Interact();
            }
        }
    }

    public void DisableMovement() 
    {
        _canMove = false;
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

    public void AnimationFinished(StringName animName) 
    {
        if (animName.Equals("slide_out")) 
        {
            GD.Print("anim finished");
            GooseScene.QueueFree();
            GooseScene = null;
            GetTree().Paused = false;
            colorRect.Visible = false;
        }
        
    }
}
