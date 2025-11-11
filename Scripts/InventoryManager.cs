using Godot;
using System;

public partial class InventoryManager : Node
{
    private static int[] _instance;

    public static int[] GetInstance()
    {
        if (_instance == null)
        {
            _instance = new int[8];
        }
        return _instance;
    }
}
