using Godot;
using System;

public partial class PlayerHealthBar : HBoxContainer
{

    [Export] private Ship playerShip;
    private HealthComponent healthComponent;

    private TextureProgressBar progressBar;
    
    public override void _Ready()
    {
        progressBar = GetNode<TextureProgressBar>("TextureProgressBar");
        healthComponent = playerShip.GetNode<HealthComponent>("HealthComponent");
        healthComponent.HealthChanged += HandleHealthChanged;
        HandleHealthChanged(healthComponent.GetCurrentHealth());
    }

    private void HandleHealthChanged(int newHealth)
    {
        float normalizedHealth = (float) newHealth / healthComponent.GetMaxHealth();
        progressBar.Value = normalizedHealth;
    }
}
