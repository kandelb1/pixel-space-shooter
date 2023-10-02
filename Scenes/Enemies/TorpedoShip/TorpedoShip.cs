using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TorpedoShip : RigidBody2D
{
    [Export] private PackedScene torpedoScene;

    private AnimatedSprite2D ship;
    private AnimatedSprite2D engine;

    private bool dead;

    private List<Vector2> firePoints;
    private int firePointIndex;
    private bool firedAllTorpedoes;
    
    private Node2D uiNode;
    
    private HealthComponent healthComponent;
    private HurtboxComponent hurtboxComponent;
    private SightRadiusComponent sightRadiusComponent;

    private Node2D player;

    public override void _Ready()
    {
        ship = GetNode<AnimatedSprite2D>("BaseShip");
        ship.FrameChanged += HandleFrameChanged;
        engine = GetNode<AnimatedSprite2D>("BaseShip/Engine");

        firePoints = GetNode("TorpedoFirePoints").GetChildren().Cast<Node2D>().Select(x => x.Position).ToList();

        uiNode = GetNode<Node2D>("UI");
        
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        healthComponent.HealthZero += Destroy;
        hurtboxComponent = GetNode<HurtboxComponent>("HurtboxComponent");
        sightRadiusComponent = GetNode<SightRadiusComponent>("SightRadiusComponent");
        sightRadiusComponent.EnteredSightRadius += HandleEnemyInRange;
        // sightRadiusComponent.ExitedSightRadius += HandleEnemyExitedRange;

        player = (Node2D) GetTree().GetFirstNodeInGroup("player");
    }
    
    private void LookFollow(PhysicsDirectBodyState2D state, Vector2 targetPosition)
    {
        Vector2 dirToTarget = (targetPosition - Position).Normalized();
        
        float rotationAngle = Mathf.Atan2(dirToTarget.Y, dirToTarget.X) + (Mathf.Pi / 2) - Rotation;
        
        if (rotationAngle > Mathf.Pi)
        {
            rotationAngle -= Mathf.Pi * 2;
        }
        else if (rotationAngle < -Mathf.Pi)
        {
            rotationAngle += Mathf.Pi * 2;
        }

        state.AngularVelocity = rotationAngle / state.Step;
    }
    
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        Vector2 targetPos = player.Position;
        LookFollow(state, targetPos);
    }

    private async void Destroy()
    {
        dead = true;
        engine.Hide();
        uiNode.Hide();
        ship.Play("explode");
        await ToSignal(ship, AnimatedSprite2D.SignalName.AnimationFinished);
        QueueFree();
    }

    private void HandleFrameChanged()
    {
        if (ship.Animation != "shoot") return;
        if (ship.Frame < 4) return;
        if (ship.Frame % 2 == 0)
        {
            TorpedoProjectile torpedo = torpedoScene.Instantiate<TorpedoProjectile>();
            torpedo.SetPosition(ToGlobal(firePoints[firePointIndex]));
            torpedo.SetRotation(Rotation);
            GetNode("/root").AddChild(torpedo);
            firePointIndex++;
        }
    }

    private async void HandleEnemyInRange(Node2D body)
    {
        if (firedAllTorpedoes) return;
        // fire all the torpedoes!!
        ship.Play("shoot");
        await ToSignal(ship, AnimatedSprite2D.SignalName.AnimationFinished);
        ship.Play("idle_unloaded");
        firedAllTorpedoes = true;
    }
}
