using Godot;
using System;

public partial class Spring : Area2D
{
    public override void _Ready()
    {
        
    }
    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
        GD.Print("Spring Freed");
    }

    public void PlayAnimation() 
    {
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        animatedSprite2D.Play();
    }
}
