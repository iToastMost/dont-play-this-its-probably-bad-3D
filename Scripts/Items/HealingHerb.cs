using Godot;
using System;

public partial class HealingHerb : StaticBody3D, iLootable
{
    public void Loot(Node3D[] inventory)
    {
        inventory[0] = this;
        QueueFree();
    }
}
