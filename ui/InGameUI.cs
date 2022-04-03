using System;
using Godot;

public class InGameUI : Control
{
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("return_to_title")) GetTree().ChangeScene("res://UI/TitleScreen.tscn");
    }
}
