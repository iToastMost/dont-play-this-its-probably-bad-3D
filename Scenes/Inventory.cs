using Godot;
using System;

public partial class Inventory : Control
{
    [Signal] public delegate void ItemUsedEventHandler(Node3D node);
    
    private CanvasLayer _canvasLayer;
    private bool _inventoryIsOpen = false;
    
    //TODO Change ItemList to GridContainer with ImageButtons or some other type of inventory that is more convenient
    private ItemList _itemList;
    public override void _Ready()
    {
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _canvasLayer.Visible = _inventoryIsOpen;
        
        
        
        _itemList = GetNode<ItemList>("CanvasLayer/ItemList");
        _itemList.ItemActivated += UseItem;
    }
    public void ToggleInventory()
    {
        _inventoryIsOpen = !_inventoryIsOpen;
        _canvasLayer.Visible = _inventoryIsOpen;
    }

    public void UpdateInventory(string itemName)
    {
        //Call down from game manager to add the item looted from player to the inventory
        _itemList.AddItem(itemName);
    }

    private void UseItem(long idx)
    {
        //Node3D node = _itemList.GetItem
        //EmitSignal(nameof(ItemUsedEventHandler), node);
    }
    
}
