using System;
using System.Media;
using Godot;

public class InGameUI : Control
{
    public override void _Ready()
    {
        if (GetTree().Root.FindChildByName<Node>("BGM") == null)
        {
            var bgm = new AudioStreamPlayer();
            GetTree().Root.AddChild(bgm);
            bgm.Name = "BGM";
            bgm.Stream = GD.Load<AudioStream>("res://sounds/bgm.ogg");
            bgm.VolumeDb = -5;

            bgm.Play();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("return_to_title")) GetTree().ChangeScene("res://UI/TitleScreen.tscn");
    }
}
