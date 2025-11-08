using Godot;
using System;

public partial class InventoryManager : Node
{
    private static Node3D[] _instance;

    public static Node3D[] GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Node3D[8];
        }
        return _instance;
    }
}
