using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class RocketWeapon : BaseWeapon
{
    [Export] private PackedScene rocketProjectileScene;
    [Export] private RigidBody2D ship;
    
    private Timer timer;

    private int currentAmmo = 6;
    private List<AnimatedSprite2D> rockets;
    private int fireIndex = 0;

    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer.Timeout += HandleTimeout;
        timer.Start();
        rockets = GetNode("Rockets").GetChildren().Cast<AnimatedSprite2D>().ToList();
        // targets = new List<Node2D>();
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsEquipped()) return;
        if (@event.IsActionPressed("left_click"))
        {
            TargetingManager.Instance.SetMaxTargets(currentAmmo);
            TargetingManager.Instance.SetTargeting(true);
            GD.Print("TARGETING STARTED");
        }
        if (@event.IsActionReleased("left_click"))
        {
            List<Node2D> targets = TargetingManager.Instance.GetTargets();
            if (targets.Count == 0)
            {
                TargetingManager.Instance.SetTargeting(false);
                return;
            }
            foreach (Node2D target in targets)
            {
                rockets[fireIndex].Play();
                RocketProjectile rocket = rocketProjectileScene.Instantiate<RocketProjectile>();
                Vector2 position = rockets[fireIndex].Position + new Vector2(0, -10);
                rocket.SetPosition(ToGlobal(position));
                rocket.SetRotation(ship.Rotation);
                // rocket.SetInitialVelocity(ship.LinearVelocity);
                rocket.SetTarget(target);
                GetNode("/root").AddChild(rocket);
                currentAmmo--;
                fireIndex++;
                timer.Start(); // restart the reload timer
            }
            TargetingManager.Instance.SetTargeting(false);
            GD.Print("TARGETING ENDED");
        }
        // if (!IsEquipped()) return;
        // if (currentAmmo <= 0) return;
        // if (fireIndex >= rockets.Count) return;
        // if (@event.IsActionPressed("fire"))
        // {
        //     rockets[fireIndex].Play();
        //     RocketProjectile rocket = rocketProjectile.Instantiate<RocketProjectile>();
        //     Vector2 position = rockets[fireIndex].Position + new Vector2(0, -10);
        //     rocket.SetPosition(ToGlobal(position));
        //     rocket.SetRotation(ship.Rotation);
        //     rocket.SetInitialVelocity(ship.LinearVelocity);
        //     GetNode("/root").AddChild(rocket);
        //     currentAmmo--;
        //     fireIndex++;
        //     timer.Start(); // restart the reload timer
        // }
    }
    
    private void HandleTimeout()
    {
        // reload a rocket
        fireIndex--;
        if (fireIndex < 0)
        {
            fireIndex = 0;
            return;
        }
        rockets[fireIndex].Stop();
        rockets[fireIndex].Frame = 0;
        currentAmmo++;
        // TargetingManager.Instance.SetMaxTargets(currentAmmo);
    }
}
