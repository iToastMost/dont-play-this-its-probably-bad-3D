using Godot;
using System;
using System.Collections.Generic;

public class ZoneState
{
	public HashSet<string> DeadEnemyIds = new();
	public HashSet<string> LootedItemIds = new();
	public HashSet<string> EventTriggeredIds = new();
	public HashSet<string> DoorUnlockedIds = new();
}
