using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FighterShip : RigidBody2D
{
    private const float MAX_SPEED = 300f;
    private const float ACCELERATION = 300f;
    
    [Export] private PackedScene bigBulletScene;
    [Export] private string deathSoundPath;
    [Export] private string shootSoundPath;
    
    private AnimatedSprite2D ship;
    private AnimatedSprite2D engine;
    
    private HealthComponent healthComponent;
    private SightRadiusComponent sightRadiusComponent;
    private Node2D uiNode;
    
    private Node2D player;

    private List<Vector2> firePoints;

    private bool dead;

    public override void _Ready()
    {
        ship = GetNode<AnimatedSprite2D>("Ship");
        ship.FrameChanged += HandleFrameChanged;
        engine = GetNode<AnimatedSprite2D>("Ship/Engine");
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        sightRadiusComponent = GetNode<SightRadiusComponent>("SightRadiusComponent");
        sightRadiusComponent.EnteredSightRadius += HandleEnemyInRange;
        sightRadiusComponent.ExitedSightRadius += HandleEnemyExitedRange;
        healthComponent.HealthZero += Destroy;
        
        uiNode = GetNode<Node2D>("UI");
        player = (Node2D) GetTree().GetFirstNodeInGroup("player");
        firePoints = GetNode("BulletFirePoints").GetChildren().Cast<Node2D>().Select(x => x.Position).ToList();
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
        state.ApplyForce(new Vector2(0, -1).Rotated(Rotation) * ACCELERATION);
        
    }

    private void HandleFrameChanged()
    {
        int frame = ship.Frame;
        if (ship.Animation == "shoot" && frame is 1 or 3)
        {
            BigBullet bullet = bigBulletScene.Instantiate<BigBullet>();
            bullet.SetStartPosition(ToGlobal(firePoints[(frame == 1) ? 0 : 1]));
            bullet.SetStartRotation(Rotation);
            GetNode("/root").AddChild(bullet);
            AudioManager.Instance.PlaySound(shootSoundPath);
        }
    }

    private void HandleEnemyInRange(Node2D body)
    {
        if (dead) return;
        ship.Play("shoot");
    }

    private void HandleEnemyExitedRange(Node2D body)
    {
        if (dead) return;
        ship.Play("default");
    }
    
    private async void Destroy()
    {
        if (dead) return;
        dead = true;
        engine.Hide();
        uiNode.Hide();
        GetNode<CollisionShape2D>("HurtboxComponent/CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        ship.Play("explode");
        AudioManager.Instance.PlaySound(deathSoundPath);
        await ToSignal(ship, AnimatedSprite2D.SignalName.AnimationFinished);
        GameEventBus.Instance.EmitSignal(GameEventBus.SignalName.EnemyDestroyed, this);
        QueueFree();
    }
}
