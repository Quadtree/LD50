using System;
using Godot;

public class Drone : RigidBody
{
    public override void _Ready()
    {
        Connect("body_entered", this, nameof(OnCollision));
    }

    public override void _Process(float delta)
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        Ship.ApplyGravityAndDrag(this, delta);

        var target = GetTree().CurrentScene.FindChildByType<Ship>();
        if (target != null)
        {
            var desiredVel = (target.GetGlobalLocation() - this.GetGlobalLocation()).Normalized() * 9;

            var diffFromCurrent = desiredVel - LinearVelocity;

            if (diffFromCurrent.Length() >= 0.5f)
            {
                var thrustVector = diffFromCurrent.Normalized() * 4 * delta;

                ApplyCentralImpulse(thrustVector);
            }
        }
    }

    void OnCollision(Node other)
    {
        if (other is Ship)
        {
            ((Ship)other).Death();
        }

        Util.SpawnOneShotSound("res://sounds/destroyed.wav", this, this.GetGlobalLocation());

        Minimap.DeleteFromMinimap(this);
        QueueFree();
    }
}
