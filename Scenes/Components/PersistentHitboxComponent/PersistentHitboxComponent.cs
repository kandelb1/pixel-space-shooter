using Godot;
using System;
using System.Collections.Generic;

// A persistent hitbox is one that keeps applying damage each time interval to whatever objects are inside of it
public partial class PersistentHitboxComponent : Area2D
{
    // the amount of time between damage ticks
    [Export] private float timeInterval = 0.5f;
    private Timer timer;

    [Export] private int damage;

    private List<HurtboxComponent> trackedHurtboxes;

    public override void _Ready()
    {
        trackedHurtboxes = new List<HurtboxComponent>();
        timer = GetNode<Timer>("Timer");
        timer.WaitTime = timeInterval;
        timer.Timeout += HandleTimeout;
        AreaEntered += HandleAreaEntered;
        AreaExited += HandleAreaExited;
    }

    private void HandleTimeout()
    {
        foreach (HurtboxComponent hurtbox in trackedHurtboxes)
        {
            hurtbox.ApplyDamage(damage);
        }
    }
    
    // TODO: I don't think it's a problem we need to solve right now, but as it stands, the player COULD:
    // pass through the hitbox in-between time intervals and not take any damage
    // I would like it to apply damage upon entering the area, and then start the timer from there
    // the only caveat is that we need a separate timer for each tracked hurtbox
    private void HandleAreaEntered(Area2D other)
    {
        if (other is not HurtboxComponent hurtbox) return;
        trackedHurtboxes.Add(hurtbox);
    }

    private void HandleAreaExited(Area2D other)
    {
        if (other is not HurtboxComponent hurtbox) return;
        trackedHurtboxes.Remove(hurtbox);
    }
}
