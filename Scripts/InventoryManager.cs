using Godot;
using System;

public partial class InventoryManager : Node
{
    private static int[] _instance;

    public static int[] GetInstance()
    {
        if (_instance == null)
        {
            _instance = [-1, -1, -1, -1, -1, -1, -1, -1];
        }
        return _instance;
    }
}
