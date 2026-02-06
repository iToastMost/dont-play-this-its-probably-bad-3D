using Godot;
using System;

public partial class SisyphusBoulderInsert : EnvironmentItemRequired
{
    public override void Interact()
    {
        IsEventTriggered = GameStateManager.Instance.IsEventTriggered(ZoneId, EventNameId);
        if (IsEventTriggered)
        {
            GD.Print("I've already fixed the fusebox");
            EmitSignalAlreadyCompletedText(false,"You", EventAlreadyCompletedText);
            return;
        }
        GD.Print("Fusebox interacted with!");
        EmitSignalInteracted(KeyIdRequired, ZoneId, EventNameId, InteractText, EventCompletionText);
    }
}
