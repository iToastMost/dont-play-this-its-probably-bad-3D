using Godot;
using System;

public partial class Dialogue : Node
{
	[Signal]
	public delegate void YesLootButtonPressedEventHandler(Node3D item);
	
	[Signal]
	public delegate void ChoiceSelectedEventHandler(bool choice);

	[Signal]
	public delegate void DialogueFinishedEventHandler();
	
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

	private Button YesLootButton;
	private Button NoLootButton;
	private HFlowContainer _hFlowContainer;

	public Node3D ItemToLoot;
	
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NextCharacterTimer = GetNode<Timer>("NextCharacterTimer");
		RichTextLabel = GetNode<RichTextLabel>("RichTextLabel");
		_charName = GetNode<RichTextLabel>("ColorRect/CharName");
		ColorRect = GetNode<ColorRect>("ColorRect");
		_continueLabel = GetNode<Label>("ContinueLabel");
		YesLootButton = GetNode<Button>("ChoiceHFlowContainer/YesLootButton");
		NoLootButton = GetNode<Button>("ChoiceHFlowContainer/NoLootButton");
		_hFlowContainer = GetNode<HFlowContainer>("ChoiceHFlowContainer");
		YesLootButton.Pressed += YesLootPressed;
		NoLootButton.Pressed += NoLootPressed;
		
		RichTextLabel.VisibleCharacters = 0;

		NextCharacterTimer.Timeout += NextCharacterTimeout;
		_displayTextLength = RichTextLabel.Text.Length;

		NextCharacterTimer.WaitTime = TimerDuration;

		RichTextLabel.Visible = false;
		ColorRect.Visible = false;
		_continueLabel.Visible = false;
		_hFlowContainer.Visible = false;
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

	public void ShowMessage(bool showInstant, string name, string message)
	{
		_visibleText = 0;

		if (showInstant)
			_visibleText = message.Length;

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

	public void AskToLoot(Node3D item)
	{
		if (item is ItemBase itemBase)
		{
			var message = "Will you take the " + itemBase.GetName() + "?";
			_visibleText = message.Length;
            
			_charName.Text = "";
			RichTextLabel.Text = message;
			RichTextLabel.VisibleCharacters = _visibleText;
			_displayTextLength = RichTextLabel.Text.Length;
            
			_charName.Visible = true;
			RichTextLabel.Visible = true;
			ColorRect.Visible = true;
            
			_hFlowContainer.Visible = true;
			YesLootButton.GrabFocus();		
		}
	}

	public void AskToUseKey(Node3D key)
	{
		if (key is KeyItem keyToUse)
		{
			var message = "Will you use the " + keyToUse.GetName() + "?";
			_visibleText = message.Length;
            
			_charName.Text = "";
			RichTextLabel.Text = message;
			RichTextLabel.VisibleCharacters = _visibleText;
			_displayTextLength = RichTextLabel.Text.Length;
            
			_charName.Visible = true;
			RichTextLabel.Visible = true;
			ColorRect.Visible = true;
            
			_hFlowContainer.Visible = true;
			YesLootButton.GrabFocus();	
		}
	}
	
	public void AskToUse()
	{
		var message = "Use the keypad?";
		_visibleText = message.Length;
            
		_charName.Text = "";
		RichTextLabel.Text = message;
		RichTextLabel.VisibleCharacters = _visibleText;
		_displayTextLength = RichTextLabel.Text.Length;
            
		_charName.Visible = true;
		RichTextLabel.Visible = true;
		ColorRect.Visible = true;
            
		_hFlowContainer.Visible = true;
		YesLootButton.GrabFocus();	
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
		_hFlowContainer.Visible = false;
		YesLootButton.ReleaseFocus();
		EmitSignal(SignalName.DialogueFinished);
	}

	private void ContinueLabelTween()
	{
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(_continueLabel, "modulate:a", 0.75, _tweenSpeed / 2);
		_tween.TweenProperty(_continueLabel, "modulate:a", 0.25, _tweenSpeed / 2);
		_tween.SetLoops();
	}

	private void YesLootPressed()
	{
		//Sends looted signal to game manager with item id to tell manager what to loot
		if (ItemToLoot != null)
		{
			EmitSignal(SignalName.YesLootButtonPressed, ItemToLoot);
			
			//Sends out choice selected. Use async functions with await on this signal
			EmitSignalChoiceSelected(true);
			ItemToLoot = null;
			HideMessage();
			return;
		}
			
		//Sends out choice selected. Use async functions with await on this signal
		EmitSignalChoiceSelected(true);
		HideMessage();
	}

	private void NoLootPressed()
	{
		EmitSignalChoiceSelected(false);
		HideMessage();
	}

}
