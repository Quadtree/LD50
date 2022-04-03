using System;
using Godot;

public class Crate : RigidBody
{
    public override void _Ready()
    {
        LinearVelocity = new Vector3(
            Util.random() * 7 - 3.5f,
            0,
            Util.random() * 7 - 3.5f
        );
    }

    public override void _Process(float delta)
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        Ship.ApplyGravityAndDrag(this, delta);
    }
}
