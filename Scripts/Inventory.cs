using Godot;
using System;

public partial class Inventory : Control
{
    [Signal] 
    public delegate void ItemUsedEventHandler(int idx);

    [Signal]
    public delegate void CheckItemSlotClickedEventHandler(int idx);
    
    [Signal]
    public delegate void InspectItemEventHandler(int idx);
    
    private CanvasLayer _canvasLayer;
    private bool _inventoryIsOpen = false;
    private int _slotIdx = 0;
    private int _slotSelectedIdx = 0;

    private Button _useItemButton;
    private Button _inspectItemButton;
    
    //TODO Change ItemList to GridContainer with ImageButtons or some other type of inventory that is more convenient
    private GridContainer _gridContainer;
    private GridContainer _itemSelected;
    private AnimationPlayer _animationPlayer;
    private AudioStreamPlayer2D _audioStreamPlayer2D;
   
    private bool _isItemSelected = false;
    public override void _Ready()
    {
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _canvasLayer.Visible = _inventoryIsOpen;
        
        _gridContainer = GetNode<GridContainer>("CanvasLayer/GridContainer");
        _itemSelected = GetNode<GridContainer>("CanvasLayer/ItemSelectedSlide/ItemSelected");
        _animationPlayer = GetNode<AnimationPlayer>("CanvasLayer/ItemSelectedSlide");
        _audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        _itemSelected.Visible = false;
        
        _useItemButton =  GetNode<Button>("CanvasLayer/ItemSelectedSlide/ItemSelected/Use_Equip");
        _inspectItemButton = GetNode<Button>("CanvasLayer/ItemSelectedSlide/ItemSelected/Inspect");
        
        _useItemButton.Pressed += UseItemClicked;
        _inspectItemButton.Pressed += InspectItemClicked;
        
        _animationPlayer.AnimationFinished += HideItemMenu;
        
        foreach (Button button in _gridContainer.GetChildren())
        {
            var idx = _slotIdx;
            button.Pressed += () => CheckItemClicked(idx);
            button.FocusEntered += CloseItemMenuOnFocus;
            button.FocusEntered += PlayUiChangeSound;
            _slotIdx++;
        }

        _itemSelected.GetNode<Button>("Use_Equip").FocusEntered += PlayUiChangeSound;
        _itemSelected.GetNode<Button>("Inspect").FocusEntered += PlayUiChangeSound;
        _itemSelected.GetNode<Button>("Combine").FocusEntered += PlayUiChangeSound;
        
        _audioStreamPlayer2D.Stream = ResourceLoader.Load<AudioStreamWav>("res://Art/Audio/uiChange.wav");
        
    }
    public void ToggleInventory(Label _healthLabel, Label _ammoLabel)
    {
        _inventoryIsOpen = !_inventoryIsOpen;
        _canvasLayer.Visible = _inventoryIsOpen;
        _healthLabel.Visible = _inventoryIsOpen;
        _ammoLabel.Visible = _inventoryIsOpen;
        _gridContainer.GetNode<Button>("Slot0").GrabFocus();
        if (_itemSelected.Visible)
        {
            _itemSelected.Visible = false;
            _isItemSelected = false;
        }
        
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
        
        if (_isItemSelected) 
            return;
        
        _itemSelected.GetNode<Button>("Use_Equip").GrabFocus();
        _itemSelected.GetNode<Button>("Use_Equip").FocusPrevious = _gridContainer.GetNode<Button>("Slot" +  _slotSelectedIdx).GetPath();
        _itemSelected.GetNode<Button>("Inspect").FocusPrevious = _gridContainer.GetNode<Button>("Slot" +  _slotSelectedIdx).GetPath();
        _itemSelected.GetNode<Button>("Combine").FocusPrevious = _gridContainer.GetNode<Button>("Slot" +  _slotSelectedIdx).GetPath();
        
        _isItemSelected = true;
        _animationPlayer.CurrentAnimation = "slide_out";
        _animationPlayer.Play();

    }
    
    private void UseItemClicked()
    {
        EmitSignal(SignalName.ItemUsed, _slotSelectedIdx);
        
        _audioStreamPlayer2D.Stream = ResourceLoader.Load<AudioStreamWav>("res://Art/Audio/uiSelect.wav");
        _audioStreamPlayer2D.Play();
        
        _animationPlayer.CurrentAnimation = "slide_in";
        _gridContainer.GetNode<Button>("Slot0").GrabFocus();
        _isItemSelected = false;
    }

    private async void InspectItemClicked()
    {
        EmitSignal(SignalName.InspectItem, _slotSelectedIdx);
        
        _audioStreamPlayer2D.Stream = ResourceLoader.Load<AudioStreamWav>("res://Art/Audio/uiSelect.wav");
        _audioStreamPlayer2D.Play();
        
        _inspectItemButton.ReleaseFocus();
        _animationPlayer.CurrentAnimation = "slide_in";
        
        await ToSignal(GetNode<GameManager>("/root/GameManager"), GameManager.SignalName.DialogueCompleted);
        
        _gridContainer.GetNode<Button>("Slot0").GrabFocus();
        _isItemSelected = false;
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
        if (anim == "slide_in")
        {
            _audioStreamPlayer2D.Stream = ResourceLoader.Load<AudioStreamWav>("res://Art/Audio/uiChange.wav");
            _itemSelected.Visible = false;
        }
            
    }
    
    private void CloseItemMenuOnFocus()
    {
        _animationPlayer.CurrentAnimation = "slide_in";
        _isItemSelected = false;
    }

    private void PlayUiChangeSound()
    {
        _audioStreamPlayer2D.Play();
    }
}
