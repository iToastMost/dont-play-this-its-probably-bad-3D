using Godot;
using System;

public interface iEquippable : iLootable
{
    Node3D EquipItem();
}
