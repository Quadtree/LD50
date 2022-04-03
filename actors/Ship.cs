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
    float DragConstant = 0.6f;

    [Export]
    float ThrustMultiplier = 6f;

    [Export]
    public float Fuel = 10f;

    [Export]
    public float Battery = 30f;

    [Export]
    public float MaxBattery = 500f;

    [Export]
    public float BatteryChargeRate = 2f;

    Camera Cam;

    bool Destroyed = false;

    float RespawnTimer = 0;


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

        //MaxBattery = Battery;

        Cam = this.FindChildByType<Camera>();

        Connect("body_entered", this, nameof(OnCollision));
    }

    void Death()
    {
        if (Destroyed) return;

        foreach (var it in this.GetChildren())
        {
            if (!(it is Camera))
            {
                RemoveChild((Node)it);
            }
        }

        Destroyed = true;
    }

    void OnCollision(Node other)
    {
        Console.WriteLine("Collision!");

        Death();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        EnsurePlanets();

        if (Destroyed)
        {
            Engine.TimeScale = 1f;
            RespawnTimer += delta;

            if (RespawnTimer >= 3.5f)
            {
                GetTree().ChangeScene("res://maps/default.tscn");
                RespawnTimer = 0;
            }

            return;
        }

        UpdateFutureMoves();

        Control.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        Control.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        Battery -= delta;

        Cam.LookAt(this.GetGlobalLocation(), new Vector3(0, 0, -1));

        if (Fuel <= 0)
        {
            Engine.TimeScale = Mathf.Min(Engine.TimeScale + delta * 0.5f, 10f);
        }

        if (Battery <= 0)
        {
            Death();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (Destroyed)
        {
            LinearVelocity = new Vector3(0, 0, 0);
            AngularVelocity = new Vector3(0, 0, 0);
            return;
        }

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

        foreach (var it in GetTree().CurrentScene.FindChildrenByType<Planet>())
        {
            var diff = (it.Translation - Translation);
            var dist = diff.Length();
            ApplyCentralImpulse(diff.Normalized() * delta * GravityConstant * it.Mass * Mass / (dist * dist));
            //Console.WriteLine(dist);

            if (dist < it.AtmoRadius)
            {
                ApplyCentralImpulse(-(LinearVelocity - it.OrbitalVelocity) * delta * DragConstant * it.AtmoThickness);

                var gained = Math.Max(Math.Min(Math.Min(delta * LinearVelocity.Length() * BatteryChargeRate, MaxBattery - Battery), it.Battery), 0);

                Battery += gained;
                it.Battery -= gained;
            }
        }
    }

    void UpdateFutureMoves()
    {
        var pos = this.GetGlobalLocation();
        var vel = this.LinearVelocity;
        float delta = 1f / 5f;
        bool crashed = false;

        var substeps = 12;
        if (OS.GetName() == "HTML5") substeps = 1;
        delta /= substeps;

        var planets = GetTree().CurrentScene.FindChildrenByType<Planet>();

        var nearestPlanet = planets.MinBy(it => (int)it.GetGlobalLocation().DistanceSquaredTo(this.GetGlobalLocation()));
        //Console.WriteLine($"{nearestPlanet.OrbitalVelocity} {vel}");

        vel -= nearestPlanet.OrbitalVelocity;

        for (var i = 0; i < 16; ++i)
        {
            if (!crashed)
            {
                for (int j = 0; j < substeps; ++j)
                {
                    foreach (var it in planets)
                    {
                        var diff = (it.Translation - pos);
                        var dist = diff.Length();
                        vel += (diff.Normalized() * delta * it.Mass * GravityConstant / (dist * dist));

                        if (dist < it.AtmoRadius)
                        {
                            vel += (-(LinearVelocity - it.OrbitalVelocity) * delta * DragConstant * it.AtmoThickness) / Mass;
                        }

                        if (dist < it.RockyRadius)
                        {
                            crashed = true;
                        }
                    }

                    pos += vel * delta;
                }
            }

            FutureMoves[i].SetGlobalLocation(pos);
        }
    }

    void EnsurePlanets()
    {
        //if (Planets == null) Planets = GetTree().CurrentScene.FindChildrenByType<Planet>().ToArray();
    }
}
