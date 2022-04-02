using System;
using Godot;

public class Ship : RigidBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    Vector2 Control;

    MeshInstance[] FutureMoves = new MeshInstance[16];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FutureMoves[0] = this.FindChildByName<MeshInstance>("FutureMove0");

        for (var i = 1; i < FutureMoves.Length; ++i)
        {
            FutureMoves[i] = (MeshInstance)FutureMoves[0].Duplicate();
            AddChild(FutureMoves[i]);

            FutureMoves[i].Translation = new Vector3(i, 0, 0);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        ApplyImpulse(new Vector3(0, 0, 0), new Vector3(Control.x, 0, Control.y) * delta * 6f);

        foreach (var it in GetTree().CurrentScene.FindChildrenByType<Planet>())
        {
            var diff = (it.Translation - Translation);
            var dist = diff.Length();
            ApplyCentralImpulse(diff.Normalized() * delta * 500f / (dist * dist * dist));
            //Console.WriteLine(dist);

            if (dist / it.Scale.x < 1.5f)
            {
                ApplyCentralImpulse(-LinearVelocity * delta * 0.25f);
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        Control.x = @event.GetActionStrength("move_right") - @event.GetActionStrength("move_left");
        Control.y = @event.GetActionStrength("move_down") - @event.GetActionStrength("move_up");
    }
}
