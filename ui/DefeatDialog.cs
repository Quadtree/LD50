using System;
using Godot;

public class DefeatDialog : PopupDialog
{
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseButton)
        {
            GetTree().ChangeScene("res://maps/default.tscn");
        }
    }
}
