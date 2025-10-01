using Godot;
using System;

public partial class Door : CsgBox3D, iInteractable
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void Interact()
    {
		GD.Print("You talked with a door! It doesn't have much to say");
    }
}
