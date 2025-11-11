using Godot;
using System;

public interface iLootable
{
    void Loot(int[] inventory, int itemID);
    string GetName();
    int GetID();
}
