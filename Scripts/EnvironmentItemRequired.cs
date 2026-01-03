using Godot;
using System;

public partial class EnvironmentItemRequired : StaticBody3D, iInteractable
{
    [Signal]
    public delegate void InteractedEventHandler(int itemIdRequired, string zoneId, string eventName);

    [Signal]
    public delegate void AlreadyCompletedTextEventHandler(string alreadyCompletedText);

    [Export] public string EventAlreadyCompletedText;
    
    [Export] public int ItemIdRequired;

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

    public void Interact()
    {
        IsEventTriggered = GameStateManager.Instance.IsEventTriggered(ZoneId, EventNameId);
        if (IsEventTriggered)
        {
            GD.Print("I've already fixed the fusebox");
            EmitSignalAlreadyCompletedText(EventAlreadyCompletedText);
            return;
        }
        GD.Print("Fusebox interacted with!");
        EmitSignalInteracted(ItemIdRequired, ZoneId, EventNameId);
    }
}
