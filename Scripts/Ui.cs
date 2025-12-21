using Godot;
using System;

public partial class Ui : Control
{
	private Label _healthLabel;
	private Label _ammoLabel;

	private ColorRect _colorRect;
	private Inventory _inventory;
	Map _map;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_inventory = GetNode<Inventory>("Inventory");
		_colorRect = GetNode<ColorRect>("CanvasLayer/ColorRect");
		_map = GetNode<Map>("CanvasLayer/Map");
		_healthLabel = GetNode<Label>("CanvasLayer/HealthLabel");
		_ammoLabel = GetNode<Label>("CanvasLayer/AmmoLabel");
		//this.Connect("ShowDialog", new Callable(this, nameof(UpdateText)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("accept_dialog")) 
		{
			_colorRect.Visible = false;
		}

		if (Input.IsActionJustPressed("open_inventory"))
		{
			_inventory.ToggleInventory(_healthLabel, _ammoLabel);
		}

		if (Input.IsActionJustPressed("toggle_map"))
			_map.ToggleMap();
	}

	public void UpdateText(string dialog) 
	{
		var label = GetNode<RichTextLabel>("CanvasLayer/ColorRect/RichTextLabel");
		label.Text = dialog;
        _colorRect.Visible = true;
        label.Visible = true;
	}
}
