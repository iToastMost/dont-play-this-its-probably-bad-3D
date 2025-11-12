using Godot;
using System;
using System.Text.RegularExpressions;

public partial class GameManager : Node3D
{

	private Ui _ui;
	private PlayerRE _playerRe;
	private Node3D _environment;
	private Node3D _currentEnvironment;
	private Node3D _previousEnvironment;
	private Marker3D _spawnPoint;
	private int[] _playerInventory;
	private Inventory _inventory;
	private Label _healthLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//CallDeferred(nameof(ConnectDialogSignals));
		
		_ui = GetNode<Ui>("UI");
		_inventory = _ui.GetNode<Inventory>("Inventory");
		_playerRe = GetNode<PlayerRE>("3DPlayer");
		_environment = GetNode<Node3D>("Environment");

		//TODO update signals to look like this
		//_playerRe.Connect("UpdateInventoryItems", new Callable(this, nameof(UpdateInventory)));
		//_playerRe.Connect("UpdateHealth", new Callable(this, nameof(UpdatePlayerHealth)));
		_playerRe.UpdateInventoryItems += UpdateInventory;
		_playerRe.UpdateHealth += UpdatePlayerHealth;

		//_inventory.Connect("ItemUsed", new Callable(this, nameof(UseItem)));
		_inventory.ItemUsed += UseItem;
		
		var loadBathroom = ResourceLoader.Load<PackedScene>("res://Scenes/Environments/bathroom_scene.tscn");
		_currentEnvironment = loadBathroom.Instantiate<Node3D>();
		_environment.AddChild(_currentEnvironment);
        //LoadEnvironment("res://Scenes/Environments/bathroom_scene.tscn");

        _previousEnvironment = _currentEnvironment;

        _playerInventory = InventoryManager.GetInstance();

        for (int i = 0; i < _playerInventory.Length; i++)
        {
	        GD.Print(_playerInventory[i]);
        }
        
        _healthLabel = _ui.GetNode<Label>("HealthLabel");
        _healthLabel.Text = _playerRe._health.ToString();
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
		_ui.UpdateText(dialog);
	}

	//Disables player movement from dialog trigger signal
	private void PreventPlayerMovment() 
	{
		_playerRe.DisableMovement();
	}

	private void UpdateInventory(Node3D item)
	{
		//Signals to ui to update inventory node with item looted from the signal emitted from player
		if (item is iLootable loot)
		{
			for (int i = 0; i < _playerInventory.Length; i++)
				if (_playerInventory[i] == -1)
				{
					//_playerInventory[i] = itemID;
					//Queue free doesnt work here? fix this later
					//QueueFree();
					loot.Loot(_playerInventory, loot.GetID(), i);
					_inventory.UpdateInventory(loot.GetName(), i);
					return;
				}
			
		}
	}

	private void UseItem(Button slot, int idx)
	{
		//code for item consumption here
		if (_playerInventory[idx] != null && _playerInventory[idx] >= 0)
		{
			var item = ItemDatabase.GetItem(_playerInventory[idx]).Instantiate<Node3D>();
			
			if (item is iConsumable consumable)
			{
				_playerRe._health += consumable.Consume();
				_playerInventory[idx] = -1;
				_inventory.UpdateInventory("", idx);
				if (_playerRe._health > 100)
				{
					_playerRe._health = 100;
				}
				_healthLabel.Text = _playerRe._health.ToString();
				return;
			}

			if(item is iEquippable equippable)
			{
				//Play around with this code to get it to work, this is kinda pseudo code
				//Maybe send the ID then "equip" that item looping through weapons in the game that are just invisible on the player
				EquipItem(item);
			}
		}
	}

	//Loads a new area
	private void LoadEnvironment(string path) 
	{
		//Removes current environment from GameManagers Environment node
		var environmentChildren = _environment.GetChildren();
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

		if(_currentEnvironment != null) 
		{
			_currentEnvironment = null;
		}

		//Prints the level being loaded to console for debugging
		GD.Print("Loading: " + path);

		//Loads and instantiates new area
		var environmentToLoad = ResourceLoader.Load<PackedScene>(path);
		_currentEnvironment = environmentToLoad.Instantiate<Node3D>();
		
		GD.Print(_currentEnvironment.Name);
		
		_environment.AddChild(_currentEnvironment);
        var spawnPoint = _currentEnvironment.GetNode<Marker3D>("Spawnpoints/From" + _previousEnvironment.Name);

        _previousEnvironment = _currentEnvironment;
        
        _playerRe.GlobalPosition = spawnPoint.GlobalPosition;
        ConnectDoorSignals();
		
    }

	private void UpdatePlayerHealth()
	{
		_healthLabel.Text = _playerRe._health.ToString();
	}

	private void EquipItem(Node3D toEquip)
	{
		_playerRe.EquipItem(toEquip);
	}
	
	private static void MovePlayerToSpawn() 
	{
        
    }
}
