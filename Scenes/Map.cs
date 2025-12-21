using Godot;
using System;

public partial class Map : Control
{
    private bool _isMapOpen = false;
    CanvasLayer _canvasLayer;
    
    public override void _Ready()
    {
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _canvasLayer.Visible = false;
    }

    public void ToggleMap()
    {
        _isMapOpen = !_isMapOpen;
        _canvasLayer.Visible = _isMapOpen;
    }

}
