using Godot;
using System;

public partial class HealthComponent : Node
{

    [Signal]
    public delegate void HealthChangedEventHandler(int newHealth); // TODO: rename to DamageTaken?

    [Signal]
    public delegate void HealthZeroEventHandler();

    [Export] private int maxHealth = 10;
    private int currentHealth;
    
    private bool invulnerable;

    public override void _Ready()
    {
        currentHealth = maxHealth;
    }

    public int GetMaxHealth() => maxHealth;

    public int GetCurrentHealth() => currentHealth;

    public bool IsInvulnerable() => invulnerable;

    public void SetInvulnerable(bool invulnerable) => this.invulnerable = invulnerable;

    public void Damage(int amount)
    {
        if (invulnerable) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            EmitSignal(SignalName.HealthZero);
        }
        EmitSignal(SignalName.HealthChanged, currentHealth);
    }
}
