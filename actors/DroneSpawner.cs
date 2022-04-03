using System;
using System.Linq;
using Godot;

public class DroneSpawner : Node
{
    [Export]
    PackedScene DroneType;

    float SpawnCharge = 0;

    public override void _Ready()
    {

    }

    public override void _Process(float delta)
    {
        SpawnCharge += delta;

        var count = GetTree().CurrentScene.FindChildrenByType<Drone>(1).Count();

        if (SpawnCharge >= Mathf.Pow(count, 1.5f))
        {
            var drone = DroneType.Instance<Drone>();
            GetTree().CurrentScene.AddChild(drone);

            drone.SetGlobalLocation(new Vector3(
                Util.random() * 300 - 150,
                0,
                Util.random() * 300 - 150
            ));

            SpawnCharge = 0;
        }
    }
}
