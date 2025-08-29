using Godot;
using System;
using System.Collections.Generic;

public partial class VerticalPlatform : StaticBody2D
{
	[Export]
	public float moveDistance = -400f;

	private double duration = 5;
	
	private Tween tween;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("Platforms");
        StartTween();
    }

	private void StartTween() 
	{
		GD.Print("Tweening off my gourd");
		var body = GetNode<AnimatableBody2D>("AnimatableBody2D");

		Vector2 start = body.GlobalPosition;
		Vector2 end = start + new Vector2(0, moveDistance);

		tween = GetTree().CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics);

		tween.SetLoops(100).SetParallel(false);
		tween.TweenProperty(body, "global_position", end, duration / 2).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(body, "global_position", start, duration / 2).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
    }

    public void FreeMovingPlatform()
    {
		tween?.Kill();
        SetProcess(false);
    }

    public override void _ExitTree()
    {
        FreeMovingPlatform();
    }

}
