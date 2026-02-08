using Godot;
using System;

public partial class EventItemBase : ItemBase, iKeyItem
{
    [Export]
    public string EventId { get; set; }

}
