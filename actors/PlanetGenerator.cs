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

            var radius = Util.random() * 6 + 1;
            var mass = radius * radius * radius;
            var atmoRadius = 3;
            var atmoThickness = Util.random();

            planet.CallDeferred(nameof(Planet.InitPlanetSize),
                radius,
                atmoRadius,
                i * 25 + 20,
                Util.random() * Mathf.Pi * 2,
                atmoThickness
            );

            var atmoMass = (Mathf.Pow(radius + atmoRadius, 3) - Mathf.Pow(radius, 3)) * atmoThickness;

            planet.Battery = atmoMass * (Util.random() * 0.5f + 0.5f);
            planet.MaxBattery = atmoMass;
            planet.Mass = mass * 150;

            Console.WriteLine($"Planet Created: radius={radius} mass={mass} battery={planet.Battery}/{planet.MaxBattery}");
        }
    }
}
