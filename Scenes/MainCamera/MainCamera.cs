using Godot;
using System;

public partial class MainCamera : Camera2D
{
    [Export] private RigidBody2D playerShip;

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        Position = playerShip.Position.Round();
        // Position = GetGlobalMousePosition().Round();
    }
}
