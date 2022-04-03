using System;
using Godot;

public class Drone : RigidBody
{
    public override void _Ready()
    {

    }

    public override void _Process(float delta)
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        Ship.ApplyGravityAndDrag(this, delta);
    }
}
