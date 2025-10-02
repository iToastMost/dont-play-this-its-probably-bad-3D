using Godot;
using System;
using System.Text.RegularExpressions;

public partial class GameManager : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void ConnectDialogSignals() 
	{
		var dialogSignals = GetTree().GetNodesInGroup("DialogTriggers");
		var ui = GetNode<Control>("UI");
		foreach(DialogTrigger dTrigger in dialogSignals) 
		{
			
		}
	}
}
