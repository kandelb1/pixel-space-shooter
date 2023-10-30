using Godot;
using System;

public partial class GameEventBus : Node
{
    public static GameEventBus Instance { get; private set; }

    [Signal]
    public delegate void EnemyDestroyedEventHandler(Node2D enemy);

    [Signal]
    public delegate void PlayerDestroyedEventHandler();
    
    // we want this Action to be invokable outside of this class, so don't use the 'event' keyword 
    public Action<IPowerup> PowerupAcquired;

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.PrintErr("There is already an instance of GameEventBus");
            QueueFree();
            return;
        }
        Instance = this;
    }
    
    public override void _ExitTree()
    {
        Instance = null;
    }
}
