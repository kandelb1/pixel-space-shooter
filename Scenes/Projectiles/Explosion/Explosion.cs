using Godot;
using System;
using Godot.Collections;

public partial class Explosion : AnimatedSprite2D
{
    [Export] private int explosionDamage = 4;
    [Export] private string explosionSoundPath;
    
    private Area2D explosionArea;
    private bool appliedDamage;
    
    public override void _Ready()
    {
        AnimationFinished += HandleAnimationFinished;
        explosionArea = GetNode<Area2D>("ExplosionArea");
        AudioManager.Instance.PlaySound(explosionSoundPath);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (appliedDamage) return;
        Array<Area2D> areas = explosionArea.GetOverlappingAreas();
        foreach (Area2D area in areas)
        {
            if (area is not HurtboxComponent hurtbox) return;
            hurtbox.ApplyDamage(explosionDamage);
        }
        appliedDamage = true;
    }

    private void HandleAnimationFinished()
    {
        QueueFree();
    }
}
