using Godot;
using System;

public abstract partial class FirearmBase : EquippableItem
{
    [Export]
    public int MagazineSize;

    [Export]
    public double FireRate;

    [Export]
    public double Damage;

    [Export]
    public Node3D MuzzlePosition;

    [Export] 
    public string AmmoType;

    public abstract void Shoot();
    public abstract void Reload();
}
