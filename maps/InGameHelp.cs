using System;
using Godot;

public class InGameHelp : PopupDialog
{
    float TimeOpen = 0;

    public override void _Ready()
    {
        PopupExclusive = true;
    }

    public override void _Process(float delta)
    {
        if (Visible) TimeOpen += delta / 0.0001f;



        //Console.WriteLine(TimeOpen);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed("show_help"))
        {
            Engine.TimeScale = 0.0001f;
            PopupCentered();
            Console.WriteLine("OPEN!");
        }

        if (TimeOpen >= 1 && @event is InputEventMouseButton)
        {
            Console.WriteLine("Unpause!");
            TimeOpen = 0;
            Engine.TimeScale = 1f;
            Visible = false;
        }
    }
}
