using Godot;
using System;

// a projectile hitbox is one that applies damage once and then deletes its owner
public partial class ProjectileHitboxComponent : Area2D
{
    [Export] private int damage;

    public override void _Ready()
    {
        AreaEntered += HandleAreaEntered;
    }

    private void HandleAreaEntered(Area2D other)
    {
        if (other is not HurtboxComponent hurtbox) return;
        hurtbox.ApplyDamage(damage);
        Owner.QueueFree();
    }
}
