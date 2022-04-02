using System;
using Godot;

public class FuelLabel : Label
{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Text = $"Fuel: {(int?)GetTree().CurrentScene.FindChildByType<Ship>()?.Fuel}";
    }
}
