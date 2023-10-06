using Godot;
using System;

public partial class BigCannonWeapon : BaseWeapon
{
    [Export] private PackedScene bigCannonProjectileScene;
    [Export] private RigidBody2D ship;
    
    private AnimatedSprite2D animSprite;
    private Vector2 firePoint;

    public override void _Ready()
    {
        animSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animSprite.FrameChanged += HandleFrameChanged;
        animSprite.AnimationFinished += HandleAnimationFinished;
        firePoint = GetNode<Node2D>("FirePoint").Position;
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsEquipped()) return;
        if (@event.IsActionPressed("left_click"))
        {
            animSprite.Play();    
        }
    }

    private void HandleFrameChanged()
    {
        if (!IsEquipped()) return;
        if (animSprite.Frame == 7)
        {
            BigCannonProjectile bullet = bigCannonProjectileScene.Instantiate<BigCannonProjectile>();
            bullet.SetStartPosition(ToGlobal(firePoint));
            bullet.SetStartRotation(ship.Rotation);
            GetNode("/root").AddChild(bullet); // TODO: find a better way to add bullets to the scene tree
            // TODO: add recoil force to the ship
        }
    }

    private void HandleAnimationFinished()
    {
        animSprite.Frame = 0; 
    }

    public override Texture2D GetWeaponImage() => animSprite.SpriteFrames.GetFrameTexture("default", 0);
}
