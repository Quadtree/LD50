using System;
using Godot;

public class PlanetGenerator : Node
{
    [Export]
    PackedScene PlanetTemplate;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CallDeferred(nameof(CreatePlanets));
    }

    void CreatePlanets()
    {
        for (int i = 0; i < 4; ++i)
        {
            var planet = PlanetTemplate.Instance<Planet>();

            GetTree().CurrentScene.AddChild(planet);

            planet.CallDeferred(nameof(Planet.InitPlanetSize),
                Util.random() * 6 + 1,
                Util.random() * 2 + 0.5f,
                i * 25 + 20,
                Util.random() * Mathf.Pi * 2
            );
        }
    }
}
