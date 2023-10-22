using Godot;
using System;

public partial class ReflectorShieldPickup : LootPickup
{
    [Export] private PackedScene reflectorShieldScene;

    private const float SHIELD_TIME = 15f;
    
    public override void OnPickup(Ship playerShip)
    {
        // give the player the shield for 15 seconds, or add 15 seconds to the timer if they already have one
        ReflectorShield existingShield = playerShip.GetNodeOrNull<ReflectorShield>("ReflectorShield");
        if (existingShield == null)
        {
            ReflectorShield shield = reflectorShieldScene.Instantiate<ReflectorShield>();
            shield.IncreaseTimeLeft(15f);
            // need to use CallDeferred because we can't add a child to player on the same frame as the collision with the pickup. idk, Godot stuff.
            CallDeferred(MethodName.AddShield, playerShip, shield);
        }
        else
        {
            existingShield.IncreaseTimeLeft(15f);    
        }
    }

    private void AddShield(Ship playerShip, ReflectorShield shield)
    {
        playerShip.AddChild(shield);
    }
}
