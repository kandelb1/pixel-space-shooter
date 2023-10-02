using Godot;
using System;

public partial class TorpedoProjectile : RigidBody2D
{
    private Vector2 startPos;
    private float startRotation;

    public void SetPosition(Vector2 pos) => startPos = pos;
    
    public void SetRotation(float rotation) => startRotation = rotation;

    public override void _Ready()
    {
        Position = startPos;
        Rotation = startRotation;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        Vector2 forward = new Vector2(0, -1).Rotated(Rotation); // 0, -1 because the rocket is facing up when its rotation is at 0 degrees
        ApplyForce(forward * 300f);
    }
}
