using Godot;
using System;

public partial class BigCannonPickup : LootPickup
{
    // public override void _Ready()
    // {
    //     GD.Print("BigCannonPickup._Ready()");
    //     base._Ready(); // if we override the ready method, we have to call it on the base class
    // }
    
    public override void OnPickup(Ship playerShip)
    {
        BigCannonWeapon cannonWeapon = playerShip.GetWeapon<BigCannonWeapon>();
        cannonWeapon?.AddAmmo(3);
    }
}
