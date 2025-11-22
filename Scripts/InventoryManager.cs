using Godot;
using System;

public partial class InventoryManager : Node
{
    private static ItemBase[] _instance;

    public static ItemBase[] GetInstance()
    {
        if (_instance == null)
        {
            _instance = [null, null, null, null, null, null, null, null];
        }
        return _instance;
    }
}
