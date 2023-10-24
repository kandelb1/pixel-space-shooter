using Godot;
using System;

public partial class ReflectorShieldPickup : LootPickup
{
    [Export] private PackedScene reflectorShieldScene;
    [Export] private string soundEffectPath;
    
    private const float SHIELD_TIME = 15f;
    
    public override void OnPickup(Ship playerShip)
    {
        // give the player the shield for 15 seconds, or add 15 seconds to the timer if they already have one
        ReflectorShield existingShield = playerShip.GetNodeOrNull<ReflectorShield>("ReflectorShield");
        if (existingShield == null)
        {
            ReflectorShield shield = reflectorShieldScene.Instantiate<ReflectorShield>();
            shield.IncreaseTimeLeft(SHIELD_TIME);
            shield.SetPowerupIcon(GetNode<AnimatedSprite2D>("AnimatedSprite2D").SpriteFrames.GetFrameTexture("default", 0));
            // need to use CallDeferred because we can't add a child to player on the same frame as the collision with the pickup. idk, Godot stuff.
            CallDeferred(MethodName.AddShield, playerShip, shield);
        }
        else
        {
            existingShield.IncreaseTimeLeft(SHIELD_TIME);    
        }
        // having issues with an [Export] AudioStream, so I'm passing paths around
        AudioManager.Instance.PlaySound(soundEffectPath);
    }

    private void AddShield(Ship playerShip, ReflectorShield shield)
    {
        playerShip.AddChild(shield);
    }
}
