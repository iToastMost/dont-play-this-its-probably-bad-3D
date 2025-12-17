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
	private ItemBase[] _playerInventory;
	private Inventory _inventory;
	private Label _healthLabel;
	private Label _ammoLabel;
	
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
		_playerRe.UseAmmo += UpdateAmmo;
		_playerRe.ReloadCheck += ReloadCheck;
		_playerRe.ReloadFinished += ReloadFinished;

		//_inventory.Connect("ItemUsed", new Callable(this, nameof(UseItem)));
		_inventory.ItemUsed += UseItem;
		_inventory.CheckItemSlotClicked += CheckInventorySlot;
		
		var loadBathroom = ResourceLoader.Load<PackedScene>("res://Scenes/Environments/bathroom_scene.tscn");
		//var loadBathroom = ResourceLoader.Load<PackedScene>("res://Scenes/Environments/Sandbox.tscn");
		_currentEnvironment = loadBathroom.Instantiate<Node3D>();
		_environment.AddChild(_currentEnvironment);
        //LoadEnvironment("res://Scenes/Environments/bathroom_scene.tscn");

        _previousEnvironment = _currentEnvironment;

        _playerInventory = InventoryManager.GetInstance();

        for (int i = 0; i < _playerInventory.Length; i++)
        {
	        GD.Print(_playerInventory[i]);
        }
        
        _healthLabel = _ui.GetNode<Label>("CanvasLayer/HealthLabel");
        _healthLabel.Text = "Fine";
        
        _ammoLabel = _ui.GetNode<Label>("CanvasLayer/AmmoLabel");
        _ammoLabel.Text = "Ammo: " + _playerRe.Ammo;
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
			dTrigger.Connect("MySignalWithArgument", new Callable(this, nameof(SendDialog)));
		}
	}

	private void ConnectDoorSignals() 
	{
		var doors = GetTree().GetNodesInGroup("Doors");
		foreach(Door door in doors) 
		{
			door.Connect("LoadEnvironment", new Callable(this, nameof(LoadEnvironment)));
			//door.KeyIdCheck += 
		}
	}


	//Updates dialog from dialog trigger signal
	private void SendDialog(string dialog) 
	{
		_ui.UpdateText(dialog);
		_playerRe.UpdateState(PlayerStateTypes.Dialog);
	}
	
	private void UpdateInventory(Node3D item)
	{
		//Signals to ui to update inventory node with item looted from the signal emitted from player
		if (item is iLootable loot)
		{
			//Adds ammo items to inventory, first checking if that ammo is already in the inventory, and if not keeping track of the first open idx
			//to place ammo and not loop over the inventory twice. I'm sure there's a more elegant way to do this
			if (item is AmmoItemBase lootedAmmo)
			{
				int emptyIdx = -1;
				for (int i = 0; i < _playerInventory.Length; i++)
				{
					if (_playerInventory[i] is AmmoItemBase ammo)
					{
						ammo.AmmoAmount += lootedAmmo.AmmoAmount;
						_inventory.UpdateInventory(ammo.GetName() + " (" + ammo.AmmoAmount + ")", i);
						lootedAmmo.QueueFree();

						return;
					} 
					
					if (_playerInventory[i] == null && emptyIdx == -1)
					{
						emptyIdx = i;
					}
					
				}
				
				loot.Loot(_playerInventory, loot.GetID(), emptyIdx);
				_inventory.UpdateInventory(lootedAmmo.GetName() + " (" + lootedAmmo.AmmoAmount + ")", emptyIdx);
				return;
			}
			
			//Adds non ammo items to inventory in first free available space
			for (int i = 0; i < _playerInventory.Length; i++)
				if (_playerInventory[i] == null)
				{
					loot.Loot(_playerInventory, loot.GetID(), i);
					_inventory.UpdateInventory(loot.GetName(), i);
						
					return;
				}
		}
	}

	private void CheckInventorySlot(int idx)
	{
		if (_playerInventory[idx] is ItemBase item)
		{
			if (item == _playerRe.HandEquipmentSlot)
			{
				_inventory.UpdateUseEquipItemButtonText("Unequip");
			}
			else
			{
				_inventory.UpdateUseEquipItemButtonText(item is EquippableItem ? "Equip" : "Use");
			}
			
			_inventory.ItemClicked(idx);
		}
			
	}
	
	private void UseItem(int idx)
	{
		//code for item consumption here
		if (_playerInventory[idx] != null)
		{
			var item = _playerInventory[idx];
			
			if (item is iConsumable consumable)
			{
				if (consumable is HealingItemBase)
				{
					_playerRe._health += consumable.Consume();
					_playerInventory[idx] = null;
					_inventory.UpdateInventory("", idx);
					if (_playerRe._health > 100)
					{
						_playerRe._health = 100;
					}
					UpdatePlayerHealth();
					return;
				}

				if (consumable is AmmoItemBase ammo)
				{
					Reload(ammo, idx);
				}
			}

			if(item is iEquippable equippable)
			{
				//Play around with this code to get it to work, this is kinda pseudocode
				//Maybe send the ID then "equip" that item looping through weapons in the game that are just invisible on the player
				if (equippable == _playerRe.HandEquipmentSlot)
				{
					UnEquipItem(equippable);
				}
				else
				{
					EquipItem(equippable);
				}
				
			}
		}
	}

	//checks inventory for ammo if player hits reload button (not in inventory)
	private void ReloadCheck()
	{
		for (int i = 0; i < _playerInventory.Length; i++)
		{
			if (_playerInventory[i] is AmmoItemBase ammo)
			{
				_playerRe.Reload();
			}
		}
	}

	//TODO Change this code to not loop through for loop again. Reload code is kinda a mess
	private void ReloadFinished()
	{
		for (int i = 0; i < _playerInventory.Length; i++)
		{
			if (_playerInventory[i] is AmmoItemBase ammo)
			{
				Reload(ammo, i);
			}
		}
	}
	
	private void Reload(AmmoItemBase ammo, int idx)
	{
		_playerRe.Ammo += ammo.Consume(_playerRe.Ammo);
		if (ammo.AmmoAmount <= 0)
		{
			_playerInventory[idx] = null;
			_inventory.UpdateInventory("", idx);
		}
		else
		{
			_inventory.UpdateInventory(ammo.GetName() + " (" + ammo.AmmoAmount + ")", idx);
		}
					
					
		if (_playerRe.Ammo > 12)
		{
			_playerRe.Ammo = 12;
		}
		UpdateAmmo(_playerRe.Ammo);
	}

	//Loads a new area
	private void LoadEnvironment(string path, int keyId) 
	{
		CheckForKey(keyId);
		
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

	private void CheckForKey(int keyId)
	{
		if (keyId != 0)
		{
			bool keyCheck = false;

			for (int i = 0; i < _playerInventory.Length; i++)
			{
				if (_playerInventory[i] is KeyItem keyItem)
				{
					GD.Print("Checking Key Id: " + keyItem.KeyId);
					if (keyItem.KeyId == keyId)
					{
						GD.Print("You have the key to this door, door unlocked");
						keyCheck = true;
					}
				}	
			}
			
			if (!keyCheck)
			{
				GD.Print("You do not have the key to this door");
				SendDialog("The door is locked. I think I left the key in the break room supply closet.");
				return;
			}
				
		}
	}

	private void UpdatePlayerHealth()
	{
		string condition = "";
		if (_playerRe._health >= 70)
		{
			condition = "Fine";
		}
		else if (_playerRe._health is < 70 and >= 30)
		{
			condition = "Wounded";
		}
		else
		{
			condition = "Critical";
		}
		_healthLabel.Text = condition;
	}

	private void UpdateAmmo(int ammo)
	{
		_ammoLabel.Text = "Ammo: " + ammo;
	}

	private void EquipItem(iEquippable toEquip)
	{
		_playerRe.EquipItem(toEquip);
	}

	private void UnEquipItem(iEquippable toUnEquip)
	{
		_playerRe.UnEquipItem(toUnEquip);
	}
	
	private static void MovePlayerToSpawn() 
	{
        
    }
}
