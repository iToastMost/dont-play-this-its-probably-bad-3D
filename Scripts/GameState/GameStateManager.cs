using Godot;
using System;
using System.Collections.Generic;

public partial class GameStateManager : Node3D
{
	public static GameStateManager Instance { get; private set; } 
	private string CurrentZoneId;
	private static Dictionary<string, ZoneState> _zoneStates { get; set; } = new();
	

	public override void _Ready()
	{
		Instance = this;
	}
	public void AddZoneState(string zoneId)
	{
		CurrentZoneId = zoneId;
		if (!_zoneStates.ContainsKey(zoneId))
		{
			_zoneStates[zoneId] = new ZoneState();
			GD.Print($"Zone {zoneId} has been added.");
		}
	}
	
	// public ZoneState GetZoneState()
	// {
	// 	if (_zoneStates.ContainsKey(CurrentZoneId))
	// 	{
	// 		return _zoneStates[CurrentZoneId];
	// 	}
	// 	
	// 	return null;
	// }

	public void MarkItemLooted(string zoneId, string itemId)
	{
		if (_zoneStates.TryGetValue(zoneId, out var value))
		{
            value.LootedItemIds.Add(itemId);
		}
		GD.Print($"Item {itemId} has been looted.");
	}

	public bool IsItemLooted(string zoneId, string itemId)
	{
		if (!_zoneStates.TryGetValue(zoneId, out var zone))
		{
			return false;
		}
		
		return zone.LootedItemIds.Contains(itemId);
	}

	public void MarkEnemyKilled(string zoneId, string enemyId)
	{
		_zoneStates[zoneId].DeadEnemyIds.Add(enemyId);
	}

	public bool IsEnemyKilled(string zoneId, string enemyId)
	{
		if (!_zoneStates.TryGetValue(zoneId, out var zone))
		{
			return false;
		}

		return zone.DeadEnemyIds.Contains(enemyId);
	}

	public Dictionary<string, ZoneState> SaveData()
	{
		return _zoneStates;
	}

	public void LoadData(Dictionary<string, ZoneState> zoneStates)
	{
		_zoneStates = zoneStates;
	}

	public void PrintSaveData()
	{
		foreach (var (zoneId, zone) in _zoneStates)
		{
			GD.Print($"Zone {zoneId}: Looted={zone.LootedItemIds.Count} Item={zone.LootedItemIds.Count}");
		}
	}
	
	// public static Dictionary<string, Variant> SaveData()
	// {
	// 	return new Dictionary<string, Variant>()
	// 	{
	// 		{ "ZoneStates", _zoneStates },
	// 	};
	// }
}
