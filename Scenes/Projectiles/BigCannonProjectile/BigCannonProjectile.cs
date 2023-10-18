using Godot;
using System;

public partial class BigCannonProjectile : RigidBody2D
{
    [Export] private PackedScene explosionScene;
    
    private float startRotation;
    private Vector2 startPosition;
    private Area2D area;

    public void SetStartRotation(float rotation) => startRotation = rotation;

    public void SetStartPosition(Vector2 pos) => startPosition = pos;
    
    public override void _Ready()
    {
        Rotation = startRotation;
        Position = startPosition;
        LinearVelocity = new Vector2(0, -1).Rotated(Rotation) * 150f;
        area = GetNode<Area2D>("Area2D");
        area.AreaEntered += HandleAreaEntered;
    }

    private void HandleAreaEntered(Area2D other)
    {
        GD.Print("Big projectile encountered something, spawning explosion");
        CallDeferred(MethodName.SpawnExplosion);
        QueueFree();
    }

    private void SpawnExplosion()
    {
        Node2D explosion = explosionScene.Instantiate<Node2D>();
        explosion.Position = Position;
        GetNode("/root").AddChild(explosion);
    }
}
