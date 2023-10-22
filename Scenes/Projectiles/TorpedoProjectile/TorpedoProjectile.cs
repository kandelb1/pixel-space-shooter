using Godot;
using System;

public partial class TorpedoProjectile : Node2D
{
    private const float ACCELERATION = 300f;
    
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
        velocityComponent.SetAcceleration(new Vector2(0, -1).Rotated(Rotation) * ACCELERATION);
        velocityComponent.VelocityChanged += HandleVelocityChanged;
    }
    
    public override void _Process(double delta)
    {
        Position += velocityComponent.Velocity * (float) delta;
    }

    private void HandleVelocityChanged()
    {
        Rotation = velocityComponent.Velocity.Angle() + (Mathf.Pi / 2);
        // make sure to change acceleration to match the new direction
        velocityComponent.SetAcceleration(new Vector2(0, -1).Rotated(Rotation) * ACCELERATION);
    }
}
