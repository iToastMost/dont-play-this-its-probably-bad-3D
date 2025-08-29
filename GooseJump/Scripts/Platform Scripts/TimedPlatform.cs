using Godot;
using System;

public partial class TimedPlatform : StaticBody2D
{
    Timer timer;
    private bool timerStarted;
    private double duration;

    [Export]
    private double timeRangeStart = 0;

    [Export]
    private double timeRangeEnd = 0;

    Timer tweenStartTimer;
    Tween tween;
    AnimatedSprite2D animation;
    Sprite2D platform;
    public override void _Ready()
    {
        duration = GD.RandRange(timeRangeStart, timeRangeEnd);
        timer = GetNode<Timer>("Timer");
        tweenStartTimer = GetNode<Timer>("StartTweenTimer");
        animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        platform = GetNode<Sprite2D>("Platform");
        timerStarted = false;
        timer.WaitTime = duration;
        
        if(duration - 4 < 0) 
        {
            ColorTween(2.0);
        }
        else 
        {
           tweenStartTimer.WaitTime = duration - 4.0;
        }
       

    }

    public void StartTween() 
    {
        ColorTween(4.0);
    }

    public void Timeout() 
    {
        platform.Visible = false;
        animation.Visible = true;
        animation.Play();
    }
    
    private void AnimationFinished() 
    {
        animation.QueueFree();
        QueueFree();
    }

    private void ColorTween(double duration) 
    {
        var body = GetNode<Sprite2D>("Platform");
        tween = GetTree().CreateTween().SetProcessMode(Tween.TweenProcessMode.Physics);

        var redColor = new Color(1f, 0f, 0f, 1f);
        redColor.R = 255;
        redColor.G = 0;
        redColor.B = 0;
        redColor.A = 255;

        tween.SetLoops(1).SetParallel(false);
        tween.TweenProperty(body, "modulate", redColor, duration).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
    }

    public void OnScreenEntered() 
    {
        if(timerStarted == false) 
        {
            timer.Start();
            timerStarted = true;
            tweenStartTimer.Start();
        }
        
    }
}
