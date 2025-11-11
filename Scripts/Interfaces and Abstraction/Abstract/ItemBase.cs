using Godot;
using System;

public abstract partial class ItemBase : StaticBody3D, iLootable
{
    [Export] public string itemName { get; protected set; } = "Unnamed";
    
    [Export] public string itemDescription { get; protected set; } = "Blank Description";
    public void Loot(Node3D[] inventory)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = this;
                QueueFree();
            }
            else
            {
                i++;
            }
        }
        
    }
    public string GetName() => itemName;
    public string GetDescription() => itemDescription;
   
}
