using Godot;
using System;

public partial class HealthComponent : Node
{

    [Signal]
    public delegate void HealthChangedEventHandler(int newHealth);

    [Signal]
    public delegate void HealthZeroEventHandler();

    [Export] private int maxHealth = 10;
    private int currentHealth;

    public override void _Ready()
    {
        currentHealth = maxHealth;
    }

    public int GetMaxHealth() => maxHealth;

    public int GetCurrentHealth() => currentHealth;

    public void Damage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            EmitSignal(SignalName.HealthZero);
        }
        EmitSignal(SignalName.HealthChanged, currentHealth);
    }
}
