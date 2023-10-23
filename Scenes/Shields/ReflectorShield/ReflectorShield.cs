using Godot;
using System;

public partial class ReflectorShield : AnimatedSprite2D
{
    private const int PLAYER_PROJECTILE_COLLISION_LAYER = 2;
    private const int ENEMY_COLLISION_LAYER = 4;

    private float timeLeftSeconds;
    
    public override void _Ready()
    {
        GetNode<Area2D>("Area2D").AreaEntered += HandleAreaEntered;
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
}
