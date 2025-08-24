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
    private SubViewportContainer _subViewportContainer;
    public override void _Ready()
    {
        _subViewportContainer = GetNode<SubViewportContainer>("SubViewportContainer");
    }

    public override void _PhysicsProcess(double delta)
    {
        Move(delta);
        MoveAndSlide();
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
            if (_subViewportContainer.Visible == true) 
            {
                _subViewportContainer.Visible = false;
            }
            else 
            {
                _subViewportContainer.Visible = true;
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
