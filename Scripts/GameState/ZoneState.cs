using Godot;
using System;
using System.Collections.Generic;

public class ZoneState
{
	public HashSet<string> DeadEnemyIds { get; set; }= new();
	public HashSet<string> LootedItemIds { get; set; } = new();
	public HashSet<string> EventTriggeredIds { get; set; } = new();
	public HashSet<string> DoorUnlockedIds { get; set; } = new();
}
