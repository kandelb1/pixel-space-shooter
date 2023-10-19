using Godot;
using System;

public partial class EnemySpawner : AnimatedSprite2D
{
    [Export] private PackedScene shipScene;
    [Export] private float spawnTime = 3f;

    private Timer spawnTimer;
    private TextureProgressBar progressBar;

    public override void _Ready()
    {
        spawnTimer = GetNode<Timer>("SpawnTimer");
        spawnTimer.WaitTime = spawnTime;
        spawnTimer.Timeout += SpawnEnemy;
        spawnTimer.Start();
        progressBar = GetNode<TextureProgressBar>("SpawnProgressBar");
    }

    public override void _Process(double delta)
    {
        double normalizedTimerVal = 1 - (spawnTimer.TimeLeft / spawnTimer.WaitTime);
        progressBar.Value = normalizedTimerVal;
    }

    private void SpawnEnemy()
    {
        RigidBody2D enemy = shipScene.Instantiate<RigidBody2D>();
        enemy.Position = Position;
        GetNode("/root").AddChild(enemy);
    }
}
