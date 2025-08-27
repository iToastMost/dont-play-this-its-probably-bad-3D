using Godot;
using System;

public partial class InvisiblePlatform : StaticBody2D
{
    [Export]
    StaticBody2D NextInvisiblePlatform {  get; set; }

    [Export]
    public bool _isVisible = false;

    private float _duration = 2;

    private Sprite2D _opacity;

    private Tween _tween;
    public override void _Ready()
    {
        AddToGroup("Platforms");
        _opacity = GetNode<Sprite2D>("Platform");

        if (_isVisible) 
        {
            _opacity.Modulate = new Color(255, 255, 255);
            StartTween();
        }
        else 
        {
            _opacity.Modulate = new Color(255,255,255,0);
        }
    }

    private void StartTween() 
    {
        
        float opacityLow = 0.4f;
        float opacityHigh = 0.7f;

        _tween = GetTree().CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics);

        _tween.SetLoops().SetParallel(false);
        _tween.TweenProperty(_opacity, "modulate:a", opacityLow, _duration / 2).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        _tween.TweenProperty(_opacity, "modulate:a", opacityHigh, _duration / 2).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
    }

    private void FreePlatform() 
    {
        _tween?.Kill();
        SetProcess(false);
    }

    public void ShowNextPlatform() 
    {
        this.Modulate = new Color(0, 0, 0, 0);
        if(NextInvisiblePlatform  != null && NextInvisiblePlatform is InvisiblePlatform invisiblePlatform) 
        {
            invisiblePlatform.ChangeVisibility();
        }
        QueueFree();
    }

    private void ChangeVisibility() 
    {
        if (_tween == null) 
        {
            StartTween();
        }
    }

    public override void _ExitTree()
    {
        FreePlatform();
    }

}
