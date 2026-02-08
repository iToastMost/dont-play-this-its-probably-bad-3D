using Godot;
using System;

public partial class StoreKeypad : EnvironmentItemRequired
{
    public override async void Interact()
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

        //await ToSignal(_gameManager, GameManager.SignalName.DialogueCompleted);

        //waits for choice selected signal from dialogue 
        
        // var choice = await ToSignal(_dialogue, Dialogue.SignalName.ChoiceSelected);
        // GD.Print("Answer sent " + choice);
        // bool value = choice[0].AsBool();
        // if (value)
        // {
        //     GameStateManager.Instance.MarkEventTriggered(ZoneId, EventNameId);
        //     _boulderMesh.Visible = false;
        // }
    }
}
