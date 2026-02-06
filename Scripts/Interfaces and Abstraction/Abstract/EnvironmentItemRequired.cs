using Godot;
using System;

public abstract partial class EnvironmentItemRequired : StaticBody3D, iInteractable
{
    [Signal]
    public delegate void InteractedEventHandler(int itemIdRequired, string zoneId, string eventName, string interactText, string eventCompletedText);

    [Signal]
    public delegate void AlreadyCompletedTextEventHandler(bool showDialogueInstantly, string name, string alreadyCompletedText);

    [Export] public string InteractText;

    [Export] public string EventCompletionText;

    [Export] public string EventAlreadyCompletedText;
    
    [Export] public int KeyIdRequired;

    [Export] public string EventNameId;

    [Export] public string ZoneId;

    [Export] public bool IsEventTriggered;

    public override void _Ready()
    {
        AddToGroup("InteractableEnvironment");
        if (GameStateManager.Instance.IsEventTriggered(ZoneId, EventNameId))
        {
            IsEventTriggered = true;
        }
    }

    public virtual void Interact()
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
