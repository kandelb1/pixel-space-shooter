using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Ship : RigidBody2D
{

    private const int MOVE_SPEED = 300;

    [Export] private PackedScene rocketProjectile;

    private Vector2 firePoint;
    private AnimatedSprite2D engineEffects;

    private List<BaseWeapon> weapons;
    private BaseWeapon equippedWeapon;
    private int equippedIndex;

    private HealthComponent healthComponent;

    private AnimationPlayer animPlayer;

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

        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
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
            // state.ApplyForce(new Vector2(-MOVE_SPEED, 0 ));
            moving = true;
        }

        if (Input.IsActionPressed("move_right"))
        {
            state.ApplyForce(new Vector2(MOVE_SPEED, 0).Rotated(Rotation));
            // state.ApplyForce(new Vector2(MOVE_SPEED, 0 ));
            moving = true;
        }
        engineEffects.Play(moving ? "moving" : "idle");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("change_weapon"))
        {
            equippedWeapon.SetEquipped(false); // unequip current weapon
            equippedIndex += 1;
            if (equippedIndex >= weapons.Count) equippedIndex = 0;
            equippedWeapon = weapons[equippedIndex];
            equippedWeapon.SetEquipped(true);
        }
        // if (@event.IsActionPressed("fire"))
        // {
        //     RocketProjectile rocket = rocketProjectile.Instantiate<RocketProjectile>();
        //     rocket.SetPosition(ToGlobal(firePoint));
        //     rocket.SetRotation(Rotation);
        //     rocket.SetInitialVelocity(LinearVelocity);
        //     GetNode("/root").AddChild(rocket);
        // }
    }

    private async void HandleHealthChanged(int newHealth)
    {
        // healthComponent.SetInvulnerable(true);
        // animPlayer.Play("damage_flash");
        // await ToSignal(animPlayer, AnimationPlayer.SignalName.AnimationFinished);
        // healthComponent.SetInvulnerable(false);
    }
}
