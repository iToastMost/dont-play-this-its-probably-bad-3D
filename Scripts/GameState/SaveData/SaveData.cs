using Godot;
using System;
using System.Collections.Generic;

public partial class SaveData
{
	public string PlayerPosX { get; set; }
	public string PlayerPosY { get; set; }
	public string PlayerPosZ { get; set; }
	
	public string PlayerRotationY { get; set; }
	public int Playerhealth { get; set; }
	public string CurrentEnvironment { get; set; }
	public int[] playerInventory { get; set; }
	public int equippedItem { get; set; }
	public Dictionary<string, ZoneState> ZoneStates { get; set; }
	
}
