using Godot;
using System;

public partial class HomePlanet : AnimatedSprite2D
{
    [Export] private Ship playerShip;

    private Vector2 playerSpawnPosition;
    
    private HealthComponent healthComponent;

    public override void _Ready()
    {
        playerSpawnPosition = ToGlobal(GetNode<Node2D>("PlayerSpawnPosition").Position);
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        healthComponent.HealthZero += HandleHealthZero;
    }
    
    public Vector2 GetSpawnPosition() => playerSpawnPosition;

    private void HandleHealthZero()
    {
        GameEventBus.Instance.EmitSignal(GameEventBus.SignalName.HomePlanetDestroyed); // nothing listens to this signal yet
    }
}
