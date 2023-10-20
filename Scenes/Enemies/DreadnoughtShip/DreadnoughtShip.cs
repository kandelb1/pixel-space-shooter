using Godot;
using System;

public partial class DreadnoughtShip : RigidBody2D
{

    private const float ROTATION_SPEED = 0.13f;
    private const int MAX_DEATHRAY_DISTANCE = 370;
    
    private AnimatedSprite2D ship;
    private AnimatedSprite2D engine;
    private AnimatedSprite2D shield;
    
    private Node2D uiNode;
    
    private HealthComponent healthComponent;
    private SightRadiusComponent sightRadiusComponent;

    private Node2D player;

    // TODO: move the deathray to its own scene, so we only have to talk to 1 node instead of all of these
    private Line2D deathRay;
    private PersistentHitboxComponent deathRayHitbox;
    private CollisionShape2D deathRayCollisionShape;
    private bool playerInRange;
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
        sightRadiusComponent.ExitedSightRadius += HandleEnemyExitedRange;

        player = (Node2D) GetTree().GetFirstNodeInGroup("player");
        
        deathRay = GetNode<Line2D>("DeathRay");
        deathRayHitbox = GetNode<PersistentHitboxComponent>("DeathRay/PersistentHitboxComponent");
        deathRayCollisionShape = GetNode<CollisionShape2D>("DeathRay/PersistentHitboxComponent/CollisionShape2D");
        deathRayCooldownTimer = GetNode<Timer>("DeathRay/CooldownTimer");
        deathRayCooldownTimer.Timeout += HandleCooldownOver;
        ToggleDeathRay(false);
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
        if (ship.Animation == "shoot")
        {
            state.AngularVelocity *= ROTATION_SPEED;
        }
    }
    
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        Vector2 targetPos = player.Position;
        LookFollow(state, targetPos);
    }

    public override void _Process(double delta)
    {
        if (!deathRayFiring) return;
        // update death ray length
        UpdateDeathRay();
    }

    private void UpdateDeathRay()
    {
        int distanceToPlayer = (int) Position.DistanceTo(player.Position);
        int deathRayDistance = Mathf.Min(distanceToPlayer, MAX_DEATHRAY_DISTANCE);
        deathRay.SetPointPosition(1, new Vector2(0, -deathRayDistance));
        RectangleShape2D rect = (RectangleShape2D) deathRayCollisionShape.Shape;
        rect.Size = new Vector2(6, deathRayDistance);
        deathRayCollisionShape.Position = new Vector2(0, -deathRayDistance / 2);
    }

    private void ToggleDeathRay(bool active)
    {
        deathRay.Visible = active;
        deathRayHitbox.Monitoring = active;
        deathRayHitbox.Monitorable = active;
    }

    private async void Destroy()
    {
        if (dead) return;
        dead = true;
        ship.Play("idle"); // in case we're in the middle of firing the death ray
        ToggleDeathRay(false);
        engine.Hide();
        shield.Hide();
        uiNode.Hide();
        ship.Play("explode");
        await ToSignal(ship, AnimatedSprite2D.SignalName.AnimationFinished);
        GameEventBus.Instance.EmitSignal(GameEventBus.SignalName.EnemyDestroyed, this);
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

    private void HandleEnemyInRange(Node2D body)
    {
        if (dead) return;
        playerInRange = true;
        TryFireDeathRay();
    }

    private void HandleEnemyExitedRange(Node2D body)
    {
        playerInRange = false;
    }

    private void HandleCooldownOver()
    {
        TryFireDeathRay();
    }

    private async void TryFireDeathRay()
    {
        if (!playerInRange) return;
        if (deathRayFiring || deathRayCooldownTimer.TimeLeft > 0) return;
        ship.Play("shoot");
        deathRayFiring = true;
        await ToSignal(ship, AnimatedSprite2D.SignalName.AnimationFinished);
        ship.Play("idle");
        deathRayFiring = false;
        deathRayCooldownTimer.Start();
    }
}
