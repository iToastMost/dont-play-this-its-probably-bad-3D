using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FileAccess = Godot.FileAccess;

public abstract class SaveFileManager
{
	public static void SaveGame(PlayerRE player, ItemBase[] playerInventory)
	{
		var PlayerInventory = new int[8];
		for (int i = 0; i < playerInventory.Length; i++)
		{
			if (playerInventory[i] == null)
			{
				//-1 is placeholder value for empty slot, can't be null since it is an array of ints where ItemBase can be null
				//convert -1's to null on load
				//probably better off using strings later
				PlayerInventory[i] = -1;
			}
			else
			{
				PlayerInventory[i] = playerInventory[i].GetID();
			}
				
		}
		SaveData dataToSave = new SaveData();
		dataToSave.PlayerPosX = player.GlobalPosition.X.ToString();
		dataToSave.PlayerPosY = player.GlobalPosition.Y.ToString();
		dataToSave.PlayerPosZ = player.GlobalPosition.Z.ToString();
		dataToSave.PlayerRotationY = player.GlobalRotation.Y.ToString();
		dataToSave.Playerhealth = player._health;
		dataToSave.CurrentEnvironment = GameStateManager.Instance.GetCurrentZoneId();
		dataToSave.playerInventory = PlayerInventory;
		dataToSave.PlayerAmmo = player.Ammo;
		
		if(player.HandEquipmentSlot != null)
			dataToSave.equippedItem = player.HandEquipmentSlot.GetID();
		if(player.HandEquipmentSlot == null)
			dataToSave.equippedItem = -1;
		
		dataToSave.ZoneStates = GameStateManager.Instance.SaveData();
		
		//var saveFile = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
		var saveFile = "savegame.json";
		//var saveData = GameStateManager.Instance.SaveData();
		var options = new JsonSerializerOptions {WriteIndented = true};
		var jsonString = JsonSerializer.Serialize(dataToSave, options);
		File.WriteAllText(saveFile, jsonString);
		
		GD.Print("SaveGame");
		GD.Print(File.ReadAllText(saveFile));
	}

	public static SaveData LoadGame()
	{
		var loadFile = "savegame.json";
		string jsonString = File.ReadAllText(loadFile);
		SaveData loadedData = null;
		loadedData = JsonSerializer.Deserialize<SaveData>(jsonString);
		//GameStateManager.Instance.LoadData(loadedData.ZoneStates);
		
		GD.Print("LoadGame");
		return loadedData;
	}
	
}
