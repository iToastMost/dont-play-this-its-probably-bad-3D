using Godot;
using System;

public partial class OneJumpPlatform : Area2D
{
    AnimationPlayer animation;
    Timer timer;
    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        animation = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void Hit() 
    {
        animation.CurrentAnimation = "shrink";
        animation.Play();
        timer.Start();
    }

    private void OnTimer() 
    {
        animation?.Stop();
        QueueFree();
    }
}
