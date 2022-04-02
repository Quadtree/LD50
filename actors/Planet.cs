using System;
using Godot;

public class Planet : RigidBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // oribtal velocity formula
    // v=(2*pi*r)/T

    // orbital period formula
    // T=2*pi*sqrt((a^3) / (G*M))



    [Export]
    Material TappedOutMaterial;

    [Export]
    public float Battery = 30f;

    [Export]
    public bool IsSun = false;

    float MaxBattery;

    MeshInstance Atmo;

    SpatialMaterial AtmoMat;

    float OrbitalDistance = 0;
    float OrbitalAngularVelocity = 0;
    float CurrentOrbitAngle = 0;
    Planet OrbitalPrimary;

    public Vector3 OrbitalVelocity;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Atmo = this.FindChildByName<MeshInstance>("MeshInstance2");
        AtmoMat = (SpatialMaterial)Atmo.MaterialOverride.Duplicate();
        Atmo.MaterialOverride = AtmoMat;
        MaxBattery = Battery;

        if (!IsSun)
        {
            OrbitalPrimary = GetTree().CurrentScene.FindChildByPredicate<Planet>((it) => it.IsSun);
        }

        if (OrbitalPrimary != null)
        {
            OrbitalDistance = (OrbitalPrimary.GetGlobalLocation() - this.GetGlobalLocation()).Length();
            CurrentOrbitAngle = (OrbitalPrimary.GetGlobalLocation() - this.GetGlobalLocation()).Normalized().SignedAngleTo(new Vector3(-1, 0, 0), new Vector3(0, 1, 0));
            Console.WriteLine(CurrentOrbitAngle);

            var gravConst = GetTree().CurrentScene.FindChildByType<Ship>().GravityConstant;

            float orbitalPeriod = 2 * Mathf.Pi * Mathf.Sqrt((Mathf.Pow(OrbitalDistance, 3) / (gravConst * OrbitalPrimary.Mass)));

            OrbitalAngularVelocity = (Mathf.Pi * 2) / orbitalPeriod;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Battery = Math.Max(0, Battery);

        AtmoMat.AlbedoColor = new Color(
            0,
            0,
            Battery / 140f + (Battery > 0 ? .15f : 0.0f),
            0.3f
        );

        if (OrbitalPrimary != null)
        {
            CurrentOrbitAngle += OrbitalAngularVelocity * delta;

            var oldPos = this.GetGlobalLocation();

            this.SetGlobalLocation(new Vector3(Mathf.Cos(CurrentOrbitAngle) * OrbitalDistance, 0, Mathf.Sin(CurrentOrbitAngle) * OrbitalDistance));

            OrbitalVelocity = (this.GetGlobalLocation() - oldPos) / delta;
        }
    }
}
