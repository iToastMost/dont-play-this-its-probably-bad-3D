using Godot;
using System;

public partial class JitterPlatform : AnimatableBody2D
{
	[Export]
	public float sineAmplitudeX = 4f;

	[Export]
	public float sineSpeedX = 25f;

    [Export]
    public float sineAmplitudeY = 4f;

    [Export]
    public float sineSpeedY = 25f;

    private float _time = 0;

	private Vector2 _startPosition;

	private float _jitterX;
	private float _jitterY;

	private float _moveToNewPositionSpeed = 1000f;
	private bool _jitterPositionReached = true;

	private Vector2 _jitterPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_startPosition = Position;
        AddToGroup("Platforms");
		AddToGroup("JitterPlatforms");
		_jitterPosition = Position;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (_jitterPositionReached) 
		{
            Move(delta);
        }
		else 
		{
            MoveToPosition(delta);
        }
		
		
	}

	private void Move(double delta) 
	{
		_time += (float)delta;

		_jitterX = Mathf.Sin(_time * sineSpeedX) * sineAmplitudeX;
		_jitterY = Mathf.Sin(_time * sineSpeedY) * sineAmplitudeY;

		Position = _jitterPosition + new Vector2(_jitterX, _jitterY);
	}

	private void MoveToPosition(double delta) 
	{
		//_time += (float)delta;
		//_jitterX = Mathf.Sin(_time * sineSpeedX) * sineAmplitudeX;
		//Position = _startPosition + new Vector2(_jitterX, Position.Y);
		if(Position.DistanceTo(_jitterPosition) < 20) 
		{
			_jitterPositionReached = true;
		}
		else 
		{
            Position += Position.DirectionTo(_jitterPosition) * _moveToNewPositionSpeed * (float)delta;
        }
		
	}

	public void MoveAllPlatforms() 
	{
		var jitterPlatforms = GetTree().GetNodesInGroup("JitterPlatforms");

		foreach(JitterPlatform jitterPlatform in jitterPlatforms) 
		{
			jitterPlatform._jitterPositionReached = false;
			//jitterPlatform.Position.X = new Vector2(GD.RandRange(0, 405), 0);
			jitterPlatform._jitterPosition = new Vector2(GD.RandRange(0, 405), jitterPlatform.Position.Y);
			//GD.Print("New jitterPosition = " + _jitterPosition.X);
		}
	}
}
