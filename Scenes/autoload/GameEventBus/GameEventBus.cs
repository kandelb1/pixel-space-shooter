using Godot;
using System;

public partial class GameEventBus : Node
{
    public static GameEventBus Instance { get; private set; }

    [Signal]
    public delegate void HomePlanetDestroyedEventHandler();

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
