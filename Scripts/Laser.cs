using Godot;
using System;

public partial class Laser : RayCast3D
{

	MeshInstance3D beamMesh;
	CylinderMesh beamShape;

	//private static int MAX_LENGTH = 1000;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		beamMesh = GetNode<MeshInstance3D>("Beam");
		beamShape = beamMesh.Mesh as CylinderMesh;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ProjectBeam();
	}

	private void ProjectBeam() 
	{
		ForceRaycastUpdate();

		if (IsColliding()) 
		{
			//GD.Print("Beam is colliding");
			var laserCollision = ToLocal(GetCollisionPoint());

			beamShape.Height = laserCollision.Y;
			beamMesh.Position = new Vector3(0, laserCollision.Y / 2f, 0);
		}
		else 
		{
			beamShape.Height = 0;
			beamMesh.Position = new Vector3(0, 0, 0);
		}
	}
}
