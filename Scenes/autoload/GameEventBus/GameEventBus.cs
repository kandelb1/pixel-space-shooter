using Godot;
using System;

public partial class GameEventBus : Node
{
    public static GameEventBus Instance { get; private set; }

    [Signal]
    public delegate void HomePlanetDestroyedEventHandler();

    [Signal]
    public delegate void EnemyDestroyedEventHandler(Node2D enemy);
    
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
}
