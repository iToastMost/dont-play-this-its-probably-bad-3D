using Godot;
using System;

public partial class Inventory : Control
{
    [Signal] public delegate void ItemUsedEventHandler(Button slot, int idx);
    
    private CanvasLayer _canvasLayer;
    private bool _inventoryIsOpen = false;
    private int _slotIdx = 0;
    
    //TODO Change ItemList to GridContainer with ImageButtons or some other type of inventory that is more convenient
    private GridContainer _gridContainer;
    public override void _Ready()
    {
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _canvasLayer.Visible = _inventoryIsOpen;
        
        _gridContainer = GetNode<GridContainer>("CanvasLayer/GridContainer");
        foreach (Button button in _gridContainer.GetChildren())
        {
            var idx = _slotIdx;
            button.Pressed += () => UseItem(button, idx);
            _slotIdx++;
        }
    }
    public void ToggleInventory()
    {
        _inventoryIsOpen = !_inventoryIsOpen;
        _canvasLayer.Visible = _inventoryIsOpen;
    }

    public void UpdateInventory(string itemName, int slotIdx)
    {
        //Call down from game manager to add the item looted from player to the inventory
       var slot = _gridContainer.GetNode<Button>("Slot" + slotIdx);
       slot.Text = itemName;
    }

    private void UseItem(Button slot, int idx)
    {
        EmitSignal(SignalName.ItemUsed, slot, idx);
    }
    
}
