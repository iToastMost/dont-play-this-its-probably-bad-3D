using Godot;
using System;

public partial class Interactable : Node
{

    public override void _Ready()
    {
        AddToGroup("Interactable");
    }

    private void Interact() 
    {
        
    }
}
