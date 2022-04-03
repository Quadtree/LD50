using System;
using Godot;

public class MissionBox : Label
{
    public override void _Ready()
    {
        switch (TitleScreen.MissionType)
        {
            case TitleScreen.MissionTypeEnum.Standard:
                Text = "Mission: Survive for as long as possible.";
                break;
            case TitleScreen.MissionTypeEnum.Collection:
                Text = "Mission: Survive for as long as possible while collecting crates.";
                break;
            case TitleScreen.MissionTypeEnum.Collision:
                Text = "Mission: Build up as much battery as possible, then crash into a planet to generate scientific data.";
                break;
            case TitleScreen.MissionTypeEnum.Evasion:
                Text = "Mission: Survive for as long as possible while avoiding the hostile drones.";
                break;
        }
    }
}
