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
    Node GooseScene;
    public override void _Ready()
    {
        _phoneAnimation = GetNode<AnimationPlayer>("SubViewportContainer/AnimationPlayer");
        _subViewport = GetNode<SubViewport>("SubViewportContainer/SubViewport");
        _gooseJumpScene = ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/main.tscn");
    }

    public override void _PhysicsProcess(double delta)
    {
        Move(delta);
        MoveAndSlide();
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
        
        if (Input.IsActionJustPressed("open_phone")) 
        {
            if (_phoneVisible == true) 
            {
                //_subViewportContainer.Visible = false;
                _phoneVisible = false;
                _phoneAnimation.CurrentAnimation = "slide_in";
                _phoneAnimation.PlayBackwards();
                
                
                //GooseScene.QueueFree();
            }
            else 
            {
                //_subViewportContainer.Visible = true;
                _phoneVisible = true;
                //if(GooseScene == null) 
                {
                    //GooseScene = _gooseJumpScene.Instantiate();
                    //_subViewport.AddChild(GooseScene);
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
}
