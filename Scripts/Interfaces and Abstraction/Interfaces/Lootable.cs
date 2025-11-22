using Godot;
using System;

public interface iLootable
{
    void Loot(ItemBase[] inventory, int itemID, int inventoryIdx);
    string GetName();
    int GetID();
}
