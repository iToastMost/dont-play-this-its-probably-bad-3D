using Godot;
using System;

public partial class Dialogue : Node
{
	[Export] public double TimerDuration;

	private string _displayText;
	private int _displayTextLength;
	private int _visibleText = 0;
	private float _tweenSpeed = 1f;
	
	
	private Timer NextCharacterTimer;
	private RichTextLabel RichTextLabel;
	private RichTextLabel _charName;
	private ColorRect ColorRect;
	private Label _continueLabel;
	private Tween _tween;
	
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NextCharacterTimer = GetNode<Timer>("NextCharacterTimer");
		RichTextLabel = GetNode<RichTextLabel>("RichTextLabel");
		_charName = GetNode<RichTextLabel>("ColorRect/CharName");
		ColorRect = GetNode<ColorRect>("ColorRect");
		_continueLabel = GetNode<Label>("ContinueLabel");
		RichTextLabel.VisibleCharacters = 0;

		NextCharacterTimer.Timeout += NextCharacterTimeout;
		_displayTextLength = RichTextLabel.Text.Length;

		NextCharacterTimer.WaitTime = TimerDuration;

		RichTextLabel.Visible = false;
		ColorRect.Visible = false;
		_continueLabel.Visible = false;
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

	public void ShowMessage(string name, string message)
	{
		_visibleText = 0;

		_charName.Text = name;
		RichTextLabel.Text = message;
		RichTextLabel.VisibleCharacters = _visibleText;
		_displayTextLength = RichTextLabel.Text.Length;

		_charName.Visible = true;
		RichTextLabel.Visible = true;
		ColorRect.Visible = true;
		_continueLabel.Visible = true;
		ContinueLabelTween();

		NextCharacterTimer.Start();
	}

	public void HideMessage()
	{
		if(_visibleText < _displayTextLength)
		{
			_visibleText = _displayTextLength - 1;
			return;
		}

		if(_tween != null && _tween.IsRunning())
			_tween.Kill();
		
		_charName.Visible = true;
		_continueLabel.Visible = false;
		RichTextLabel.Text = "";
		RichTextLabel.Visible = false;
		ColorRect.Visible = false;
	}

	private void ContinueLabelTween()
	{
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(_continueLabel, "modulate:a", 0.75, _tweenSpeed / 2);
		_tween.TweenProperty(_continueLabel, "modulate:a", 0.25, _tweenSpeed / 2);
		_tween.SetLoops();
	}

}
