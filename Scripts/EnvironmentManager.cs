using Godot;
using System;
using System.Collections.Generic;

public static class EnvironmentManager
{
    private static Dictionary<string, PackedScene> _environments = new()
    {
        {"STORE_BREAKROOM", ResourceLoader.Load<PackedScene>("res://scenes/Environments/breakroom.tscn")},
        {"STORE_WAREHOUSE", ResourceLoader.Load<PackedScene>("res://scenes/Environments/warehouse.tscn")},
    };

    public static PackedScene GetEnvironment(string environmentId)
    {
        return _environments.ContainsKey(environmentId) ? _environments[environmentId] : null;
    }
}
