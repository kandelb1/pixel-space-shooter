using Godot;
using System;
using System.Collections.Generic;

public partial class PowerupList : VBoxContainer
{
    [Export] private PackedScene powerupListItemScene;
    
    public override void _Ready()
    {
        GameEventBus.Instance.PowerupAcquired += HandlePowerupAcquired;
    }

    public override void _ExitTree()
    {
        // since PowerupAcquired is a regular C# Action and not a Godot signal, we have to handle unsubscribing ourselves
        GameEventBus.Instance.PowerupAcquired -= HandlePowerupAcquired;
    }
    
    private void HandlePowerupAcquired(IPowerup powerup)
    {
        PowerupListItem listItem = powerupListItemScene.Instantiate<PowerupListItem>();
        listItem.SetPowerup(powerup);
        AddChild(listItem);
    }
}
