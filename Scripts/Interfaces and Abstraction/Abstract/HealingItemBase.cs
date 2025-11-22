using Godot;
using System;

public abstract partial class HealingItemBase : ItemBase, iConsumable
{
    [Export] public int HealAmount;
    
    //public abstract void Loot(Node3D[] inventory);
    //public abstract string GetName();
    public int Consume()
    {
        GD.Print(HealAmount);
        return HealAmount;
    }
}
