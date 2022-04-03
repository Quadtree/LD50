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

    public static float GravityConstant = 0.01f;

    public static float DragConstant = 0.6f;

    [Export]
    float ThrustMultiplier = 6f;

    [Export]
    public float Fuel = 15f;

    [Export]
    public float Battery = 30f;

    [Export]
    public float MaxBattery = 500f;

    public static float BatteryChargeRate = 2f;

    Camera Cam;

    bool Destroyed = false;

    float RespawnTimer = 0;

    public float Score;

    AudioStreamPlayer ThrusterLoop;
    AudioStreamPlayer ChargeLoop;

    float BatteryGained = 0;


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

        ThrusterLoop = this.FindChildByName<AudioStreamPlayer>("ThrusterLoop");
        ChargeLoop = this.FindChildByName<AudioStreamPlayer>("ChargeBattery");
    }

    public void Death()
    {
        if (Destroyed) return;

        foreach (var it in this.GetChildren())
        {
            if (!(it is Camera) && ((Node)it).Name != "Starfield")
            {
                RemoveChild((Node)it);
            }
        }

        Destroyed = true;

        Util.SpawnOneShotSound("res://sounds/destroyed2.wav", this, -5f);
    }

    void OnCollision(Node other)
    {
        Console.WriteLine("Collision!");

        if (other is Crate)
        {
            Util.SpawnOneShotSound("res://sounds/crate.wav", this, -5f);
            other.QueueFree();
            Score += 30;
            return;
        }

        if (TitleScreen.MissionType == TitleScreen.MissionTypeEnum.Collision)
        {
            var relativeVelocity = (this.LinearVelocity - ((RigidBody)other).LinearVelocity).Length();
            // 8 or so is a good speed

            Score += relativeVelocity * Battery / 3;
        }

        Death();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        EnsurePlanets();

        Cam.LookAt(this.GetGlobalLocation(), new Vector3(0, 0, -1));

        if (Destroyed)
        {
            Engine.TimeScale = 1f;
            RespawnTimer += delta;

            if (RespawnTimer >= 1.5f && !GetTree().CurrentScene.FindChildByName<PopupDialog>("GameOverDialog").Visible)
            {
                GetTree().CurrentScene.FindChildByName<PopupDialog>("GameOverDialog").PopupCentered();
            }

            /*if (RespawnTimer >= 3.5f)
            {
                GetTree().ChangeScene("res://maps/default.tscn");
                RespawnTimer = 0;
            }*/

            return;
        }

        Score += delta;

        UpdateFutureMoves();

        Control.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        Control.y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        Battery -= delta;

        if (Fuel <= 0)
        {
            Engine.TimeScale = Mathf.Min(Engine.TimeScale + delta * 0.5f, 10f);
        }

        if (Battery <= 0)
        {
            Death();
        }

        if (Control.Length() > 0 && Fuel > 0)
        {
            if (!ThrusterLoop.Playing) ThrusterLoop.Play();
        }
        else
        {
            if (ThrusterLoop.Playing) ThrusterLoop.Stop();
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

        BatteryGained = 0;
        ApplyGravityAndDrag(this, delta);

        if (BatteryGained > 0 && !ChargeLoop.Playing) ChargeLoop.Play();
        if (BatteryGained <= 0 && ChargeLoop.Playing) ChargeLoop.Stop();
    }

    public static void ApplyGravityAndDrag(RigidBody target, float delta)
    {
        foreach (var it in target.GetTree().CurrentScene.FindChildrenByType<Planet>())
        {
            var diff = (it.Translation - target.Translation);
            var dist = diff.Length();
            target.ApplyCentralImpulse(diff.Normalized() * delta * GravityConstant * it.Mass * target.Mass / (dist * dist));
            //Console.WriteLine(dist);

            if (dist < it.AtmoRadius)
            {
                target.ApplyCentralImpulse(-(target.LinearVelocity - it.OrbitalVelocity) * delta * DragConstant * it.AtmoThickness);

                if (target is Ship)
                {
                    var ship = (Ship)target;

                    var gained = Math.Max(Math.Min(Math.Min(delta * target.LinearVelocity.Length() * BatteryChargeRate, ship.MaxBattery - ship.Battery), it.Battery), 0);

                    ship.Battery += gained;
                    it.Battery -= gained;

                    ship.BatteryGained += gained;
                }
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
