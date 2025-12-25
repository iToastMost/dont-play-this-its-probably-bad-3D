using Godot;
using System;

public partial class MapTile : ColorRect
{
    [Export]
    public string AreaName { get; set; }
    
    private ColorRect _colorRect;
    private Polygon2D _polygon;
    private Label _label;
    private Vector2 _tileSize;
    public override void _Ready()
    {
        _colorRect = this;
        _label = GetNode<Label>("Label");
        _polygon = GetNode<Polygon2D>("Polygon2D");
        
        _tileSize = _colorRect.Size;
        CalculateBorders();
        
        _label.Text = AreaName;
    }

    private void CalculateBorders()
    {
        _polygon.Polygon = new Vector2[]
        {
            new(0,0), 
            new (_tileSize.X, 0), 
            new (_tileSize.X, _tileSize.Y), 
            new (0,_tileSize.Y)
        };
    }
}
