using Godot;
using System;

public partial class MovingPlatform : AnimatableBody2D
{
    [Export]
    private Vector2 offSet = new Vector2(405, 0);

    [Export]
    private Vector2 onSet = new Vector2(0, 0);

    [Export]
    private float duration = 5.0f;

    private Tween _initialTween;
    private Tween _loopingTween;

    private float _time = 0f;

    [Export]
    public float SineSpeed = 1f;
    private float SineAmplitude;

    private float platformSpawnOffset = 205;

    private Vector2 _StartPosition;

    private float _tauOffset;

    public override void _PhysicsProcess(double delta)
    {
        Move(delta);
        //GD.Print("Platform Y: ", GlobalPosition.Y);
        //GD.Print("Platform Y: ", GlobalPosition.X);
    }

    public override void _Ready()
    {
        //tauOffset for picking a random point of the sine curve to make platforms seem randomly spawned
        _tauOffset = GD.Randf() * Mathf.Tau;
        //random platform speed
        if(SineSpeed <= 1) 
        {
            SineSpeed = (float)GD.RandRange(0.5, 1.0);
        }
        _StartPosition = Position;

        //checks if the platforms should start by moving left or right
        int roll = GD.RandRange(0, 1);
        if(roll == 0) 
        {
            //offset for platform to not move offscreen. This sets the amplitude to be the same as the spawn so it's symmetrical
            _StartPosition.X = platformSpawnOffset;
            SineAmplitude = platformSpawnOffset;
        }
        else 
        {
            //offset for platform to not move offscreen. This sets the amplitude to be the same as the spawn so it's symmetrical
            _StartPosition.X = platformSpawnOffset;
            SineAmplitude = -platformSpawnOffset;
        }
        AddToGroup("Platforms");
        GD.Print("Platform Y: ", GlobalPosition.Y);
        GD.Print("Platform X: ", GlobalPosition.X);

    }

    //code for actually moving along the sine wave
    private void Move(double delta)
    {
        _time += (float)delta;

        float xOffset = Mathf.Sin(_time * SineSpeed + _tauOffset) * SineAmplitude;

        Position = _StartPosition + new Vector2(xOffset, 0);
    }
}
