using Godot;
using System;

public partial class EnvironmentItemRequired : StaticBody3D, iInteractable
{
    [Signal]
    public delegate void InteractedEventHandler(int itemIdRequired);
    
    [Export]
    public int ItemIdRequired;

    public override void _Ready()
    {
        AddToGroup("InteractableEnvironment");
    }

    public void Interact()
    {
        GD.Print("Fusebox interacted with!");
        EmitSignalInteracted(ItemIdRequired);
    }
}
