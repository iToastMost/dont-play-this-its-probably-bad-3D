using Godot;
using System;

public abstract partial class AmmoItemBase : ItemBase, iConsumable
{
    [Export]
    public int AmmoAmount { get; set; }
    
    [Export]
    public string AmmoType { get; set; }

    public int Consume() => Consume(1);
    
    public int Consume(int amount)
    {
        GD.Print("Ammo used");
        //12 is current max ammo, adjust later
        int toConsume = 12 - amount;
        if (AmmoAmount - toConsume < 0)
        {
            int empty = AmmoAmount;
            AmmoAmount -= toConsume;
            return empty;
        }
        
        AmmoAmount -= toConsume;
        return toConsume;
        
       // return AmmoAmount;
    }
}
