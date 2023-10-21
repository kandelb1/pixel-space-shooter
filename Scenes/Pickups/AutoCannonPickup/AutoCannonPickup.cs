using Godot;
using System;

public partial class AutoCannonPickup : LootPickup
{
    public override void OnPickup(Ship playerShip)
    {
        AutoCannonWeapon autoCannon = playerShip.GetWeapon<AutoCannonWeapon>();
        autoCannon?.IncreaseFiringSpeed(0.10f);
    }
}
