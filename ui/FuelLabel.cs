using System;
using Godot;

public class FuelLabel : Label
{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Text = $"Fuel: {(GetTree().CurrentScene.FindChildByType<Ship>()?.Fuel ?? 0).ToString("0.0")}s";
    }
}
