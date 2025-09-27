using Godot;
using System;

public partial class PlayerRE : CharacterBody3D
{
    [Export]
    public PackedScene GooseJumpScene;

    [Export]
    public int Speed { get; set; } = 5;

    [Export]
    public float turnSpeed = 4f;

    private Vector3 _targetVelocity = Vector3.Zero;
    private SubViewport _subViewport;
    private PackedScene _gooseJumpScene;

    private AnimationPlayer _phoneAnimation;
    private bool _phoneVisible = false;
    private bool _canMove = true;
    private bool _3DStarted = false;
    Node GooseScene;
    CharacterBody3D player;
    CollisionShape3D playerCollider;
    ColorRect colorRect;
    public override void _Ready()
    {
        colorRect = GetNode<ColorRect>("SubViewportContainer/CanvasLayer/ColorRect");
        playerCollider = GetNode<CollisionShape3D>("CollisionShape3D");

        _phoneAnimation = GetNode<AnimationPlayer>("SubViewportContainer/AnimationPlayer");
        _subViewport = GetNode<SubViewport>("SubViewportContainer/SubViewport");
        _gooseJumpScene = ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/main.tscn");


        //Uncomment below code and switch _canMove and _3DStarted to false to start with GooseJump game
        playerCollider.Disabled = true;
        _canMove = false;
        _3DStarted = false;
        if (GooseScene == null)
        {
        GooseScene = _gooseJumpScene.Instantiate();
        GooseScene.Connect("GameStart", new Callable(this, nameof(OnStart)));
        _subViewport.AddChild(GooseScene);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_canMove)
        {
            Move(delta);
            MoveAndSlide();
        }
        else 
        {
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

        if (Input.IsActionPressed("walk_forward")) 
        {
            Position += transform.Basis.X * Speed * (float) delta;
        }

        if (Input.IsActionPressed("walk_back")) 
        {
            Position -= transform.Basis.X * Speed / 2 * (float)delta;
        }

        if (Input.IsActionPressed("turn_left")) 
        {
            RotateY(turnSpeed * (float)delta);
        }
        if (Input.IsActionPressed("turn_right"))
        {
            RotateY(-turnSpeed * (float)delta);
        }

        if (Input.IsActionJustPressed("spin_back"))
        {
            RotateY(180 * (float)delta);
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

        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        Velocity = _targetVelocity;
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
