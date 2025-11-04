using Godot;
using System;

[Tool]
public partial class CameraSwitcher : Area3D
{
    [Export]
    private Camera3D _currentCamera;

    [Export]
    private Camera3D _nextCamera;

    [Export] private double sizeX;
    [Export] private double sizeY;
    [Export] private double sizeZ;
    
    CollisionShape3D _collisionShape;

    public override void _Ready()
    {
        BodyEntered += OnAreaEnter;
        _collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
        
    }

    public void OnAreaEnter(Node3D body) 
    {
        GD.Print("Body entered");
        _currentCamera.Current = true;
        //if (_currentCamera.Current == true) 
        //{
        //    _nextCamera.Current = true;
        //}
        //else 
        //{
        //    _currentCamera.Current = true;
        //}
        
    }
}
