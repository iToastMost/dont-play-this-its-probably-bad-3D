using Godot;
using System;

public interface iConsumable : iLootable
{
    int Consume(int amount);
}
