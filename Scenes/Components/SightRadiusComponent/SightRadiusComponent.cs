using Godot;
using System;

public partial class SightRadiusComponent : Area2D
{
    [Signal]
    public delegate void EnteredSightRadiusEventHandler(Node2D node);

    [Signal]
    public delegate void ExitedSightRadiusEventHandler(Node2D node);
    
    // private CircleShape2D circle;

    public override void _Ready()
    {
        // circle = (CircleShape2D) GetNode<CollisionShape2D>("CollisionShape2D").Shape;
        // circle.Radius = _radius;
        // GD.Print($"Set sight radius circle to {radius}");
        BodyEntered += HandleBodyEntered;
        BodyExited += HandleBodyExited;
    }

    private void HandleBodyEntered(Node2D body)
    {
        GD.Print($"Body entered SightRadiusComponent: {body.Name}");
        EmitSignal(SignalName.EnteredSightRadius, body);
    }

    private void HandleBodyExited(Node2D body)
    {
        GD.Print($"Body exited SightRadiusComponent: {body.Name}");
        EmitSignal(SignalName.ExitedSightRadius, body);
    }
    
    
}
