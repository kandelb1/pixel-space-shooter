using Godot;
using System;

public partial class PowerupListItem : HBoxContainer
{
    private IPowerup powerup;

    private TextureRect icon;
    private Label label;
    
    public override void _Ready()
    {
        icon = GetNode<TextureRect>("Icon");
        label = GetNode<Label>("Label");

        icon.Texture = powerup.GetIcon();
    }

    public override void _Process(double delta)
    {
        float timeRemaining = powerup.GetTimeRemaining();
        if (timeRemaining < 0) QueueFree();
        label.Text = $"{timeRemaining:.0}";
    }

    public void SetPowerup(IPowerup powerup) => this.powerup = powerup;
}
