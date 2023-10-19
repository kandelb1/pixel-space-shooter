using Godot;
using System;

public partial class PlayerShieldBar : HBoxContainer
{
    [Export] private Ship playerShip;

    private readonly Color mutedColor = new Color(0.5f, 0.5f, 0.5f, 1);

    private TextureProgressBar progressBar;

    public override void _Ready()
    {
        progressBar = GetNode<TextureProgressBar>("TextureProgressBar");
    }

    public override void _Process(double delta)
    {
        float normalizedValue = playerShip.ShieldTimeRemaining / Ship.MAX_SHIELD_TIME;
        progressBar.Value = normalizedValue;
        Modulate = playerShip.ShieldRecharging ? mutedColor : Colors.White;
    }
}
