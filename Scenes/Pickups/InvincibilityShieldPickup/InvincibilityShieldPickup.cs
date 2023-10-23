using Godot;
using System;

public partial class InvincibilityShieldPickup : LootPickup
{
    [Export] private PackedScene invincibilityShieldScene;

    private const float SHIELD_TIME = 10f;
    
    public override void OnPickup(Ship playerShip)
    {
        // give the player the shield for 15 seconds, or add 15 seconds to the timer if they already have one
        InvincibilityShield existingShield = playerShip.GetNodeOrNull<InvincibilityShield>("InvincibilityShield");
        if (existingShield == null)
        {
            InvincibilityShield shield = invincibilityShieldScene.Instantiate<InvincibilityShield>();
            shield.IncreaseTimeLeft(SHIELD_TIME);
            // need to use CallDeferred because we can't add a child to player on the same frame as the collision with the pickup. idk, Godot stuff.
            CallDeferred(MethodName.AddShield, playerShip, shield);
        }
        else
        {
            existingShield.IncreaseTimeLeft(SHIELD_TIME);    
        }
    }
    
    private void AddShield(Ship playerShip, InvincibilityShield shield)
    {
        playerShip.AddChild(shield);
    }
}
