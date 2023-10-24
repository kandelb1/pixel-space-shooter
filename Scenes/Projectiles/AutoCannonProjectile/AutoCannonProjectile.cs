using Godot;
using System;

public partial class AutoCannonProjectile : Node2D
{
    [Export] private string explosionSoundPath;
    
    private const float VELOCITY = 400f;
    
    private ProjectileHitboxComponent hitboxComponent;
    
    private VelocityComponent velocityComponent;
    private float startRotation;
    private Vector2 startPosition;

    public void SetStartRotation(float rotation) => startRotation = rotation;

    public void SetStartPosition(Vector2 pos) => startPosition = pos;
    
    public override void _Ready()
    {
        Rotation = startRotation;
        Position = startPosition;
        hitboxComponent = GetNode<ProjectileHitboxComponent>("ProjectileHitboxComponent");
        hitboxComponent.AreaEntered += HandleAreaEntered;
        velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
        velocityComponent.SetVelocity(new Vector2(0, -1).Rotated(Rotation) * VELOCITY);
    }
    
    private void HandleAreaEntered(Area2D other)
    {
        AudioManager.Instance.PlaySound(explosionSoundPath);
    }

    public override void _Process(double delta)
    {
        Position += velocityComponent.Velocity * (float) delta;
    }
}
