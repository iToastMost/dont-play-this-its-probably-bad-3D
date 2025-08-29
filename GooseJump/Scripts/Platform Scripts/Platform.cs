using Godot;
using System;

public partial class Platform : CharacterBody2D
{

    public override void _Ready()
    {
        AddToGroup("Platforms");
    }
}
