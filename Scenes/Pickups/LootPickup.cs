using Godot;
using System;

public abstract partial class LootPickup : Node2D
{
    public override void _Ready()
    {
        // all LootPickup subclasses must have an Area2D with the collision mask set to player
        Area2D area = GetNode<Area2D>("Area2D");
        area.AreaEntered += HandleAreaEntered;
    }

    private void HandleAreaEntered(Area2D other)
    {
        if (other.Owner is not Ship playerShip) return;
        OnPickup(playerShip);
        QueueFree();
    }
    
    public abstract void OnPickup(Ship playerShip);
}
