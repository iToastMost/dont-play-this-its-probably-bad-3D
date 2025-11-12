using Godot;
using System;

public abstract partial class ConsumableItemBase : ItemBase, iConsumable
{
    [Export] public int HealAmount;
    
    //public abstract void Loot(Node3D[] inventory);
    //public abstract string GetName();
    public void Consume(PlayerRE player)
    {
        GD.Print(HealAmount);
        player._health += HealAmount;
    }
}
