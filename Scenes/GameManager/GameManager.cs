using Godot;
using Godot.Collections;

public partial class GameManager : Node
{
    [Export] private PackedScene scoutShipScene;
    [Export] private PackedScene torpedoShipScene;
    [Export] private PackedScene dreadnoughtShipScene;
    [Export] private PackedScene battlecruiserShipScene;
    
    [Export] private WorldBoundary worldBoundary;
    [Export] private Node enemiesNode;
    [Export] private Node2D playerShip;

    private const float MIN_SPAWN_DISTANCE = 100f;

    private Timer scoutWaveTimer;
    private Timer torpedoWaveTimer;
    private Timer scaryWaveTimer;

    private RandomNumberGenerator rng;

    public override void _Ready()
    {
        scoutWaveTimer = GetNode<Timer>("ScoutWaveTimer");
        scoutWaveTimer.Timeout += SpawnScoutWave;
        torpedoWaveTimer = GetNode<Timer>("TorpedoWaveTimer");
        torpedoWaveTimer.Timeout += SpawnTorpedoWave;
        scaryWaveTimer = GetNode<Timer>("ScaryWaveTimer");
        scaryWaveTimer.Timeout += SpawnScaryWave;
        rng = new RandomNumberGenerator();
        CallDeferred(MethodName.SpawnInitialEnemies); // needs to be deferred because WorldBoundary hasn't set its boundaries yet
    }

    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < 5; i++)
        {
            RigidBody2D enemy = scoutShipScene.Instantiate<RigidBody2D>();
            enemy.Position = GetRandomPosition();
            enemiesNode.AddChild(enemy);
        }
    }

    private void SpawnScoutWave()
    {
        for (int i = 0; i < 5; i++)
        {
            RigidBody2D enemy = scoutShipScene.Instantiate<RigidBody2D>();
            enemy.Position = GetRandomPosition();
            enemiesNode.AddChild(enemy);
        }
    }

    private void SpawnTorpedoWave()
    {
        for (int i = 0; i < 3; i++)
        {
            RigidBody2D enemy = torpedoShipScene.Instantiate<RigidBody2D>();
            enemy.Position = GetRandomPosition();
            enemiesNode.AddChild(enemy);
        }
    }

    private void SpawnScaryWave()
    {
        // spawn 2 dreadnoughts and 2 battlecruisers       
        for (int i = 0; i < 2; i++)
        {
            RigidBody2D dreadnought = dreadnoughtShipScene.Instantiate<RigidBody2D>();
            dreadnought.Position = GetRandomPosition();
            enemiesNode.AddChild(dreadnought);
            RigidBody2D battlecruiser = battlecruiserShipScene.Instantiate<RigidBody2D>();
            battlecruiser.Position = GetRandomPosition();
            enemiesNode.AddChild(battlecruiser);
        }
    }

    private Vector2 GetRandomPosition()
    {
        Vector2 position = new Vector2(
            rng.RandfRange(worldBoundary.TopLeft.X, worldBoundary.BottomRight.X),
            rng.RandfRange(worldBoundary.TopLeft.Y, worldBoundary.BottomRight.Y)
        );
        if (position.DistanceTo(playerShip.Position) <= MIN_SPAWN_DISTANCE)
        {
            return GetRandomPosition(); // pick a new position
        }
        return position;
    }
}
