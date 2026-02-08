using Godot;
using System;

public partial class SisyphusBoulderInsert : EnvironmentItemRequired
{
    private CsgSphere3D _sphere;
    private Door _doorToUnlock;
    private Node parent;
    public override void _Ready()
    {
        AddToGroup("InteractableEnvironment");
        if (GameStateManager.Instance.IsEventTriggered(ZoneId, EventNameId))
        {
            IsEventTriggered = true;
        }
        _sphere  = GetNode<CsgSphere3D>("CSGSphere3D");
        parent = GetParent();
        _doorToUnlock = parent.GetNode<Door>("ElevatorDoor");
        if (IsEventTriggered)
            _sphere.Visible = true;

    }

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
        
        IsEventTriggered = GameStateManager.Instance.IsEventTriggered(ZoneId, EventNameId);
        if (IsEventTriggered)
        {
            _sphere.Visible = true;
            _doorToUnlock.IsLocked = false;
            if(parent is Zone zone)
                GameStateManager.Instance.MarkDoorUnlocked(zone.ZoneId, _doorToUnlock.DoorId);
            
        }
            
    }
}
