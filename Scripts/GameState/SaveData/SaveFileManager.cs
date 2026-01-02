using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FileAccess = Godot.FileAccess;

public abstract class SaveFileManager
{
	public static void SaveGame()
	{
		
		
		//var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
		var saveFile = "savegame.json";
		var saveData = GameStateManager.Instance.SaveData();
		var options = new JsonSerializerOptions {WriteIndented = true};
		var jsonString = JsonSerializer.Serialize(saveData, options);
		File.WriteAllText(saveFile, jsonString);
		
		GD.Print("SaveGame");
		GD.Print(File.ReadAllText(saveFile));
	}

	public static void LoadGame()
	{
		var loadFile = "savegame.json";
		GameStateManager.Instance.LoadData(JsonSerializer.Deserialize<Dictionary<string, ZoneState>>(File.ReadAllText(loadFile)));
		
		GD.Print("LoadGame");
	}
	
}
