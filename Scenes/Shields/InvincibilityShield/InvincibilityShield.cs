using Godot;
using System;

public partial class InvincibilityShield : AnimatedSprite2D, IPowerup
{
    private HealthComponent playerHealth;
    private float timeLeftSeconds;
    private Texture2D powerupIcon;

    public override void _Ready()
    {
        Ship player = GetParent<Ship>();
        playerHealth = player.GetNode<HealthComponent>("HealthComponent");
        playerHealth.SetInvulnerable(true);
        GameEventBus.Instance.PowerupAcquired?.Invoke(this);
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
    
    public void SetPowerupIcon(Texture2D icon) => powerupIcon = icon;

    public Texture2D GetIcon() => powerupIcon;

    public float GetTimeRemaining() => timeLeftSeconds;
}
