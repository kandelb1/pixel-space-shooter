using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TorpedoShip : RigidBody2D
{
    private const float ROTATION_SPEED = 0.01f;
    
    [Export] private PackedScene torpedoScene;
    [Export] private string deathSoundPath;

    private AnimatedSprite2D ship;
    private AnimatedSprite2D engine;
    private AnimatedSprite2D shield;

    private bool dead;

    private List<Vector2> firePoints;
    private int firePointIndex;
    private bool firedAllTorpedoes;
    
    private Node2D uiNode;
    
    private HealthComponent healthComponent;
    private HurtboxComponent hurtboxComponent;
    private SightRadiusComponent sightRadiusComponent;
    private ShieldComponent shieldComponent;

    private Node2D player;

    public override void _Ready()
    {
        ship = GetNode<AnimatedSprite2D>("BaseShip");
        ship.FrameChanged += HandleFrameChanged;
        engine = GetNode<AnimatedSprite2D>("BaseShip/Engine");
        shield = GetNode<AnimatedSprite2D>("BaseShip/Shield");
        shield.Hide();

        firePoints = GetNode("TorpedoFirePoints").GetChildren().Cast<Node2D>().Select(x => x.Position).ToList();

        uiNode = GetNode<Node2D>("UI");
        
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        healthComponent.HealthZero += Destroy;
        hurtboxComponent = GetNode<HurtboxComponent>("HurtboxComponent");
        sightRadiusComponent = GetNode<SightRadiusComponent>("SightRadiusComponent");
        sightRadiusComponent.EnteredSightRadius += HandleEnemyInRange;
        // sightRadiusComponent.ExitedSightRadius += HandleEnemyExitedRange;
        shieldComponent = GetNode<ShieldComponent>("BaseShip/Shield/ShieldComponent");
        shieldComponent.Monitoring = false;

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
        if (ship.Animation == "shoot" || firedAllTorpedoes)
        {
            state.AngularVelocity *= ROTATION_SPEED;
        }
    }
    
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        Vector2 targetPos = player.Position;
        LookFollow(state, targetPos);
        LinearVelocity = new Vector2(0, -1).Rotated(Rotation) * 50f;
    }

    private async void Destroy()
    {
        if (dead) return;
        dead = true;
        engine.Hide();
        shield.Hide();
        uiNode.Hide();
        GetNode<CollisionShape2D>("HurtboxComponent/CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        ship.Play("explode");
        AudioManager.Instance.PlaySound(deathSoundPath);
        await ToSignal(ship, AnimatedSprite2D.SignalName.AnimationFinished);
        GameEventBus.Instance.EmitSignal(GameEventBus.SignalName.EnemyDestroyed, this);
        QueueFree();
    }

    private void HandleFrameChanged()
    {
        if (ship.Animation != "shoot") return;
        if (ship.Frame < 4) return;
        if (ship.Frame % 2 == 0)
        {
            TorpedoProjectile torpedo = torpedoScene.Instantiate<TorpedoProjectile>();
            torpedo.SetStartPosition(ToGlobal(firePoints[firePointIndex]));
            torpedo.SetStartRotation(Rotation);
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
        firedAllTorpedoes = true;
        ship.Play("idle_unloaded");
        shield.Show();
        shield.Play();
        await ToSignal(shield, AnimatedSprite2D.SignalName.AnimationFinished);
        shieldComponent.Monitoring = true;
    }

    // private void ActivateShield()
    // {
    //     shield.Show();
    //     shield.Play();
    // }
}
