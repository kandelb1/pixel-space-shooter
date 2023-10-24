using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class RocketWeapon : BaseWeapon
{
    [Export] private PackedScene rocketProjectileScene;
    [Export] private RigidBody2D ship;
    [Export] private PackedScene targetLockScene;
    [Export] private AudioStream shootSound;
    
    private Timer timer;

    private int currentAmmo = 6;
    private List<AnimatedSprite2D> rockets;
    private int fireIndex = 0;

    private AtlasTexture weaponImage;

    private Sprite2D crosshair;
    private Label ammoLabel;

    private Node targetsNode;
    private bool targeting;
    private List<Node2D> targets;
    private int maxTargets;
    private Area2D targetDetector;
    
    private AudioStreamPlayer audioPlayer;

    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer.Timeout += HandleTimeout;
        timer.Start();
        rockets = GetNode("Rockets").GetChildren().Cast<AnimatedSprite2D>().ToList();

        weaponImage = new AtlasTexture();
        weaponImage.Atlas = GD.Load<Texture2D>("res://Assets/Player/Main Ship - Weapons - Rockets.png");
        weaponImage.Region = new Rect2(0, 0, 48, 48);

        crosshair = GetNode<Sprite2D>("Crosshair");
        ammoLabel = GetNode<Label>("Crosshair/AmmoLabel");

        targetsNode = GetNode("Targets");
        targets = new List<Node2D>();
        targetDetector = GetNode<Area2D>("Crosshair/Area2D");
        
        audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        audioPlayer.Stream = shootSound;
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsEquipped()) return;
        if (@event.IsActionPressed("left_click"))
        {
            SetMaxTargets(currentAmmo);
            SetTargeting(true);
        }
        if (@event.IsActionReleased("left_click"))
        {
            if (targets.Count == 0)
            {
                SetTargeting(false);
                targeting = false;
                return;
            }
            foreach (Node2D target in targets)
            {
                rockets[fireIndex].Play();
                audioPlayer.Play();
                RocketProjectile rocket = rocketProjectileScene.Instantiate<RocketProjectile>();
                Vector2 position = rockets[fireIndex].Position + new Vector2(0, -10);
                rocket.SetStartPosition(ToGlobal(position));
                rocket.SetStartRotation(ship.Rotation);
                rocket.SetTarget(target);
                GetNode("/root").AddChild(rocket);
                currentAmmo--;
                fireIndex++;
                timer.Start(); // restart the reload timer
            }
            SetTargeting(false);
        }
    }

    public override void _Process(double delta)
    {
        if (!IsEquipped()) return;
        crosshair.Position = GetGlobalMousePosition();
        ammoLabel.Text = $"{currentAmmo}/6";
    }

    private void HandleTimeout()
    {
        // reload a rocket
        fireIndex--;
        if (fireIndex < 0)
        {
            fireIndex = 0;
            return;
        }
        rockets[fireIndex].Stop();
        rockets[fireIndex].Frame = 0;
        currentAmmo++;
    }

    public override Texture2D GetWeaponImage() => weaponImage;
    
    public override void _PhysicsProcess(double delta)
    {
        if (!IsTargeting()) return;
        
        UpdateTargetLocks();

        foreach (Node2D potentialTarget in targetDetector.GetOverlappingBodies())
        {
            if (targets.Contains(potentialTarget)) continue;
            if (targets.Count >= maxTargets) continue;
            TrackTarget(potentialTarget);
        }
    }

    public bool IsTargeting() => targeting;

    public void SetTargeting(bool targeting)
    {
        this.targeting = targeting;
        targetDetector.Monitoring = targeting;
        ClearTargets();
    }

    public void SetMaxTargets(int max)
    {
        maxTargets = max;
    }

    private void UpdateTargetLocks()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            Node2D enemy = targets[i];
            TargetLock target = targetsNode.GetChild<TargetLock>(i);
            target.Position = enemy.Position;
        }
    }

    private void TrackTarget(Node2D target)
    {
        TargetLock targetLock = targetLockScene.Instantiate<TargetLock>();
        targetLock.Position = target.Position;
        targetsNode.AddChild(targetLock);
        targets.Add(target);
    }

    private void ClearTargets()
    {
        targets.Clear();
        foreach (Node child in targetsNode.GetChildren())
        {
            child.QueueFree();
        }
    }

    public void RefillAmmo()
    {
        foreach (AnimatedSprite2D sprite in rockets)
        {
            sprite.Stop();
            sprite.Frame = 0; // TODO: check if this is necessary. I think Stop() resets the frame to 0
        }
        fireIndex = 0;
        currentAmmo = 6;
    }
}
