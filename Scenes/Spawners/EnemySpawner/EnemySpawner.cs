using Godot;
using System;

public partial class EnemySpawner : AnimatedSprite2D
{
    [Export] private PackedScene scoutShipScene;

    private Timer spawnTimer;
    private TextureProgressBar progressBar;

    public override void _Ready()
    {
        spawnTimer = GetNode<Timer>("SpawnTimer");
        spawnTimer.Timeout += SpawnEnemy;
        progressBar = GetNode<TextureProgressBar>("SpawnProgressBar");
    }

    public override void _Process(double delta)
    {
        double normalizedTimerVal = 1 - (spawnTimer.TimeLeft / spawnTimer.WaitTime);
        progressBar.Value = normalizedTimerVal;
    }

    private void SpawnEnemy()
    {
        // lets do scout ships for now
        ScoutShip enemy = scoutShipScene.Instantiate<ScoutShip>();
        enemy.Position = Position;
        GetNode("/root").AddChild(enemy);
    }
}
