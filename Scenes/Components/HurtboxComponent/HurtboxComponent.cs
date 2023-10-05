using Godot;
using System;

public partial class HurtboxComponent : Area2D
{
    private HealthComponent healthComponent;
    
    public override void _Ready()
    {
        healthComponent = Owner.GetNodeOrNull<HealthComponent>("HealthComponent");
    }

    public void ApplyDamage(int damage)
    {
        healthComponent?.Damage(damage);
    }
}
