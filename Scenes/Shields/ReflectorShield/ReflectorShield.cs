using Godot;
using System;

public partial class ReflectorShield : AnimatedSprite2D, IPowerup
{
    private const int PLAYER_PROJECTILE_COLLISION_LAYER = 2;
    private const int ENEMY_COLLISION_LAYER = 4;

    private float timeLeftSeconds;
    private Texture2D powerupIcon;
    
    public override void _Ready()
    {
        GetNode<Area2D>("Area2D").AreaEntered += HandleAreaEntered;
        GameEventBus.Instance.PowerupAcquired?.Invoke(this);
    }

    public override void _Process(double delta)
    {
        timeLeftSeconds -= (float) delta;        
        if (timeLeftSeconds <= 0)
        {
            QueueFree();
        }
    }

    public void IncreaseTimeLeft(float seconds) => timeLeftSeconds += seconds;

    private void HandleAreaEntered(Area2D other)
    {
        if (other is not ProjectileHitboxComponent hitbox) return;
        VelocityComponent velocityComponent = hitbox.Owner.GetNodeOrNull<VelocityComponent>("VelocityComponent");
        if (velocityComponent == null) return;
        
        // reflect the projectile
        Vector2 normal = new Vector2(0, -1).Rotated(GlobalRotation).Normalized();
        Vector2 newVelocity = velocityComponent.Velocity.Bounce(normal);
        velocityComponent.SetVelocity(newVelocity);

        // convert it to a player projectile that can hit enemies
        hitbox.CollisionLayer = PLAYER_PROJECTILE_COLLISION_LAYER;
        hitbox.CollisionMask = ENEMY_COLLISION_LAYER;
    }

    public void SetPowerupIcon(Texture2D icon) => powerupIcon = icon;

    public Texture2D GetIcon() => powerupIcon;

    public float GetTimeRemaining() => timeLeftSeconds;
}
