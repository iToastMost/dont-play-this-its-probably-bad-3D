using Godot;
using System;

public partial class ComputerTerminal : StaticBody3D, iInteractable
{
	public void Interact()
	{
		SaveGame();
	}

	public void SaveGame()
	{
		SaveFileManager.SaveGame();
	}
}
