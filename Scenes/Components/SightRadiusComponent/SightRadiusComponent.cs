using Godot;
using System;

public partial class SightRadiusComponent : Area2D
{
    [Signal]
    public delegate void EnteredSightRadiusEventHandler(Node2D node);

    [Signal]
    public delegate void ExitedSightRadiusEventHandler(Node2D node);

    public override void _Ready()
    {
        BodyEntered += HandleBodyEntered;
        BodyExited += HandleBodyExited;
    }

    private void HandleBodyEntered(Node2D body)
    {
        EmitSignal(SignalName.EnteredSightRadius, body);
    }

    private void HandleBodyExited(Node2D body)
    {
        EmitSignal(SignalName.ExitedSightRadius, body);
    }
    
    
}
