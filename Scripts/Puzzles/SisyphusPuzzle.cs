using Godot;
using System;

public partial class SisyphusPuzzle : StaticBody3D, iInteractable
{
	[Signal]
	public delegate void InteractedEventHandler(string itemIdRequired, string zoneId, string eventName, string interactText, string eventCompletedText);

	[Signal]
	public delegate void AlreadyCompletedTextEventHandler(bool showInstant, string name, string alreadyCompletedText);
	
	[Signal]
	public delegate void AskToLootItemEventHandler(Node3D item);

	[Export] public string InteractText;

	[Export] public string EventCompletionText;

	[Export] public string EventAlreadyCompletedText;
    
	[Export] public string KeyIdRequired;

	[Export] public string EventNameId;

	[Export] public string ZoneId;

	[Export] public bool IsEventTriggered;
	
	private PlayerRE _player;
	private Node3D _sisyphusBoulderItem;
	private Dialogue _dialogue;
	private GameManager _gameManager;
	private CsgSphere3D _boulderMesh;

	public override void _Ready()
	{
		AddToGroup("InteractableEnvironment");
		
		_player = GetNode<PlayerRE>("/root/GameManager/PlayerSetup/3DPlayer");
		_dialogue = GetNode<Dialogue>("/root/GameManager/PlayerSetup/UI/CanvasLayer/Dialogue");
		_gameManager = GetNode<GameManager>("/root/GameManager");
		_boulderMesh = GetNode<CsgSphere3D>("CSGBox3D/CSGSphere3D");
		
		ItemBase item = ItemDatabase.GetItem(6).Instantiate<ItemBase>();
		_sisyphusBoulderItem = item;
		
		if (GameStateManager.Instance.IsEventTriggered(ZoneId, EventNameId))
		{
			IsEventTriggered = true;
			_boulderMesh.Visible = false;
		}
	}

	public async void Interact()
	{
		IsEventTriggered = GameStateManager.Instance.IsEventTriggered(ZoneId, EventNameId);
		if (IsEventTriggered)
		{
			GD.Print("I've already fixed the fusebox");
			EmitSignalAlreadyCompletedText(false, "You", EventAlreadyCompletedText);
			return;
		}
		GD.Print("Fusebox interacted with!");
		EmitSignalInteracted(KeyIdRequired, ZoneId, EventNameId, InteractText, EventCompletionText);

		await ToSignal(_gameManager, GameManager.SignalName.DialogueCompleted);
		
		EmitSignalAskToLootItem(_sisyphusBoulderItem);

		//waits for choice selected signal from dialogue 
		var choice = await ToSignal(_dialogue, Dialogue.SignalName.ChoiceSelected);
		GD.Print("Answer sent " + choice);
		bool value = choice[0].AsBool();
		if (value)
		{
			GameStateManager.Instance.MarkEventTriggered(ZoneId, EventNameId);
			_boulderMesh.Visible = false;
		}
			

	}
}
