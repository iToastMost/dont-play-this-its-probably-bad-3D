using Godot;
using System;

public partial class Dialogue : Node
{
	[Export] public double TimerDuration;

	private string _displayText;
	private int _displayTextLength;
	private int _visibleText = 0;

	private Timer NextCharacterTimer;
	private RichTextLabel RichTextLabel;
	private ColorRect ColorRect;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NextCharacterTimer = GetNode<Timer>("NextCharacterTimer");
		RichTextLabel = GetNode<RichTextLabel>("RichTextLabel");
		ColorRect = GetNode<ColorRect>("ColorRect");
		RichTextLabel.VisibleCharacters = 0;

		NextCharacterTimer.Timeout += NextCharacterTimeout;
		_displayTextLength = RichTextLabel.Text.Length;

		NextCharacterTimer.WaitTime = TimerDuration;

		RichTextLabel.Visible = false;
		ColorRect.Visible = false;
		//NextCharacterTimer.Start();
	}

	private void NextCharacterTimeout()
	{
		if (_displayTextLength == (_visibleText - 1))
		{
			NextCharacterTimer.Stop();
		}

		_visibleText++;
		RichTextLabel.VisibleCharacters = _visibleText;
	}

	public void ShowMessage(string message)
	{
		_visibleText = 0;
		
		RichTextLabel.Text = message;
		RichTextLabel.VisibleCharacters = _visibleText;
		_displayTextLength = RichTextLabel.Text.Length;

		RichTextLabel.Visible = true;
		ColorRect.Visible = true;

		NextCharacterTimer.Start();
	}

	public void HideMessage()
	{
		RichTextLabel.Text = "";
		RichTextLabel.Visible = false;
		ColorRect.Visible = false;
	}

}
