using Godot;
using System;
using System.IO;

public partial class Door : StaticBody3D, iInteractable
{
	[Export]
	private string environmentToLoad;

	[Export]
	private bool _isFlipped;

	[Signal]
	public delegate void LoadEnvironmentEventHandler(string path);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("Doors");
		if (_isFlipped) 
		{
			var mesh = GetNode<MeshInstance3D>("Cube_013");
			mesh.Scale = new Vector3(-1, 1, 1);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Interact()
    {
		GD.Print("You talked with a door! It doesn't have much to say");
		//GD.Print("Transporting you to: " + environmentToLoad);
		//GetTree().ChangeSceneToFile(environmentToLoad);
		EmitSignal(SignalName.LoadEnvironment, environmentToLoad);
    }
}
