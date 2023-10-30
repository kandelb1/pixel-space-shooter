using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// TODO: spell Battlecruiser correctly lol
public partial class BattlescruiserShip : RigidBody2D
{
    [Export] private PackedScene shipToDeployScene;
    [Export] private string deathSoundPath;
    
    private AnimatedSprite2D ship;
    private AnimatedSprite2D engine;
    
    private HealthComponent healthComponent;
    private SightRadiusComponent sightRadiusComponent;
    private Node2D uiNode;
    
    private Node2D player;

    private List<Vector2> deployPoints;
    private Dictionary<int, Vector2> frameToDeployPoint; // maps animation frame indices to the deploy position (in local coords)
    private bool deployedShips;
    
    private bool dead;

    public override void _Ready()
    {
        ship = GetNode<AnimatedSprite2D>("Ship");
        ship.FrameChanged += HandleFrameChanged;
        ship.AnimationFinished += HandleAnimationFinished;
        engine = GetNode<AnimatedSprite2D>("Ship/Engine");
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        sightRadiusComponent = GetNode<SightRadiusComponent>("SightRadiusComponent");
        sightRadiusComponent.EnteredSightRadius += HandleEnemyInRange;
        sightRadiusComponent.ExitedSightRadius += HandleEnemyExitedRange;
        healthComponent.HealthZero += Destroy;
        
        uiNode = GetNode<Node2D>("UI");
        player = (Node2D) GetTree().GetFirstNodeInGroup("player");
        deployPoints = GetNode("ShipDeployPoints").GetChildren().Cast<Node2D>().Select(x => x.Position).ToList();
        
        frameToDeployPoint = new Dictionary<int, Vector2>();
        // right side
        frameToDeployPoint.Add(2, deployPoints[0]);
        frameToDeployPoint.Add(4, deployPoints[1]);
        frameToDeployPoint.Add(10, deployPoints[0]);
        frameToDeployPoint.Add(12, deployPoints[1]);
        // left side
        frameToDeployPoint.Add(17, deployPoints[2]);
        frameToDeployPoint.Add(19, deployPoints[3]);
        frameToDeployPoint.Add(25, deployPoints[2]);
        frameToDeployPoint.Add(27, deployPoints[3]);
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
        LinearVelocity = new Vector2(0, -1).Rotated(Rotation) * 20f;
    }

    private void HandleFrameChanged()
    {
        if (ship.Animation != "deploy") return;
        int frame = ship.Frame;
        if (frameToDeployPoint.TryGetValue(frame, out Vector2 deployPoint))
        {
            RigidBody2D deployShip = shipToDeployScene.Instantiate<RigidBody2D>(); // all ships are rigidbody2ds 
            deployShip.Position = ToGlobal(deployPoint);
            deployShip.Rotation = Rotation;
            Vector2 deployDir = new Vector2(Mathf.Sign(deployPoint.X), 0);
            deployShip.ApplyImpulse(deployDir.Rotated(Rotation) * 100f);
            GetNode("/root/Main/Enemies").AddChild(deployShip);
        }
    }

    private void HandleAnimationFinished()
    {
        if(ship.Animation == "deploy") ship.Play("default");
    }

    private void HandleEnemyInRange(Node2D body)
    {
        if (dead || deployedShips) return;
        ship.Play("deploy");
        deployedShips = true;
    }

    private void HandleEnemyExitedRange(Node2D body)
    {
        // if (dead) return;
        // ship.Play("default");
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
