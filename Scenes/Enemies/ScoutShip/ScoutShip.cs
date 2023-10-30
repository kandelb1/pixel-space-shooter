using Godot;

public partial class ScoutShip : RigidBody2D
{
    [Export] private PackedScene bullet;
    [Export] private string deathSoundPath;
    [Export] private string shootSoundPath;

    private AnimatedSprite2D ship;
    private AnimatedSprite2D engine;

    private Vector2 firePoint;
    private bool dead;

    private HealthComponent healthComponent;

    private Node2D uiNode;

    private HurtboxComponent hurtboxComponent;

    private SightRadiusComponent sightRadiusComponent;

    public override void _Ready()
    {
        ship = GetNode<AnimatedSprite2D>("Ship");
        // ship.AnimationFinished += HandleAnimationFinished;
        ship.FrameChanged += HandleFrameChanged;
        engine = GetNode<AnimatedSprite2D>("Ship/Engine");

        firePoint = GetNode<Node2D>("FirePoint").Position;

        healthComponent = GetNode<HealthComponent>("HealthComponent");
        healthComponent.HealthZero += Destroy;

        uiNode = GetNode<Node2D>("UI");

        hurtboxComponent = GetNode<HurtboxComponent>("HurtboxComponent");
        
        // this sight radius component is configured to only detect the player
        sightRadiusComponent = GetNode<SightRadiusComponent>("SightRadiusComponent");
        sightRadiusComponent.EnteredSightRadius += HandleEnemyInRange;
        sightRadiusComponent.ExitedSightRadius += HandleEnemyExitedRange;
    }

    // TODO: duplicate code from player's Ship class
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
        Vector2 targetPos = ((Node2D) GetTree().GetFirstNodeInGroup("player")).Position;
        LookFollow(state, targetPos);
        LinearVelocity = new Vector2(0, -1).Rotated(Rotation) * 100f;
    }

    public async void Destroy()
    {
        if (dead) return;
        dead = true;
        engine.Hide();
        uiNode.Hide();
        // disable the hurtbox so bullets can pass through it while the death animation is playing
        GetNode<CollisionShape2D>("HurtboxComponent/CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        ship.Play("explode");
        AudioManager.Instance.PlaySound(deathSoundPath);
        await ToSignal(ship, AnimatedSprite2D.SignalName.AnimationFinished);
        GameEventBus.Instance.EmitSignal(GameEventBus.SignalName.EnemyDestroyed, this);
        QueueFree();
    }

    private void HandleFrameChanged()
    {
        if (ship.Animation == "shoot" && ship.Frame == 3)
        {
            BasicBullet basicBullet = bullet.Instantiate<BasicBullet>();
            basicBullet.SetStartPosition(ToGlobal(firePoint));
            basicBullet.SetStartRotation(Rotation);
            GetNode("/root").AddChild(basicBullet); // TODO: maybe use some singleton to hold the projectiles
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
        ship.Play("default"); // default animation
    }
}
