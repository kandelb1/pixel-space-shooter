using Godot;
using System;

public partial class ShieldComponent : Area2D
{
    public override void _Ready()
    {
        AreaEntered += HandleAreaEntered;
    }

    public void ToggleShield(bool active)
    {
        Monitoring = active;
        Monitorable = active;
    }

    private void HandleAreaEntered(Area2D other)
    {
        // basic shield that deletes any projectile that enters it
        if (other is ProjectileHitboxComponent projectileHitbox)
        {
            projectileHitbox.Owner.Free();    
        }
    }
}
