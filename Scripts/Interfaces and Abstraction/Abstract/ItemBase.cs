using Godot;
using System;

public abstract partial class ItemBase : StaticBody3D, iLootable
{
    [Export] public string itemName { get; protected set; } = "Unnamed";
    [Export] public string itemDescription { get; protected set; } = "Blank Description";
    [Export] public int itemID { get; protected set; } = 0;
    public void Loot(int[] inventory, int itemID)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = itemID;
                //Queue free doesnt work here? fix this later
                QueueFree();
            }
            else
            {
                i++;
            }
        }
        
        
        QueueFree();
    }
    public string GetName() => itemName;
    public string GetDescription() => itemDescription;
    
    public int GetID() => itemID;
   
}
