using Godot;
using System;

public partial class BigCannonWeapon : BaseWeapon
{
    [Export] private PackedScene bigCannonProjectileScene;
    [Export] private RigidBody2D ship;
    [Export] private AudioStream shootSound;
    
    private AnimatedSprite2D animSprite;
    private Vector2 firePoint;
    
    private int currentAmmo = 3;

    private Sprite2D crosshair;
    private Label ammoLabel;
    
    private AudioStreamPlayer audioPlayer;

    public override void _Ready()
    {
        animSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animSprite.FrameChanged += HandleFrameChanged;
        animSprite.AnimationFinished += HandleAnimationFinished;
        firePoint = GetNode<Node2D>("FirePoint").Position;
        crosshair = GetNode<Sprite2D>("Crosshair");
        ammoLabel = GetNode<Label>("Crosshair/AmmoLabel");
        audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        audioPlayer.Stream = shootSound;
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsEquipped()) return;
        if (@event.IsActionPressed("left_click") && currentAmmo > 0)
        {
            animSprite.Play();    
        }
    }
    
    public override void _Process(double delta)
    {
        if (!IsEquipped()) return;
        crosshair.Position = GetGlobalMousePosition();
        ammoLabel.Text = currentAmmo.ToString();
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
            // add recoil
            ship.ApplyImpulse(new Vector2(0, 1).Rotated(ship.Rotation) * 250f);
            currentAmmo--;
            audioPlayer.Play();
        }
    }

    private void HandleAnimationFinished()
    {
        animSprite.Frame = 0; 
    }

    public override Texture2D GetWeaponImage() => animSprite.SpriteFrames.GetFrameTexture("default", 0);

    public void AddAmmo(int amount) => currentAmmo += amount;
}
