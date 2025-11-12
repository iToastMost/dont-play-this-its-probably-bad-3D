using Godot;
using System;

public abstract partial class ItemBase : StaticBody3D, iLootable
{
    [Export] public string itemName { get; protected set; } = "Unnamed";
    [Export] public string itemDescription { get; protected set; } = "Blank Description";
    [Export] public int itemID { get; protected set; } = 0;
    public void Loot(int[] inventory, int itemID, int inventoryIdx)
    {
        inventory[inventoryIdx] = itemID
        QueueFree();
    }
    public string GetName() => itemName;
    public string GetDescription() => itemDescription;
    
    public int GetID() => itemID;
   
}
