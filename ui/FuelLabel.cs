using System;
using Godot;

public class FuelLabel : Label
{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(float delta)
    // {
    //     Text = $"Fuel: {}s";
    // }

    int LastBatteryInt;

    float FlashTime = 0;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var batteryLevel = (GetTree().CurrentScene.FindChildByType<Ship>()?.Fuel ?? 0);

        Text = $"Fuel: {batteryLevel.ToString("0.0")}s";

        if (Math.Round(batteryLevel) < LastBatteryInt && Math.Round(batteryLevel) <= 3)
        {
            FlashTime = 0.4f;
            if (Engine.TimeScale <= 1.1f && LastBatteryInt == 4) Util.SpawnOneShotSound("res://sounds/fuel_low.wav", this, -3.0f);
        }

        if (Math.Round(batteryLevel) != LastBatteryInt)
        {
            LastBatteryInt = (int)Math.Round(batteryLevel);
        }

        Modulate = new Color(1, Util.Clamp(1 - FlashTime * 2, 0, 1), Util.Clamp(1 - FlashTime * 2, 0, 1), 1);
        FlashTime -= delta;
    }
}
