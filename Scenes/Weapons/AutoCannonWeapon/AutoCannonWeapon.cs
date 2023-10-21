using Godot;
using System;

public partial class AutoCannonWeapon : BaseWeapon
{
    [Export] private PackedScene autoCannonProjectile; // TODO: be consistent with PackedScene naming: add 'scene' to the end
    [Export] private RigidBody2D ship;
    // [Export] private Texture2D crosshair;

    private const float MAX_ANIMATION_SPEED = 7f; // seems like good stopping point
    
    private AnimatedSprite2D animSprite;
    private Vector2 firePointLeft;
    private Vector2 firePointRight;

    private Sprite2D crosshair;

    public override void _Ready()
    {
        animSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animSprite.FrameChanged += HandleFrameChanged;
        firePointLeft = GetNode<Node2D>("FirePointLeft").Position;
        firePointRight = GetNode<Node2D>("FirePointRight").Position;
        crosshair = GetNode<Sprite2D>("Crosshair");
    }

    public override void _Process(double delta)
    {
        if (!IsEquipped()) return;
        crosshair.Position = GetGlobalMousePosition(); // crosshair has top_level set to true, so this works
    }

    private void HandleFrameChanged()
    {
        if (!IsEquipped()) return;
        if (animSprite.Frame is 1 or 2)
        {
            AutoCannonProjectile bullet = autoCannonProjectile.Instantiate<AutoCannonProjectile>();
            bullet.SetStartPosition(ToGlobal(animSprite.Frame == 1 ? firePointLeft : firePointRight));
            bullet.SetStartRotation(ship.Rotation);
            GetNode("/root").AddChild(bullet);
        }
    }

    public override Texture2D GetWeaponImage() => animSprite.SpriteFrames.GetFrameTexture("default", 0);

    public void IncreaseFiringSpeed(float percentage)
    {
        float newSpeed = animSprite.SpeedScale += percentage;
        animSprite.SpeedScale = Mathf.Min(newSpeed, MAX_ANIMATION_SPEED);
    }
}
