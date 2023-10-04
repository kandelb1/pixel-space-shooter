using Godot;
using System;

public partial class DreadnoughtShip : RigidBody2D
{

    private const float ROTATION_SPEED = 0.13f;
    
    private AnimatedSprite2D ship;
    private AnimatedSprite2D engine;
    private AnimatedSprite2D shield;
    
    private Node2D uiNode;
    
    private HealthComponent healthComponent;
    // private HurtboxComponent hurtboxComponent;
    private SightRadiusComponent sightRadiusComponent;
    // private ShieldComponent shieldComponent;

    private Node2D player;

    private Line2D deathRay;
    private Area2D deathRayHitbox;
    private bool deathRayFiring;
    private Timer deathRayCooldownTimer;

    private bool dead;

    public override void _Ready()
    {
        ship = GetNode<AnimatedSprite2D>("BaseShip");
        ship.FrameChanged += HandleFrameChanged;
        engine = GetNode<AnimatedSprite2D>("BaseShip/Engine");
        shield = GetNode<AnimatedSprite2D>("BaseShip/Shield");
        
        uiNode = GetNode<Node2D>("UI");

        healthComponent = GetNode<HealthComponent>("HealthComponent");
        healthComponent.HealthZero += Destroy;
        sightRadiusComponent = GetNode<SightRadiusComponent>("SightRadiusComponent");
        sightRadiusComponent.EnteredSightRadius += HandleEnemyInRange;

        player = (Node2D) GetTree().GetFirstNodeInGroup("player");
        
        deathRay = GetNode<Line2D>("DeathRay");
        deathRayHitbox = GetNode<Area2D>("DeathRay/HitboxComponent");
        ToggleDeathRay(false);
        deathRayCooldownTimer = GetNode<Timer>("DeathRay/CooldownTimer");
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

        state.AngularVelocity = rotationAngle / state.Step * ROTATION_SPEED;
    }
    
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        Vector2 targetPos = player.Position;
        LookFollow(state, targetPos);
    }

    private void ToggleDeathRay(bool active)
    {
        deathRay.Visible = active;
        deathRayHitbox.Monitoring = active;
    }

    private async void Destroy()
    {
        if (dead) return;
        dead = true;
        ship.Play("idle"); // in case we're in the middle of firing
        ToggleDeathRay(false);
        engine.Hide();
        shield.Hide();
        uiNode.Hide();
        ship.Play("explode");
        await ToSignal(ship, AnimatedSprite2D.SignalName.AnimationFinished);
        QueueFree();
    }

    private void HandleFrameChanged()
    {
        if (ship.Animation == "shoot" && ship.Frame == 12)
        {
            ToggleDeathRay(true);
        }

        if (ship.Animation == "shoot" && ship.Frame == 56)
        {
            ToggleDeathRay(false);
        }
    }

    private async void HandleEnemyInRange(Node2D body)
    {
        if (dead) return;
        if (deathRayFiring || deathRayCooldownTimer.TimeLeft > 0) return;
        ship.Play("shoot");
        deathRayFiring = true;
        await ToSignal(ship, AnimatedSprite2D.SignalName.AnimationFinished);
        ship.Play("idle");
        deathRayFiring = false;
        deathRayCooldownTimer.Start();
    }
}
