using Godot;
using System;

public abstract class SaveFileManager
{
	public static void SaveGame()
	{
		GD.Print("SaveGame");
	}

	public static void LoadGame()
	{
		GD.Print("LoadGame");
	}
}
