using System;
using Godot;

public class Ship : RigidBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    Vector2 Control;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        ApplyImpulse(new Vector3(0, 0, 0), new Vector3(Control.x, 0, Control.y) * delta);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        Control.x = @event.GetActionStrength("move_right") - @event.GetActionStrength("move_left");
        Control.y = @event.GetActionStrength("move_down") - @event.GetActionStrength("move_up");
    }
}
