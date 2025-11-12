using Godot;
using System;

public abstract partial class Firearm : EquippableItem
{
    [Export]
    public int MagazineSize;

    [export]
    public double FireRate;

    [Export]
    public double Damage;

    [Export]
    public Node3D MuzzlePosition;

    void Shoot();
    void Reload();
}
