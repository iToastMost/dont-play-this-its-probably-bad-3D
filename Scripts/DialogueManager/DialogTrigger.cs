using Godot;
using System;

public partial class DialogTrigger : Area3D
{
	[Export]
	public string dialogText;

	[Export]
	public bool eventTrigger;

    [Signal]
	public delegate void MySignalWithArgumentEventHandler(string text);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		AddToGroup("DialogTriggers");
		BodyEntered += OnAreaEnter;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnAreaEnter(Node3D body)
	{
		GD.Print("Dialog area entered");
        EmitSignal(SignalName.MySignalWithArgument, dialogText);
		QueueFree();
    }
}
