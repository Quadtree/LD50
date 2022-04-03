using System;
using Godot;

public class ScoreLabel : Label
{
    public override void _Process(float delta)
    {
        Text = $"Score: {Math.Round(GetTree().CurrentScene.FindChildByType<Ship>()?.Score ?? 0)}";
    }
}
