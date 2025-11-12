using Godot;
using System;

public interface iConsumable : iLootable
{
    void Consume(PlayerRE player);
}
