using Godot;
using System;

public partial class ItemStorageBox : StaticBody3D, iInteractable
{
    [Signal]
    public delegate void LoadStorageEventHandler();
    public override void _Ready()
    {
        EmitSignal(SignalName.LoadStorage);
    }
    
    public void Interact()
    {
        
    }
}
