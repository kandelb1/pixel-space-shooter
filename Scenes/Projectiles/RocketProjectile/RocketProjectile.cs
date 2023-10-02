using Godot;
using System;

public partial class RocketProjectile : RigidBody2D
{
    private Vector2 startPos;
    private float startRotation;
    private Vector2 initialVelocity;
    
    private Node2D target;

    public void SetPosition(Vector2 pos) => startPos = pos;
    
    public void SetRotation(float rotation) => startRotation = rotation;

    public void SetInitialVelocity(Vector2 velocity) => initialVelocity = velocity;

    public void SetTarget(Node2D target) => this.target = target;

    public override void _Ready()
    {
        Position = startPos;
        Rotation = startRotation;
        LinearVelocity = new Vector2(0, -1).Rotated(Rotation) * initialVelocity.Length();
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
        if (target != null && IsInstanceValid(target)) LookFollow(state, target.Position);
        Vector2 forward = new Vector2(0, -1).Rotated(Rotation); // 0, -1 because the rocket is facing up when its rotation is at 0 degrees
        ApplyForce(forward * 500f);
    }
}
