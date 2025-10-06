using Godot;
using System;
using System.Text.RegularExpressions;

public partial class GameManager : Node3D
{

	Ui ui;
	PlayerRE player;
	Node3D environment;
	Node3D currentEnvironment;
	Marker3D spawnPoint;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//CallDeferred(nameof(ConnectDialogSignals));
		
		ui = GetNode<Ui>("UI");
		player = GetNode<PlayerRE>("3DPlayer");
		environment = GetNode<Node3D>("Environment");

		var loadBathroom = ResourceLoader.Load<PackedScene>("res://Scenes/Environments/bathroom_scene.tscn");
		currentEnvironment = loadBathroom.Instantiate<Node3D>();
		environment.AddChild(currentEnvironment);
        //LoadEnvironment("res://Scenes/Environments/bathroom_scene.tscn");

		CallDeferred(nameof(ConnectSignals));
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void ConnectSignals() 
	{
		ConnectDialogSignals();
		ConnectDoorSignals();
	}

	private void ConnectDialogSignals() 
	{
		var dialogSignals = GetTree().GetNodesInGroup("DialogTriggers");
		foreach(DialogTrigger dTrigger in dialogSignals) 
		{
			dTrigger.Connect("PreventMovement", new Callable(this, nameof(PreventPlayerMovment)));
			dTrigger.Connect("MySignalWithArgument", new Callable(this, nameof(SendDialog)));
		}
	}

	private void ConnectDoorSignals() 
	{
		var doors = GetTree().GetNodesInGroup("Doors");
		foreach(Door door in doors) 
		{
			door.Connect("LoadEnvironment", new Callable(this, nameof(LoadEnvironment)));
		}
	}


	//Updates dialog from dialog trigger signal
	private void SendDialog(string dialog) 
	{
		ui.UpdateText(dialog);
	}

	//Disables player movement from dialog trigger signal
	private void PreventPlayerMovment() 
	{
		player.DisableMovement();
	}

	//Loads a new area
	private void LoadEnvironment(string path) 
	{
		//Removes current environment from GameManagers Environment node
		var environmentChildren = environment.GetChildren();
		var doorsInGroup = GetTree().GetNodesInGroup("Doors");

		//Remove doors from current scene from the doors group
		foreach(Door door in doorsInGroup) 
		{
			door.RemoveFromGroup("Doors");
		}

		//Gets rid of the environment child so the new one can be added
		foreach(Node3D child in environmentChildren) 
		{
			child.QueueFree();
		}

		if(currentEnvironment != null) 
		{
			currentEnvironment = null;
		}

		//Prints the level being loaded to console for debugging
		GD.Print("Loading: " + path);

		//Loads and instantiates new area
		var environmentToLoad = ResourceLoader.Load<PackedScene>(path);
		currentEnvironment = environmentToLoad.Instantiate<Node3D>();
		
		environment.AddChild(currentEnvironment);
        var spawnPoint = currentEnvironment.GetNode<Marker3D>("PlayerSpawnOnEnter");

        player.GlobalPosition = spawnPoint.GlobalPosition;
        ConnectDoorSignals();
		
    }

	private void MovePlayerToSpawn() 
	{
        
    }
}
