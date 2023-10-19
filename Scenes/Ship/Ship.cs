using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Ship : RigidBody2D
{
    [Signal]
    public delegate void WeaponEquippedEventHandler(BaseWeapon weapon);

    private const int MOVE_SPEED = 300;
    public const float MAX_SHIELD_TIME = 5.0f;

    [Export] private PackedScene rocketProjectile;
    
    [Export] private Texture2D fullHealthImage;
    [Export] private Texture2D slightlyDamagedImage;
    [Export] private Texture2D moderatelyDamagedImage;
    [Export] private Texture2D severelyDamagedImage;

    [Export] private HomePlanet homePlanet;

    private Vector2 firePoint;
    private AnimatedSprite2D engineEffects;

    private List<BaseWeapon> weapons;
    private BaseWeapon equippedWeapon;
    private int equippedIndex;

    private HealthComponent healthComponent;

    private AnimationPlayer animPlayer;

    private Sprite2D baseShip;

    private bool dead;

    private AnimatedSprite2D shield;
    private ShieldComponent shieldComponent;
    
    // lets be inconsistent and use properties (because I want to)
    public float ShieldTimeRemaining { get; private set; } = MAX_SHIELD_TIME;
    public bool ShieldRecharging { get; private set; }
    private bool shieldActive;

    public override void _Ready()
    {
        firePoint = GetNode<Node2D>("FirePoint").Position;
        engineEffects = GetNode<AnimatedSprite2D>("BaseShip/Engine/EngineEffects");
        engineEffects.Play("idle");

        weapons = GetNode("BaseShip/Weapons").GetChildren().Cast<BaseWeapon>().ToList();
        foreach (BaseWeapon weapon in weapons)
        {
            weapon.SetEquipped(false);
        }
        equippedWeapon = weapons[0];
        equippedIndex = 0;
        equippedWeapon.SetEquipped(true);

        healthComponent = GetNode<HealthComponent>("HealthComponent");
        healthComponent.HealthChanged += HandleHealthChanged;
        healthComponent.HealthZero += HandleHealthZero;

        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        baseShip = GetNode<Sprite2D>("BaseShip");
        baseShip.Texture = fullHealthImage;

        shield = GetNode<AnimatedSprite2D>("BaseShip/Shield");
        shield.Visible = shieldActive;
        shieldComponent = GetNode<ShieldComponent>("BaseShip/Shield/ShieldComponent");
        shieldComponent.ToggleShield(shieldActive);
    }
    
    private void LookFollow(PhysicsDirectBodyState2D state, Vector2 targetPosition)
    {
        // Calculate the direction from the current position to the target
        Vector2 dirToTarget = (targetPosition - Position).Normalized();

        // Calculate the angle to rotate
        float rotationAngle = Mathf.Atan2(dirToTarget.Y, dirToTarget.X) + (Mathf.Pi / 2) - Rotation;

        // Normalize the angle to the range [-pi, pi]
        if (rotationAngle > Mathf.Pi)
        {
            rotationAngle -= Mathf.Pi * 2;
        }
        else if (rotationAngle < -Mathf.Pi)
        {
            rotationAngle += Mathf.Pi * 2;
        }

        // Set the angular velocity to rotate towards the target
        state.AngularVelocity = rotationAngle / state.Step;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (dead) // unfortunately we can't set the position of a rigidbody2d anytime we want. we have to do it inside of _IntegrateForces()
        {
            healthComponent.Heal(healthComponent.GetMaxHealth());
            state.Transform = new Transform2D(0, homePlanet.GetSpawnPosition());
            dead = false;
            Show();
            return;
        }
        LookFollow(state, GetGlobalMousePosition());
        bool moving = false;
        
        if (Input.IsActionPressed("move_up"))
        {
            state.ApplyForce(new Vector2(0, -MOVE_SPEED).Rotated(Rotation));
            moving = true;
        }
        
        if (Input.IsActionPressed("move_down"))
        {
            state.ApplyForce(new Vector2(0, MOVE_SPEED).Rotated(Rotation));
            moving = true;
        }

        if (Input.IsActionPressed("move_left"))
        {
            state.ApplyForce(new Vector2(-MOVE_SPEED, 0).Rotated(Rotation));
            moving = true;
        }

        if (Input.IsActionPressed("move_right"))
        {
            state.ApplyForce(new Vector2(MOVE_SPEED, 0).Rotated(Rotation));
            moving = true;
        }
        engineEffects.Play(moving ? "moving" : "idle");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("change_weapon"))
        {
            equippedWeapon.SetEquipped(false);
            equippedIndex += 1;
            if (equippedIndex >= weapons.Count) equippedIndex = 0;
            equippedWeapon = weapons[equippedIndex];
            equippedWeapon.SetEquipped(true);
            EmitSignal(SignalName.WeaponEquipped, equippedWeapon);
        }

        if (@event.IsAction("activate_shield"))
        {
            if (ShieldRecharging) return;
            shieldActive = @event.IsPressed();
            ToggleShield(shieldActive);
        }
    }

    public override void _Process(double delta)
    {
        if (shieldActive)
        {
            ShieldTimeRemaining -= (float) delta;
            if (ShieldTimeRemaining <= 0)
            {
                ShieldRecharging = true;
                shieldActive = false;
                ToggleShield(false);
            }
        }else
        {
            ShieldTimeRemaining += (float) delta;
            if (ShieldTimeRemaining >= MAX_SHIELD_TIME)
            {
                ShieldRecharging = false;
                ShieldTimeRemaining = MAX_SHIELD_TIME;
            }
        }
    }

    private void ToggleShield(bool active)
    {
        shield.Visible = active;
        shieldComponent.ToggleShield(active);
    }

    private async void ActivateDamageInvulnerability()
    {
        healthComponent.SetInvulnerable(true);
        animPlayer.Play("damage_flash");
        await ToSignal(animPlayer, AnimationPlayer.SignalName.AnimationFinished);
        healthComponent.SetInvulnerable(false);   
    }

    private void UpdateShipSprite(int currentHealth, int maxHealth)
    {
        float normalizedHealth = (float) currentHealth / maxHealth;
        switch (normalizedHealth)
        {
            case > 0 and < .25f:
                // GD.Print("less than 25% health remaining");
                baseShip.Texture = severelyDamagedImage;
                break;
            case >= .25f and < .75f:
                // GD.Print("between 25% and 75% health");
                baseShip.Texture = moderatelyDamagedImage;
                break;
            case >= .75f and < 1f:
                // GD.Print("between 75% and 100% health");
                baseShip.Texture = slightlyDamagedImage;
                break;
            case 1f:
                // GD.Print("at full health");
                baseShip.Texture = fullHealthImage;
                break;
        }
    }

    private void HandleHealthChanged(int newHealth)
    {
        ActivateDamageInvulnerability();
        UpdateShipSprite(newHealth, healthComponent.GetMaxHealth());
    }

    private void HandleHealthZero()
    {
        dead = true;
        Hide();
    }

    public BaseWeapon GetEquippedWeapon() => equippedWeapon;
}
