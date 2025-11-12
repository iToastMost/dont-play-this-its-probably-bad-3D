using Godot;
using System;

public abstract partial class EquippableItem : ItemBase, iEquippable
{
	public Node3D EquipItem()
    {
        return this;
    }
}
