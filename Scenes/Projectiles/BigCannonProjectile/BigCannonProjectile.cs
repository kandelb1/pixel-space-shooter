using Godot;
using System;

public partial class BigCannonProjectile : Node2D
{
    [Export] private PackedScene explosionScene;

    private const float VELOCITY = 150f;

    private VelocityComponent velocityComponent;
    private float startRotation;
    private Vector2 startPosition;
    private Area2D area;

    public void SetStartRotation(float rotation) => startRotation = rotation;

    public void SetStartPosition(Vector2 pos) => startPosition = pos;
    
    public override void _Ready()
    {
        Rotation = startRotation;
        Position = startPosition;
        velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
        velocityComponent.SetVelocity(new Vector2(0, -1).Rotated(Rotation) * VELOCITY);
        area = GetNode<Area2D>("Area2D");
        area.AreaEntered += HandleAreaEntered;
    }
    
    public override void _Process(double delta)
    {
        Position += velocityComponent.Velocity * (float) delta;
    }
    
    private void HandleAreaEntered(Area2D other)
    {
        CallDeferred(MethodName.SpawnExplosion);
        QueueFree();
    }

    private void SpawnExplosion()
    {
        Node2D explosion = explosionScene.Instantiate<Node2D>();
        explosion.Position = Position;
        GetNode("/root/Main/Projectiles").AddChild(explosion);
    }
}
