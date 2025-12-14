using Godot;
using System;

public abstract partial class KeyItemBase : ItemBase, iKeyItem
{
    [Export] public int KeyId { get; set; }
    
}
