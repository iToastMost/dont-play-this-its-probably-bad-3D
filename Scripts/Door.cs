using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

public partial class Door : StaticBody3D, iInteractable
{
	[Export] private string _environmentToLoad;

	[Export] private bool _isFlipped;

	//set the required key needed for this door. 0 = unlocked
	[Export] public int KeyIdRequired { get; set; }

	[Export] public bool IsLocked { get; set; }

	[Export] public string DoorId { get; set; }
	
	[Export] public string ZoneId { get; set; }

	[Signal]
	public delegate void LoadEnvironmentEventHandler(string path, int keyIdRequired, string zoneId, string doorId, bool isLocked);

	[Signal]
	public delegate void KeyIdCheckEventHandler(int id, string path);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("Doors");
		if (GameStateManager.Instance.IsDoorUnlocked(ZoneId, DoorId))
			IsLocked = false;
		
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
		//GD.Print("You talked with a door! It doesn't have much to say");
		//GD.Print("Transporting you to: " + environmentToLoad);
		//GetTree().ChangeSceneToFile(environmentToLoad);
	    
		EmitSignal(SignalName.LoadEnvironment, _environmentToLoad, KeyIdRequired, ZoneId, DoorId, IsLocked);
		
	    //EmitSignalKeyIdCheck(KeyIdRequired, _environmentToLoad);
		
		
    }
}
