using Godot;
using System;

public partial class WorldBoundary : Node2D
{
    public Vector2 TopLeft { get; private set; }
    public Vector2 BottomRight { get; private set; }
    public Vector2 WorldSize { get; private set; }
    

    public override void _Ready()
    {
        TopLeft = ToGlobal(GetNode<Node2D>("TopLeft").Position);
        BottomRight = ToGlobal(GetNode<Node2D>("BottomRight").Position);
        WorldSize = new Vector2(BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y);
    }
}
