using Godot;
using System;

public partial class InvincibilityShield : AnimatedSprite2D
{
    private HealthComponent playerHealth;
    private float timeLeftSeconds;

    public override void _Ready()
    {
        Ship player = GetParent<Ship>();
        playerHealth = player.GetNode<HealthComponent>("HealthComponent");
        playerHealth.SetInvulnerable(true);
    }

    public override void _Process(double delta)
    {
        timeLeftSeconds -= (float) delta;
        if (timeLeftSeconds <= 0)
        {
            playerHealth.SetInvulnerable(false);
            QueueFree();
        }
    }
    
    public void IncreaseTimeLeft(float seconds) => timeLeftSeconds += seconds;
}
