using Godot;
using System;

public partial class LifetimeComponent : Node
{

    // [Signal]
    // public delegate void TimeElapsedEventHandler();
    
    [Export] private double lifetimeSeconds;
    [Export] private Node nodeToDelete;
    private Timer timer;

    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer.WaitTime = lifetimeSeconds;
        timer.Timeout += HandleTimeout;
        timer.Start();
    }

    private void HandleTimeout()
    {
        nodeToDelete.QueueFree();
        // EmitSignal(SignalName.TimeElapsed);
    }

}
