using Godot;
using System;

public partial class ComputerTerminal : StaticBody3D, iInteractable
{
	[Signal]
	public delegate void GameSavedEventHandler();
	public void Interact()
	{
		SaveGame();
	}

	public void SaveGame()
	{
		EmitSignal(SignalName.GameSaved);
	}
}
