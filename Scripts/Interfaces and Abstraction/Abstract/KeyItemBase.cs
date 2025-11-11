using Godot;
using System;

public abstract partial class KeyItemBase : ItemBase, iKeyItem
{
    public void Loot(Node3D[] inventory)
    {
        throw new NotImplementedException();
    }

    public string GetName()
    {
        throw new NotImplementedException();
    }
}
