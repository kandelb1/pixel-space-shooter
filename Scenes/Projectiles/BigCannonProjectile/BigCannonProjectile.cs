using Godot;
using System;

public partial class BigCannonProjectile : RigidBody2D
{
    public float startRotation;
    public Vector2 startPosition;

    public void SetStartRotation(float rotation) => startRotation = rotation;

    public void SetStartPosition(Vector2 pos) => startPosition = pos;
    
    public override void _Ready()
    {
        Rotation = startRotation;
        Position = startPosition;
        LinearVelocity = new Vector2(0, -1).Rotated(Rotation) * 150f;
    }
}
