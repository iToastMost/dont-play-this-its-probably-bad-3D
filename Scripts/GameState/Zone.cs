using Godot;
using System;

public partial class Zone : Node3D
{
	[Signal]
	public delegate void ZoneEnteredEventHandler(string zoneId);
	
	[Export]
	public string ZoneId {get;set;}

	public override void _Ready()
	{
		EmitSignal(SignalName.ZoneEntered, ZoneId);
	}

}
