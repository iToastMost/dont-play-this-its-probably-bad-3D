using Godot;
using System;

public abstract class Item : iLootable
{
    public void Loot(Node3D[] inventory){}
    public string GetName()
    {
        return "";
    }
}
