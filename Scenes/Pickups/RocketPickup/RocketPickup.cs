using Godot;
using System;

public partial class RocketPickup : LootPickup
{
    public override void OnPickup(Ship playerShip)
    {
        RocketWeapon rocketWeapon = playerShip.GetWeapon<RocketWeapon>();
        rocketWeapon?.RefillAmmo();
    }
}
