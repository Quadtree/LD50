using System;
using Godot;

public class TitleScreen : Control
{
    float Time = 0;

    public static MissionTypeEnum MissionType = MissionTypeEnum.Standard;

    public enum MissionTypeEnum
    {
        Standard,
        Evasion,
        Collection,
        Collision,
    }

    public override void _Ready()
    {
        base._Ready();

        this.FindChildByName<Button>("StdMission").Connect("pressed", this, nameof(StandardMission));
        this.FindChildByName<Button>("EvsMission").Connect("pressed", this, nameof(EvasionMission));
        this.FindChildByName<Button>("ClcMission").Connect("pressed", this, nameof(CollectionMission));
        this.FindChildByName<Button>("ColMission").Connect("pressed", this, nameof(CollisionMission));
    }

    void StandardMission()
    {
        MissionType = MissionTypeEnum.Standard;
        GetTree().ChangeScene("res://maps/default.tscn");
    }

    void EvasionMission()
    {
        MissionType = MissionTypeEnum.Evasion;
        GetTree().ChangeScene("res://maps/default.tscn");
    }

    void CollectionMission()
    {
        MissionType = MissionTypeEnum.Collection;
        GetTree().ChangeScene("res://maps/default.tscn");
    }

    void CollisionMission()
    {
        MissionType = MissionTypeEnum.Collision;
        GetTree().ChangeScene("res://maps/default.tscn");
    }

    public override void _Process(float delta)
    {
        if (Visible) Time += delta;
        else Time = 0;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // if (@event is InputEventMouseButton && Time >= 1)
        // {
        //     GetTree().ChangeScene("res://maps/default.tscn");
        // }
    }
}
