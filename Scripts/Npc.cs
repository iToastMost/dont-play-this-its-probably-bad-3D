using Godot;
using System;

public partial class Npc : CharacterBody3D
{
	[Export]
	public string NPCName { get; set; }
	
	[Export]
	public string NPCDialogue { get; set; }

	private AnimationPlayer _animationPlayer;
	
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("NPC_Model/AnimationPlayer");
		_animationPlayer.CurrentAnimation = "Idle";
		_animationPlayer.Play("Idle");
	}
}
