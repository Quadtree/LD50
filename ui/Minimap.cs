using System;
using Godot;
using Godot.Collections;

public class Minimap : Control
{
    [Export]
    Texture PlanetTexture;

    [Export]
    Texture SunTexture;

    [Export]
    Texture ShipTexture;

    [Export]
    Texture DroneTexture;

    [Export]
    Texture CrateTexture;

    Dictionary<Spatial, TextureRect> Objects = new Dictionary<Spatial, TextureRect>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // TODO: Perf check this
        foreach (var it in GetTree().CurrentScene.FindChildrenByType<Planet>()) AddIfNotThere(it);
        foreach (var it in GetTree().CurrentScene.FindChildrenByType<Ship>()) AddIfNotThere(it);
        foreach (var it in GetTree().CurrentScene.FindChildrenByType<Drone>()) AddIfNotThere(it);
        foreach (var it in GetTree().CurrentScene.FindChildrenByType<Crate>()) AddIfNotThere(it);

        var toDelete = new Array<Spatial>();

        foreach (var it in Objects)
        {
            if (!it.Key.IsInstanceValid() || it.Key.GetParent() == null)
            {
                toDelete.Add(it.Key);
            }
            else
            {
                var pos = it.Key.GetGlobalLocation();

                it.Value.RectPosition = new Vector2(
                    Util.Clamp(pos.x * 0.8f + 150, 20, 280) - 12,
                    Util.Clamp(pos.z * 0.8f + 150, 20, 280) - 12
                );

                if (it.Key is Planet && !((Planet)it.Key).IsSun && ((Planet)it.Key).Battery <= 0)
                {
                    it.Value.Modulate = new Color(0, 0, 0, 1);
                }
            }
        }

        foreach (var it in toDelete)
        {
            Objects.Remove(it);
        }
    }

    void AddIfNotThere(Spatial spatial)
    {
        if (!Objects.ContainsKey(spatial))
        {
            var tex = ShipTexture;

            if (spatial is Planet && ((Planet)spatial).IsSun) tex = SunTexture;
            if (spatial is Planet && !((Planet)spatial).IsSun) tex = PlanetTexture;
            if (spatial is Drone) tex = DroneTexture;
            if (spatial is Crate) tex = CrateTexture;

            var rect = new TextureRect();
            rect.Texture = tex;
            rect.RectSize = new Vector2(24, 24);
            AddChild(rect);

            Objects[spatial] = rect;
        }
    }
}
