using System;
using Godot;

public class FPSLabel : Label
{
    public override void _Process(float delta)
    {
        Visible = OS.IsDebugBuild();

        Text = $"FPS: {Engine.GetFramesPerSecond()}";
    }
}
