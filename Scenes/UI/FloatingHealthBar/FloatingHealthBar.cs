using Godot;
using System;

public partial class FloatingHealthBar : HBoxContainer
{
    private HealthComponent healthComponent;
    private TextureProgressBar progressBar;
    
    public override void _Ready()
    {
        healthComponent = Owner.GetNode<HealthComponent>("HealthComponent");
        healthComponent.HealthChanged += HandleHealthChanged;
        progressBar = GetNode<TextureProgressBar>("TextureProgressBar");
        HandleHealthChanged(healthComponent.GetCurrentHealth());
        // Rotation = 0;
    }

    private void HandleHealthChanged(int newHealth)
    {
        // normalize health value
        float normalizedHealth = (float) newHealth / healthComponent.GetMaxHealth();
        progressBar.Value = normalizedHealth;
    }
}
