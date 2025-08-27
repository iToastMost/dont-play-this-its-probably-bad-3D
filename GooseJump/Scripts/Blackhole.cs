using Godot;
using System;

public partial class Blackhole : Area2D
{

	Sprite2D blackHole;
    public override void _Ready()
    {
		blackHole = GetNode<Sprite2D>("BlackholeBase");
    }
    public override void _Process(double delta) 
	{
		blackHole.Rotate(2 * (float)delta) ;
	}
	public void OnBodyEntered(Node2D body) 
	{
		if(body is Player player) 
		{
			player.Die();
		}
	}
}
