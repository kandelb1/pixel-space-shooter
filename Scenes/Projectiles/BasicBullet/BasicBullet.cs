using Godot;
using System;

public partial class BasicBullet : Node2D
{
    private const float VELOCITY = 350f;
    
    private VelocityComponent velocityComponent;
    private float startRotation;
    private Vector2 startPosition;

    public void SetStartRotation(float rotation) => startRotation = rotation;

    public void SetStartPosition(Vector2 pos) => startPosition = pos;
    
    public override void _Ready()
    {
        Rotation = startRotation;
        Position = startPosition;
        velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
        velocityComponent.SetVelocity(new Vector2(0, -1).Rotated(Rotation) * VELOCITY);
        velocityComponent.VelocityChanged += HandleVelocityChanged;
    }

    public override void _Process(double delta)
    {
        Position += velocityComponent.Velocity * (float) delta;
    }

    private void HandleVelocityChanged()
    {
        Rotation = velocityComponent.Velocity.Angle() + (Mathf.Pi / 2);
    }
}
