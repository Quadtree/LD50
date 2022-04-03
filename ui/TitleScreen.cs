using System;
using Godot;

public class TitleScreen : Control
{
    float Time = 0;

    public override void _Process(float delta)
    {
        if (Visible) Time += delta;
        else Time = 0;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseButton && Time >= 1)
        {
            GetTree().ChangeScene("res://maps/default.tscn");
        }
    }
}
