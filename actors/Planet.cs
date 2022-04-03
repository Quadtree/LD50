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

    public float MaxBattery;

    public float RockyRadius;


    MeshInstance Atmo;

    SpatialMaterial AtmoMat;

    float OrbitalDistance = 0;
    float OrbitalAngularVelocity = 0;
    float CurrentOrbitAngle = 0;
    Planet OrbitalPrimary;

    public Vector3 OrbitalVelocity;

    public float AtmoRadius;

    public float AtmoThickness;

    public float RotationRate = Util.random();

    [Export]
    Material RockyOverrideMaterial;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (RockyOverrideMaterial != null) this.FindChildByName<MeshInstance>("MeshInstance").MaterialOverride = RockyOverrideMaterial;
    }

    public void InitPlanetSize(float rockyRadius, float atmoRadius, float orbitalDistance, float orbitalAngle, float atmoThickness)
    {
        this.SetGlobalLocation(new Vector3(Mathf.Cos(orbitalAngle) * orbitalDistance, 0, Mathf.Sin(orbitalAngle) * orbitalDistance));

        RockyRadius = rockyRadius;

        AtmoThickness = atmoThickness;
        AtmoRadius = atmoRadius + rockyRadius;

        var rocky = this.FindChildByName<MeshInstance>("MeshInstance");
        rocky.Scale = new Vector3(rockyRadius, rockyRadius, rockyRadius);

        var atmo = this.FindChildByName<MeshInstance>("MeshInstance2");
        atmo.Scale = new Vector3(AtmoRadius, AtmoRadius, AtmoRadius);

        var collisionShape = this.FindChildByName<CollisionShape>("CollisionShape");
        var sphereShape = new SphereShape();
        sphereShape.Radius = rockyRadius;
        collisionShape.Shape = sphereShape;


        Atmo = this.FindChildByName<MeshInstance>("MeshInstance2");
        AtmoMat = (SpatialMaterial)Atmo.MaterialOverride.Duplicate();
        if (AtmoMat == null) throw new Exception("We must have an AtmoMat");
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

        Engine.TimeScale = 1f;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Battery = Math.Max(0, Battery);

        if (AtmoMat != null)
        {
            AtmoMat.AlbedoColor = new Color(
                0,
                0,
                Battery / MaxBattery * 0.3f + (Battery > 0 ? .15f : 0.0f),
                AtmoThickness / 2
            );
        }

        if (OrbitalPrimary != null)
        {
            CurrentOrbitAngle += OrbitalAngularVelocity * delta;

            var oldPos = this.GetGlobalLocation();

            this.SetGlobalLocation(new Vector3(Mathf.Cos(CurrentOrbitAngle) * OrbitalDistance, 0, Mathf.Sin(CurrentOrbitAngle) * OrbitalDistance));

            OrbitalVelocity = (this.GetGlobalLocation() - oldPos) / delta;

            RotateY(RotationRate * delta);
        }
    }
}
