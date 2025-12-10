using Godot;
using System;
using Godot.Collections;

public static class ItemDatabase
{
	private static Dictionary<int, PackedScene> items = new()
	{
		{0, ResourceLoader.Load<PackedScene>("res://Scenes/Items/healing_herb.tscn")},
		{1, ResourceLoader.Load<PackedScene>("res://Scenes/Items/handgun_ammo.tscn") },
		{2, ResourceLoader.Load<PackedScene>("res://Scenes/Items/Handgun.tscn") },
	};

	public static PackedScene GetItem(int key)
	{
		return items.TryGetValue(key, out var item) ? item : null; 
	} 
}
