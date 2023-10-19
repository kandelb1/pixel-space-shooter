using Godot;
using System;
using System.Linq;

public partial class Minimap : ColorRect
{
    [Export] private WorldBoundary worldBoundary;

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        // draw player position
        Node2D player = (Node2D) GetTree().GetFirstNodeInGroup("player");
        DrawCircle(ConvertToCanvasCoords(player.Position), 1f, Colors.Aqua);
        
        // draw enemy positions
        GetTree().GetNodesInGroup("enemies").Cast<Node2D>().ToList().ForEach(enemy =>
        {
            DrawCircle(ConvertToCanvasCoords(enemy.Position), 1f, Colors.Red);
        });
    }

    private Vector2 ConvertToCanvasCoords(Vector2 globalCoords)
    {
        Vector2 worldBoundaryCoords = new Vector2(globalCoords.X - worldBoundary.TopLeft.X, globalCoords.Y - worldBoundary.TopLeft.Y);
        // divide by the world size to get coords between 0 and 1, which we can then multiply by this canvas item's rect size to get the correct position
        return worldBoundaryCoords / worldBoundary.WorldSize * Size;
    }
}
