using System;
using Godot;

public class DefeatDialog : PopupDialog
{
    float Time = 0;

    public override void _Ready()
    {
        PopupExclusive = true;
    }

    public override void _Process(float delta)
    {
        if (Visible) Time += delta;
        else Time = 0;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseButton && Visible && Time >= 1)
        {
            GetTree().ChangeScene("res://ui/TitleScreen.tscn");
        }
    }
}
