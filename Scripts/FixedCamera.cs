using Godot;
using System;

public partial class FixedCamera : Camera3D
{
	Area3D _area3D;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_area3D = GetNode<Area3D>("Area3D");
		_area3D.BodyEntered += OnAreaEnter;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnAreaEnter(Node3D body)
	{
		if (body is PlayerRE)
		{
			this.Current = true;
		}
		
	}


}
