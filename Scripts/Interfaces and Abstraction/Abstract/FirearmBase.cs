using Godot;
using System;

public abstract partial class Firearm : EquippableItem
{
    [Export]
    public int MagazineSize;

    [Export]
    public double FireRate;

    [Export]
    public double Damage;

    [Export]
    public Node3D MuzzlePosition;

    public abstract void Shoot();
    public abstract void Reload();
}
