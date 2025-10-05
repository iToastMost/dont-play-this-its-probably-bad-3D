using Godot;
using System;
using System.Text.RegularExpressions;

public partial class GameManager : Node3D
{
	[Signal]
	public delegate void ShowDialogEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Area3D dialogTrigger1 = GetNode<Area3D>("DialogTrigger");
		dialogTrigger1.Connect("PreventMovement", new Callable(this, nameof(SendDialogSignal)));
		//dialogTrigger1.PreventMovement += () => SendDialogSignal();
		
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

	private void SendDialogSignal() 
	{
		GD.Print("Signal sent to uI");
		EmitSignal(SignalName.ShowDialog);
	}
}
