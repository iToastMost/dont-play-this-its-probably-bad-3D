using Godot;
using System;

public partial class StoreKeypad : EnvironmentItemRequired
{
    private Dialogue _dialogue;
    private GameManager _gameManager;
    private Door _lockUpDoor;
    public override void _Ready()
    {
        AddToGroup("InteractableEnvironment");
        
        _dialogue = GetNode<Dialogue>("/root/GameManager/PlayerSetup/UI/CanvasLayer/Dialogue");
        _gameManager = GetNode<GameManager>("/root/GameManager");
        _lockUpDoor = GetNode<Door>("/root/GameManager/Environment/ShowroomFloorScene/NavigationRegion3D/Environment/Ace/door");
    }

    public override async void Interact()
    {
        IsEventTriggered = GameStateManager.Instance.IsEventTriggered(ZoneId, EventNameId);
        if (IsEventTriggered)
        {
            GD.Print("I've already fixed the fusebox");
            EmitSignalAlreadyCompletedText(false, "You", EventAlreadyCompletedText);
            return;
        }
        GD.Print("Keypad interacted with!");
        EmitSignalInteracted(KeyIdRequired, ZoneId, EventNameId, InteractText, EventCompletionText);

        await ToSignal(_gameManager, GameManager.SignalName.DialogueCompleted);

        EmitSignal(SignalName.AskToUse);
        //waits for choice selected signal from dialogue 
        
         var choice = await ToSignal(_dialogue, Dialogue.SignalName.ChoiceSelected);
         GD.Print("Answer sent " + choice);
         bool value = choice[0].AsBool();
         if (value)
         {
             GameStateManager.Instance.MarkEventTriggered(ZoneId, EventNameId);
             _lockUpDoor.IsLocked = false;
             GameStateManager.Instance.MarkDoorUnlocked(ZoneId, _lockUpDoor.DoorId);
         }
    }
}
