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
    public float Battery = 60f;

    [Export]
    public bool IsSun = false;

    float MaxBattery;

    MeshInstance Atmo;

    SpatialMaterial AtmoMat;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Atmo = this.FindChildByName<MeshInstance>("MeshInstance2");
        AtmoMat = (SpatialMaterial)Atmo.MaterialOverride.Duplicate();
        Atmo.MaterialOverride = AtmoMat;
        MaxBattery = Battery;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Battery = Math.Max(0, Battery);

        AtmoMat.AlbedoColor = new Color(
            0,
            0,
            Battery / 140f,
            0.3f
        );
    }
}
