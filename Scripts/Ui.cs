using Godot;
using System;

public partial class Ui : Control
{
	private ColorRect _colorRect;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_colorRect = GetNode<ColorRect>("CanvasLayer/ColorRect");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("accept_dialog")) 
		{
			_colorRect.Visible = false;
		}
	}

	public void UpdateText(string dialog) 
	{
		var label = GetNode<RichTextLabel>("CanvasLayer/ColorRect/RichTextLabel");
		label.Text = dialog;
        _colorRect.Visible = true;
        label.Visible = true;
	}
}
