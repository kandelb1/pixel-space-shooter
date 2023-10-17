using Godot;
using System;

public partial class BigBullet : RigidBody2D
{
    private Vector2 startPos;
    private float startRot;
    public void SetStartPosition(Vector2 startPos) => this.startPos = startPos;

    public void SetStartRotation(float rotation) => startRot = rotation;

    public override void _Ready()
    {
        Position = startPos;
        Rotation = startRot;
        LinearVelocity = new Vector2(0, -1).Rotated(Rotation) * 300f;
    }
}
