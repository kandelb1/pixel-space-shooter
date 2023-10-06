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
        // find the halfway point between the mouse position and player position
        Vector2 mousePos = GetGlobalMousePosition().Round();
        Vector2 playerPos = playerShip.Position.Round();
        Vector2 halfwayPoint = (mousePos + playerPos) / 2;
        Position = halfwayPoint;
    }
}
