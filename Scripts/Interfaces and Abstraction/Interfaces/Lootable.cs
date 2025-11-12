using Godot;
using System;

public interface iLootable
{
    void Loot(int[] inventory, int itemID, int inventoryIdx);
    string GetName();
    int GetID();
}
