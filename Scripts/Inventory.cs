using Godot;
using System;

public partial class Inventory : Control
{
    [Signal] 
    public delegate void ItemUsedEventHandler(int idx);

    [Signal]
    public delegate void CheckItemSlotClickedEventHandler(int idx);
    
    private CanvasLayer _canvasLayer;
    private bool _inventoryIsOpen = false;
    private int _slotIdx = 0;
    private int _slotSelectedIdx = 0;

    private Button _useItemButton;
    
    //TODO Change ItemList to GridContainer with ImageButtons or some other type of inventory that is more convenient
    private GridContainer _gridContainer;
    private GridContainer _itemSelected;
    private AnimationPlayer _animationPlayer;
    public override void _Ready()
    {
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _canvasLayer.Visible = _inventoryIsOpen;
        
        _gridContainer = GetNode<GridContainer>("CanvasLayer/GridContainer");
        _itemSelected = GetNode<GridContainer>("CanvasLayer/ItemSelectedSlide/ItemSelected");
        _animationPlayer = GetNode<AnimationPlayer>("CanvasLayer/ItemSelectedSlide");
        _itemSelected.Visible = false;
        
        _useItemButton =  GetNode<Button>("CanvasLayer/ItemSelectedSlide/ItemSelected/Use_Equip");
        
        _useItemButton.Pressed += UseItemClicked;
        
        // foreach (Button button in _gridContainer.GetChildren())
        // {
        //     var idx = _slotIdx;
        //     button.Pressed += () => UseItem(button, idx);
        //     _slotIdx++;
        // }
        
        foreach (Button button in _gridContainer.GetChildren())
        {
            var idx = _slotIdx;
            button.Pressed += () => CheckItemClicked(idx);
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
        //EmitSignal(SignalName.ItemUsed, slot, idx);
    }

    private void CheckItemClicked(int idx)
    {
        EmitSignalCheckItemSlotClicked(idx);
    }
    
    public void ItemClicked(int idx)
    {
        _slotSelectedIdx = idx;
        _itemSelected.Visible = true;
        _animationPlayer.CurrentAnimation = "slide_out";
        _animationPlayer.Play();
    }
    
    private void UseItemClicked()
    {
        EmitSignal(SignalName.ItemUsed, _slotSelectedIdx);
        _animationPlayer.CurrentAnimation = "slide_in";
        _animationPlayer.AnimationFinished += HideItemMenu;
    }

    private void InspectItemClicked()
    {
        
    }
    
    private void CombineItemClicked()
    {
        
    }

    public void UpdateUseEquipItemButtonText(string text)
    {
        _useItemButton.Text = text;
    }

    private void HideItemMenu(StringName anim)
    {
        if(anim == "slide_in")
            _itemSelected.Visible = false;
    }
    
}
