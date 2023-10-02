using Godot;
using System;

public partial class HurtboxComponent : Area2D
{

    private HealthComponent healthComponent;
    
    public override void _Ready()
    {
        healthComponent = Owner.GetNodeOrNull<HealthComponent>("HealthComponent");
        AreaEntered += HandleCollision;
    }

    private void HandleCollision(Area2D other)
    {
        if (other is not HitboxComponent hitboxComponent) return;
        // GD.Print($"Hurtbox on {Owner.Name} collided with {other.Owner.Name}");
        healthComponent?.Damage(hitboxComponent.GetDamage());
        other.Owner.QueueFree();
    }
}
