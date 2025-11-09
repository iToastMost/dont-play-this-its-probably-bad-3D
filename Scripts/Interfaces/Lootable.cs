using Godot;
using System;

public interface iLootable
{
    void Loot(Node3D[] inventory);
    string GetName();
}
