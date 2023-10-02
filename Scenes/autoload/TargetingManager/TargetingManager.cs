using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class TargetingManager : Node2D
{
    public static TargetingManager Instance { get; private set; }

    private const int ENEMY_COLLISION_LAYER = 4;

    [Export] private PackedScene targetLockScene;

    private bool targeting;
    private List<Node2D> targets;

    private int maxTargets;

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.PrintErr("There is already an instance of TargetingManager");
            QueueFree();
            return;
        }
        Instance = this;
        GD.Print("TargetingManager loaded");
        targets = new List<Node2D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsTargeting()) return;
        
        UpdateTargetLocks();
        
        Vector2 mousePos = GetGlobalMousePosition();
        PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
        PhysicsPointQueryParameters2D query = new PhysicsPointQueryParameters2D();
        query.Position = mousePos;
        query.CollisionMask = ENEMY_COLLISION_LAYER;
        Array<Dictionary> test = spaceState.IntersectPoint(query);
        foreach (Dictionary dict in test)
        {
            Node2D enemy = (Node2D) dict["collider"];
            if (targets.Contains(enemy)) continue;
            if (targets.Count >= maxTargets) continue;
            GD.Print($"detected {enemy.Name}");
            TrackTarget(enemy);
        }
    }

    public bool IsTargeting() => targeting;

    public void SetTargeting(bool targeting)
    {
        this.targeting = targeting;
        ClearTargets();
    }

    public List<Node2D> GetTargets() => targets;

    public void SetMaxTargets(int max)
    {
        maxTargets = max;
        GD.Print($"TargetingManager: max targets updated to {max}");
    }

    private void UpdateTargetLocks()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            Node2D enemy = targets[i];
            TargetLock target = GetChild<TargetLock>(i);
            target.Position = enemy.Position;
        }
    }

    private void TrackTarget(Node2D target)
    {
        TargetLock targetLock = targetLockScene.Instantiate<TargetLock>();
        targetLock.Position = target.Position;
        AddChild(targetLock);
        targets.Add(target);
    }

    private void ClearTargets()
    {
        targets.Clear();
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
    }
}
