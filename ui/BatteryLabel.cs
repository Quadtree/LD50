using System;
using Godot;

public class BatteryLabel : Label
{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Text = $"Battery: {(int?)GetTree().CurrentScene.FindChildByType<Ship>()?.Battery}";
    }
}
