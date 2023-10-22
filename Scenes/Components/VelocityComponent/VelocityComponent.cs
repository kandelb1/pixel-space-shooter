using Godot;
using System;

public partial class VelocityComponent : Node
{
    [Signal]
    public delegate void VelocityChangedEventHandler();

    public Vector2 Velocity { get; private set; }
    
    public Vector2 Acceleration { get; private set; }

    public void SetVelocity(Vector2 newVelocity)
    {
        Velocity = newVelocity;
        EmitSignal(SignalName.VelocityChanged);
    }

    public void SetAcceleration(Vector2 newAcceleration)
    {
        Acceleration = newAcceleration;
    }

    public override void _Process(double delta)
    {
        Velocity += Acceleration * (float) delta;
    }
}
