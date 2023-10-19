using Godot;
using System;

public partial class WorldBoundary : Node2D
{
    public Vector2 TopLeft { get; private set; }
    public Vector2 BottomRight { get; private set; }
    public Rect2 WorldSize { get; private set; }

    public override void _Ready()
    {
        TopLeft = GetNode<Node2D>("TopLeft").Position;
        BottomRight = GetNode<Node2D>("BottomRight").Position;
        WorldSize = new Rect2(TopLeft, BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y);
        GD.Print($"Top Left: {TopLeft}");
        GD.Print($"Bottom Right: {BottomRight}");
        GD.Print($"World Rect is {WorldSize}");
    }

    // public Vector2 GetNormalizedPosition(Vector2 pos)
    // {
    //     
    // }
}
