using Godot;
using System;

public partial class FixedCamera : Camera3D
{
	Area3D _area3D;
	private Camera3D _previousCamera3D;
	private Camera3D _currentCamera3D;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_area3D = GetNode<Area3D>("Area3D");
		_area3D.BodyEntered += OnAreaEnter;
		//_area3D.BodyExited += OnExitArea;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnAreaEnter(Node3D body)
	{
		if (body is PlayerRE)
		{
			//_previousCamera3D = GetViewport().GetCamera3D();
			Current = true;
			//_currentCamera3D = GetViewport().GetCamera3D();
		}
	}

	private void OnExitArea(Node3D body)
	{
		if (body is PlayerRE)
		{
			if (_previousCamera3D != null && _previousCamera3D == _currentCamera3D)
			{
				_previousCamera3D.Current = true;
			}
		}
	}


}
