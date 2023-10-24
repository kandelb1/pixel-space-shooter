using Godot;
using System;

public partial class RocketProjectile : Node2D
{
    [Export] private string explosionSoundPath;
    
    private const float ACCELERATION = 500f;

    private ProjectileHitboxComponent hitboxComponent;
    
    private VelocityComponent velocityComponent;
    private Vector2 startPosition;
    private float startRotation;
    private Vector2 initialVelocity;
    
    private Node2D target;

    public void SetStartPosition(Vector2 pos) => startPosition = pos;
    
    public void SetStartRotation(float rotation) => startRotation = rotation;

    public void SetInitialVelocity(Vector2 velocity) => initialVelocity = velocity;

    public void SetTarget(Node2D target) => this.target = target;

    public override void _Ready()
    {
        Rotation = startRotation;
        Position = startPosition;
        hitboxComponent = GetNode<ProjectileHitboxComponent>("ProjectileHitboxComponent");
        hitboxComponent.AreaEntered += HandleAreaEntered;
        velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
        velocityComponent.SetVelocity(new Vector2(0, -1).Rotated(Rotation) * initialVelocity.Length());
        velocityComponent.SetAcceleration(new Vector2(0, -1).Rotated(Rotation) * ACCELERATION);
    }

    private void HandleAreaEntered(Area2D other)
    {
        AudioManager.Instance.PlaySound(explosionSoundPath);
    }

    public override void _Process(double delta)
    {
        if (target != null && IsInstanceValid(target))
        {
            // look at target
            Vector2 dirToTarget = (target.Position - Position).Normalized();
            Rotation = dirToTarget.Angle() + (Mathf.Pi / 2);
            velocityComponent.SetAcceleration(new Vector2(0, -1).Rotated(Rotation) * ACCELERATION);
        }
        Position += velocityComponent.Velocity * (float) delta;
    }
}
