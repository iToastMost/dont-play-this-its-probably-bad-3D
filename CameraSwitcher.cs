using Godot;
using System;

public partial class CameraSwitcher : Area3D
{
    [Export]
    private Camera3D _currentCamera;

    [Export]
    private Camera3D _nextCamera;

    public override void _Ready()
    {
        
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
