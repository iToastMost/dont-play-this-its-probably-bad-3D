using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

public partial class Door : StaticBody3D, iInteractable
{
	[Export] private string _environmentToLoad;

	[Export] private bool _isFlipped;

	//set the required key needed for this door. 0 = unlocked
	[Export] public string KeyIdRequired { get; set; }

	[Export] public bool IsLocked { get; set; }

	[Export] public string failedCheckText { get; set; }
	
	[Export] public string DoorId { get; set; }
	
	[Export] public string ZoneId { get; set; }

	[Signal]
	public delegate void LoadEnvironmentEventHandler(string path, string keyIdRequired, string zoneId, string doorId, string failedKeyCheckText, bool isLocked);

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
	    //TODO make bool check to see if door is locked. If door is not locked send load environment signal.
	    //TODO make a signal for if door is locked to check if player has/used the key. Supah smart
		//GD.Print("You talked with a door! It doesn't have much to say");
		//GD.Print("Transporting you to: " + environmentToLoad);
		//GetTree().ChangeSceneToFile(environmentToLoad);
		EmitSignal(SignalName.LoadEnvironment, _environmentToLoad, KeyIdRequired, ZoneId, DoorId, failedCheckText, IsLocked);
		
	    //EmitSignalKeyIdCheck(KeyIdRequired, _environmentToLoad);
		
		
    }
}
