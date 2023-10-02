using Godot;
using System;

public partial class AutoCannonWeapon : BaseWeapon
{
    [Export] private PackedScene autoCannonProjectile;
    [Export] private RigidBody2D ship;
    
    private AnimatedSprite2D animSprite;
    private Vector2 firePointLeft;
    private Vector2 firePointRight;

    public override void _Ready()
    {
        animSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animSprite.FrameChanged += HandleFrameChanged;
        firePointLeft = GetNode<Node2D>("FirePointLeft").Position;
        firePointRight = GetNode<Node2D>("FirePointRight").Position;
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
}
