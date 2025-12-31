using Godot;
using System;

public abstract partial class ItemBase : StaticBody3D, iLootable
{
    [Export] public string itemName { get; protected set; } = "Unnamed";
    [Export] public string itemDescription { get; protected set; } = "Blank Description";
    [Export] public int itemID { get; protected set; } = 0;
    [Export] public string ZoneId {get; set;}
    [Export] public string ItemId {get; set;}
    
    public override void _Ready()
    {
        if (GameStateManager.Instance == null)
            return;
        
        if(GameStateManager.Instance.IsItemLooted(ZoneId, ItemId))
        {
            QueueFree();
        }
    }
    public void Loot(ItemBase[] inventory, int itemID, int inventoryIdx)
    {
        ItemBase item = ItemDatabase.GetItem(itemID).Instantiate<ItemBase>();
        inventory[inventoryIdx] = item;
        GameStateManager.Instance.MarkItemLooted(ZoneId, ItemId);
        QueueFree();
    }
    public string GetName() => itemName;
    public string GetDescription() => itemDescription;
    
    public int GetID() => itemID;
   
}
