using System;
using System.Linq;
using Godot;

public class Ship : RigidBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    Vector2 Control;

    MeshInstance[] FutureMoves = new MeshInstance[16];

    [Export]
    public float GravityConstant = 0.01f;

    [Export]
    float DragConstant = 0.1f;

    [Export]
    float ThrustMultiplier = 6f;

    Planet[] Planets = null;

    [Export]
    public float Fuel = 20f;

    [Export]
    public float Battery = 30f;

    float MaxBattery = 0f;

    [Export]
    public float BatteryChargeRate = 8f;

    Camera Cam;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FutureMoves[0] = this.FindChildByName<MeshInstance>("FutureMove0");

        for (var i = 1; i < FutureMoves.Length; ++i)
        {
            FutureMoves[i] = (MeshInstance)FutureMoves[0].Duplicate();
            AddChild(FutureMoves[i]);

            FutureMoves[i].Translation = new Vector3(i, 0, 0);
        }

        MaxBattery = Battery;

        Cam = this.FindChildByType<Camera>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        EnsurePlanets();

        UpdateFutureMoves();

        Control.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        Control.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        Battery -= delta;

        Cam.LookAt(this.GetGlobalLocation(), new Vector3(0, 0, -1));
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        EnsurePlanets();

        //Console.WriteLine(Control);

        if (Fuel > 0)
        {
            ApplyImpulse(new Vector3(0, 0, 0), new Vector3(Control.x, 0, Control.y) * delta * ThrustMultiplier);
            Fuel -= delta * new Vector3(Control.x, 0, Control.y).Length();
        }
        else
        {
            Fuel = 0;
        }

        foreach (var it in Planets)
        {
            var diff = (it.Translation - Translation);
            var dist = diff.Length();
            ApplyCentralImpulse(diff.Normalized() * delta * GravityConstant * it.Mass * Mass / (dist * dist));
            //Console.WriteLine(dist);

            if (dist / it.Scale.x < 1.5f)
            {
                ApplyCentralImpulse(-LinearVelocity * delta * DragConstant);

                var gained = Math.Min(delta * LinearVelocity.Length() * BatteryChargeRate, MaxBattery - Battery);
                Battery += gained;
                it.Battery -= gained;
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        //Control.x = @event.GetActionStrength("move_right") - @event.GetActionStrength("move_left");
        //Control.y = @event.GetActionStrength("move_down") - @event.GetActionStrength("move_up");

        //Control.y = (@event.IsActionPressed("move_down") ? 1 : 0) - (@event.IsActionPressed("move_up") ? 1 : 0);
    }

    void UpdateFutureMoves()
    {
        var pos = this.GetGlobalLocation();
        var vel = this.LinearVelocity;
        float delta = 1f / 5f;
        bool crashed = false;

        for (var i = 0; i < 16; ++i)
        {
            if (!crashed)
            {
                foreach (var it in Planets)
                {
                    var diff = (it.Translation - pos);
                    var dist = diff.Length();
                    vel += (diff.Normalized() * delta * it.Mass * GravityConstant / (dist * dist));

                    if (dist / it.Scale.x < 1.5f)
                    {
                        vel += (-LinearVelocity * delta * DragConstant) / Mass;
                    }

                    if (dist / it.Scale.x < 0.8f)
                    {
                        crashed = true;
                    }
                }

                pos += vel * delta;
            }

            FutureMoves[i].SetGlobalLocation(pos);
        }
    }

    void EnsurePlanets()
    {
        if (Planets == null) Planets = GetTree().CurrentScene.FindChildrenByType<Planet>().ToArray();
    }
}
