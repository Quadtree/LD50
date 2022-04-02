using System;
using Godot;

public class Planet : RigidBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    Material TappedOutMaterial;

    [Export]
    public float Battery = 5f;

    MeshInstance Atmo;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Atmo = this.FindChildByName<MeshInstance>("MeshInstance2");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Battery <= 0 && Atmo.MaterialOverride != TappedOutMaterial)
        {
            Atmo.MaterialOverride = TappedOutMaterial;
        }
    }
}
