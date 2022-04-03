using System;
using Godot;

public class BatteryLabel : Label
{
    int LastBatteryInt;

    float FlashTime = 0;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var batteryLevel = GetTree().CurrentScene.FindChildByType<Ship>()?.Battery ?? 0;

        Text = $"Battery: {Math.Round(batteryLevel)}s";

        if (Math.Round(batteryLevel) < LastBatteryInt && Math.Round(batteryLevel) <= 8)
        {
            FlashTime = 0.4f;
            if (Engine.TimeScale <= 1.1f && LastBatteryInt == 9) Util.SpawnOneShotSound("res://sounds/battery_low.wav", this, -5.0f);
        }

        if (Math.Round(batteryLevel) != LastBatteryInt)
        {
            LastBatteryInt = (int)Math.Round(batteryLevel);
        }

        Modulate = new Color(1, Util.Clamp(1 - FlashTime * 2, 0, 1), Util.Clamp(1 - FlashTime * 2, 0, 1), 1);
        FlashTime -= delta;
    }
}
