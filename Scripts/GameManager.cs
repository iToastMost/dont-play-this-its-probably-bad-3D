using Godot;
using System;
using System.Text.RegularExpressions;
using Environment = Godot.Environment;

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
	private AnimationPlayer _animationPlayer;
	private Timer _sceneTransitionTimer;
	private Node _playerSetup;
	private MainMenu _mainMenu;
	private Marker3D _playerInitialSpawnPoint;
	private SubViewportContainer _doorPushSubViewportContainer;
	private Camera3D _subviewportCamera;
	
	private string _sceneToLoad = "";
	private bool _isLoadingEnvironment = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitializeMainMenu();
		//NewGame();
    }

	private void NewGame()
	{
		InitializeGame("res://Scenes/Environments/bathroom_scene.tscn");
		CheckStoreLights();
	}

	private void SandboxButtonPressed()
	{
		InitializeGame("res://Scenes/Environments/Sandbox.tscn");
	}

	private void SaveGame()
	{
		GD.Print("Save Game From GameManager");
		SaveFileManager.SaveGame(_playerRe, _playerInventory);
	}
	
	private void LoadGame()
	{
		Vector3 playerSpawnPos;
		
		SaveData loadedData = null;
		loadedData = SaveFileManager.LoadGame();
		
		GD.Print("Loading Save...");
		var playerSetup = ResourceLoader.Load<PackedScene>("res://Scenes/player_setup.tscn");
		_playerSetup = playerSetup.Instantiate<Node>();
		AddChild(_playerSetup);
		
		_mainMenu.QueueFree();
		
		_ui = GetNode<Ui>("PlayerSetup/UI");
		_inventory = _ui.GetNode<Inventory>("Inventory");
		_playerRe = GetNode<PlayerRE>("PlayerSetup/3DPlayer");
		_environment = GetNode<Node3D>("Environment");
		_animationPlayer = GetNode<AnimationPlayer>("ScreenTransitions");
		_sceneTransitionTimer = GetNode<Timer>("SceneTransitionTimer");
		_doorPushSubViewportContainer = GetNode<SubViewportContainer>("CanvasLayer/ScreenFade/DoorPushAnimationViewport");
		_subviewportCamera = GetNode<Camera3D>("CanvasLayer/ScreenFade/DoorPushAnimationViewport/SubViewport/Node3D/Camera3D");
		_doorPushSubViewportContainer.Visible = false;
		
		_playerRe.UpdateInventoryItems += UpdateInventory;
		_playerRe.UpdateHealth += UpdatePlayerHealth;
		_playerRe.UseAmmo += UpdateAmmo;
		_playerRe.ReloadCheck += ReloadCheck;
		_playerRe.ReloadFinished += ReloadFinished;
		_animationPlayer.AnimationFinished += FadeOutFinished;
		_inventory.ItemUsed += UseItem;
		_inventory.CheckItemSlotClicked += CheckInventorySlot;
		
		playerSpawnPos = new Vector3(loadedData.PlayerPosX.ToFloat(),  loadedData.PlayerPosY.ToFloat(), loadedData.PlayerPosZ.ToFloat());
		var playerSpawnRotation = new Vector3(0, loadedData.PlayerRotationY.ToFloat(), 0);
		_playerRe.GlobalRotation = playerSpawnRotation;
		_playerRe.GlobalPosition = playerSpawnPos;
		_playerRe._health = loadedData.Playerhealth;
		_playerRe.Ammo = loadedData.PlayerAmmo;

		_playerInventory = InventoryManager.GetInstance();
		
		for (int i = 0; i < _playerInventory.Length; i++)
		{
			if (loadedData.playerInventory[i] != -1)
			{
				ItemBase item = ItemDatabase.GetItem(loadedData.playerInventory[i]).Instantiate<ItemBase>();
				_playerInventory[i] = item;
				if (item is AmmoItemBase ammo)
				{
					_inventory.UpdateInventory(ammo.GetName() + " (" + ammo.AmmoAmount + ")", i);
				}
				else
				{
					_inventory.UpdateInventory(item.GetName(), i);
				}
				
			}
		}

		if (loadedData.equippedItem != -1)
		{
			ItemBase item = ItemDatabase.GetItem(loadedData.equippedItem).Instantiate<ItemBase>();
			if(item is iEquippable equipment)
				EquipItem(equipment);
		}
		
		GameStateManager.Instance.LoadData(loadedData.ZoneStates);
		
		var loadEnvironment = EnvironmentManager.GetEnvironment(loadedData.CurrentEnvironment);
		_currentEnvironment = loadEnvironment.Instantiate<Node3D>();
		_environment.AddChild(_currentEnvironment);
		
		//_playerInitialSpawnPoint = _currentEnvironment.GetNode<Marker3D>("Spawnpoints/InitialPlayerSpawn");
		//_playerRe.GlobalPosition = _playerInitialSpawnPoint.GlobalPosition;
		//_playerRe.Rotation = _playerInitialSpawnPoint.Rotation;
		
		var zone = _currentEnvironment.GetNode<Zone>(".");
		GameStateManager.Instance.AddZoneState(zone.ZoneId);

		_previousEnvironment = _currentEnvironment;

		for (int i = 0; i < _playerInventory.Length; i++)
		{
			GD.Print(_playerInventory[i]);
		}
        
		_healthLabel = _ui.GetNode<Label>("CanvasLayer/HealthLabel");
		//_healthLabel.Text = "Fine";
		UpdatePlayerHealth();
		_healthLabel.Visible = false;
		
        
		_ammoLabel = _ui.GetNode<Label>("CanvasLayer/AmmoLabel");
		_ammoLabel.Text = "Ammo: " + _playerRe.Ammo;
		_ammoLabel.Visible = false;
		CallDeferred(nameof(ConnectZoneSignals));
		CallDeferred(nameof(ConnectSignals));
		
		
		ConnectDoorSignals();
		ConnectEnvironmentSignals();
		
		CheckStoreLights();
	}

	private void QuitButtonPressed()
	{
		GetTree().Quit();
	}
	
	private void InitializeMainMenu()
	{
		_mainMenu = GetNode<MainMenu>("MainMenu");
		_mainMenu.NewGameButton += NewGame;
		_mainMenu.LoadGameButton += LoadGame;
		_mainMenu.SandboxButton += SandboxButtonPressed;
		_mainMenu.QuitButton += QuitButtonPressed;
	}

	private void InitializeGame(string pathToLoad)
	{
		var playerSetup = ResourceLoader.Load<PackedScene>("res://Scenes/player_setup.tscn");
		_playerSetup = playerSetup.Instantiate<Node>();
		AddChild(_playerSetup);
		
		_mainMenu.QueueFree();
		
		_ui = GetNode<Ui>("PlayerSetup/UI");
		_inventory = _ui.GetNode<Inventory>("Inventory");
		_playerRe = GetNode<PlayerRE>("PlayerSetup/3DPlayer");
		_environment = GetNode<Node3D>("Environment");
		_animationPlayer = GetNode<AnimationPlayer>("ScreenTransitions");
		_sceneTransitionTimer = GetNode<Timer>("SceneTransitionTimer");
		_doorPushSubViewportContainer = GetNode<SubViewportContainer>("CanvasLayer/ScreenFade/DoorPushAnimationViewport");
		_subviewportCamera = GetNode<Camera3D>("CanvasLayer/ScreenFade/DoorPushAnimationViewport/SubViewport/Node3D/Camera3D");
		_doorPushSubViewportContainer.Visible = false;
		
		
		_playerRe.UpdateInventoryItems += UpdateInventory;
		_playerRe.UpdateHealth += UpdatePlayerHealth;
		_playerRe.UseAmmo += UpdateAmmo;
		_playerRe.ReloadCheck += ReloadCheck;
		_playerRe.ReloadFinished += ReloadFinished;
		_animationPlayer.AnimationFinished += FadeOutFinished;
		_inventory.ItemUsed += UseItem;
		_inventory.CheckItemSlotClicked += CheckInventorySlot;
		_playerRe.AcceptDialogue += AcceptDialogue;
		
		var loadGame = ResourceLoader.Load<PackedScene>(pathToLoad);
		_currentEnvironment = loadGame.Instantiate<Node3D>();
		_environment.AddChild(_currentEnvironment);
		
		_playerInitialSpawnPoint = _currentEnvironment.GetNode<Marker3D>("Spawnpoints/InitialPlayerSpawn");
		_playerRe.GlobalPosition = _playerInitialSpawnPoint.GlobalPosition;
		_playerRe.Rotation = _playerInitialSpawnPoint.Rotation;
		
		var zone = _currentEnvironment.GetNode<Zone>(".");
		GameStateManager.Instance.AddZoneState(zone.ZoneId);

		_previousEnvironment = _currentEnvironment;

		_playerInventory = InventoryManager.GetInstance();

		for (int i = 0; i < _playerInventory.Length; i++)
		{
			GD.Print(_playerInventory[i]);
		}
        
		_healthLabel = _ui.GetNode<Label>("CanvasLayer/HealthLabel");
		_healthLabel.Text = "Fine";
		_healthLabel.Visible = false;
        
		_ammoLabel = _ui.GetNode<Label>("CanvasLayer/AmmoLabel");
		_ammoLabel.Text = "Ammo: " + _playerRe.Ammo;
		_ammoLabel.Visible = false;
		CallDeferred(nameof(ConnectZoneSignals));
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

	private void CheckStoreLights()
	{
		if(!GameStateManager.Instance.IsEventTriggered("STORE_BASEMENT", "FUSE_USED"))
		{
			var lights = GetTree().GetNodesInGroup("StoreLights");
			foreach (Node light in lights)
			{
				if (light != null)
				{
					if (light is OmniLight3D omni)
						omni.Visible = false;

					if (light is DirectionalLight3D dir)
						dir.Visible = false;
					
					if(light is SpotLight3D spot)
						spot.Visible = false;
				}
			}
		}
		
		if(GameStateManager.Instance.IsEventTriggered("STORE_BASEMENT", "FUSE_USED"))
		{
			var lights = GetTree().GetNodesInGroup("StoreLights");
			foreach (Node light in lights)
			{
				if (light != null)
				{
					if (light is OmniLight3D omni)
						omni.Visible = true;

					if (light is DirectionalLight3D dir)
						dir.Visible = true;
					
					if(light is SpotLight3D spot)
						spot.Visible = true;
				}
			}
		}
	}
	
	private void ConnectDoorSignals() 
	{
		var doors = GetTree().GetNodesInGroup("Doors");
		foreach(Door door in doors) 
		{
			//door.Connect("LoadEnvironment", new Callable(this, nameof(LoadEnvironment)));
			door.LoadEnvironment += PrepareLoadingEnvironment;
			//door.LoadEnvironment += LoadEnvironment;
			//door.KeyIdCheck += 
		}
	}

	private void ConnectZoneSignals()
	{
		var environments = GetTree().GetNodesInGroup("Environments");
		foreach (Zone zone in environments)
		{
			zone.ZoneEntered += AddZoneState;
		}
	}

	private void ConnectEnvironmentSignals()
	{
		var interactableEnvironment = GetTree().GetNodesInGroup("InteractableEnvironment");
		foreach (Node interactableEnvironmentNode in interactableEnvironment)
		{
			if (interactableEnvironmentNode is EnvironmentItemRequired req)
			{
				req.Interacted += IdCheck;
				req.AlreadyCompletedText += SendDialog;
			}

			if (interactableEnvironmentNode is ComputerTerminal comp)
			{
				comp.GameSaved += SaveGame;
			}
		}
	}


	//Updates dialog from dialog trigger signal
	private void SendDialog(string dialog) 
	{
		//_ui.UpdateText(dialog);
		_playerRe.UpdateState(PlayerStateTypes.Dialog);
		DialogueManager.Instance.ShowDialogue(dialog);
	}

	private void AcceptDialogue()
	{
		DialogueManager.Instance.HideDialogue();
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

	private void PrepareLoadingEnvironment(string path, int keyId, string zoneId, string doorId, bool isLocked)
	{
		if(isLocked)
			if (!CheckForKey(keyId, zoneId, doorId))
				return;
		
		_sceneToLoad = path;

		_subviewportCamera.Current = true;
		_doorPushSubViewportContainer.Visible = true;
		_animationPlayer.CurrentAnimation = "DoorPush";
		_animationPlayer.Play();
	}
	
	//Loads a new area
	private void LoadEnvironment(string path)
	{
		if (_isLoadingEnvironment)
			return;
		
		_isLoadingEnvironment = true;
		
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
		
		var zone = _currentEnvironment.GetNode<Zone>(".");
		GameStateManager.Instance.AddZoneState(zone.ZoneId);
		
		GD.Print(_currentEnvironment.Name);
		
		_environment.AddChild(_currentEnvironment);
        var spawnPoint = _currentEnvironment.GetNode<Marker3D>("Spawnpoints/From" + _previousEnvironment.Name);

        _previousEnvironment = _currentEnvironment;
        
        _playerRe.GlobalPosition = spawnPoint.GlobalPosition;
        _playerRe.Rotation = spawnPoint.Rotation;
        
        _animationPlayer.CurrentAnimation = "ScreenFadeIn";
        _animationPlayer.Play();
        ConnectDoorSignals();
        ConnectEnvironmentSignals();
        
        CheckStoreLights();
    }

	private bool CheckForKey(int keyId, string zoneId, string doorId)
	{
		bool keyCheck = false;
		
		if (keyId != 0)
		{
			for (int i = 0; i < _playerInventory.Length; i++)
			{
				if (_playerInventory[i] is KeyItem keyItem)
				{
					GD.Print("Checking Key Id: " + keyItem.KeyId);
					if (keyItem.KeyId == keyId)
					{
						GD.Print("You have the key to this door, door unlocked");
						keyCheck = true;
						SendDialog("I unlocked the door. Seems like I won't be needing this key anymore.");
						_playerInventory[i] = null;
						_inventory.UpdateInventory("", i);
						GameStateManager.Instance.MarkDoorUnlocked(zoneId, doorId);
						return keyCheck;
					}
				}	
			}
			
			GD.Print("You do not have the key to this door");
			SendDialog("The door is locked. I think I left the key in the break room supply closet.");
			return keyCheck;
		}
		
		return true;
	}
	
	private void IdCheck(int reqId, string zoneId, string eventName)
	{
		
		if (reqId != 0)
		{
			for (int i = 0; i < _playerInventory.Length; i++)
			{
				if (_playerInventory[i] is KeyItem keyItem)
				{
					GD.Print("Checking Key Id: " + keyItem.KeyId);
					if (keyItem.KeyId == reqId)
					{
						GD.Print("Fuse used, power restored.");
						SendDialog("That should have restored power!");
						_playerInventory[i] = null;
						_inventory.UpdateInventory("", i);
						GameStateManager.Instance.MarkEventTriggered(zoneId, eventName);
						
						if(eventName.Equals("FUSE_USED"))
							CheckStoreLights();
						return;
					}
				}	
			}
			
			GD.Print("You do not have a fuse");
			SendDialog("I don't have a fuse. I think there's one in lockup.");
		}
		
	}

	private void AddZoneState(string zoneId)
	{
		GameStateManager.Instance.AddZoneState(zoneId);
		GD.Print(zoneId + " added");
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

	private void FadeOutFinished(StringName animName)
	{
		if (animName.Equals("DoorPush"))
		{
			LoadEnvironment(_sceneToLoad);
		}
		else
		{
			_isLoadingEnvironment = false;
		}
		
	}
}
