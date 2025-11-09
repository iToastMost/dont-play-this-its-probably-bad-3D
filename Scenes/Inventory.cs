using Godot;
using System;

public partial class Inventory : Control
{
    private CanvasLayer _canvasLayer;
    private bool _inventoryIsOpen = false;
    private ItemList _itemList;
    public override void _Ready()
    {
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _canvasLayer.Visible = _inventoryIsOpen;
        
        _itemList = GetNode<ItemList>("CanvasLayer/ItemList");
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
}
