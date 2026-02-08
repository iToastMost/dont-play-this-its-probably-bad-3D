using Godot;
using System;

public partial class Ui : Control
{
	private Label _healthLabel;
	private Label _ammoLabel;
	
	private Inventory _inventory;
	Map _map;
	// Called when the node enters the scene tree for the first time.
	
	//TODO Change UI to not have a process. Send signals from player controller to Game Manager to handle controls and UI changes
	public override void _Ready()
	{
		_inventory = GetNode<Inventory>("Inventory");
		_map = GetNode<Map>("CanvasLayer/Map");
		_healthLabel = GetNode<Label>("CanvasLayer/HealthLabel");
		_ammoLabel = GetNode<Label>("CanvasLayer/AmmoLabel");
		//this.Connect("ShowDialog", new Callable(this, nameof(UpdateText)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("open_inventory"))
		{
			ToggleInventory();
		}

		if (Input.IsActionJustPressed("toggle_map"))
			_map.ToggleMap();
	}

	public void UpdateText(string dialog) 
	{
		var label = GetNode<RichTextLabel>("CanvasLayer/ColorRect/RichTextLabel");
		label.Text = dialog;
        label.Visible = true;
	}

	public void ToggleInventory()
	{
		_inventory.ToggleInventory(_healthLabel, _ammoLabel);
	}
}
