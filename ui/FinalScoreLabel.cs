using System;
using Godot;

public class FinalScoreLabel : Label
{
    public override void _Process(float delta)
    {
        Text = $"Your final score was: {Math.Round(GetTree().CurrentScene.FindChildByType<Ship>()?.Score ?? 0)}";
    }
}
