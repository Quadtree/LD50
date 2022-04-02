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

        var toDelete = new Array<Spatial>();

        foreach (var it in Objects)
        {
            if (!it.Key.IsInstanceValid())
            {
                toDelete.Add(it.Key);
            }
            else
            {
                var pos = it.Key.GetGlobalLocation();
                it.Value.RectPosition = new Vector2(pos.x + 150, pos.z + 150);

                if (it.Key is Planet && !((Planet)it.Key).IsSun && ((Planet)it.Key).Battery <= 0)
                {
                    it.Value.Modulate = new Color(0, 0, 0, 1);
                }
            }
        }
    }

    void AddIfNotThere(Spatial spatial)
    {
        if (!Objects.ContainsKey(spatial))
        {
            var tex = ShipTexture;

            if (spatial is Planet && ((Planet)spatial).IsSun) tex = SunTexture;
            if (spatial is Planet && !((Planet)spatial).IsSun) tex = PlanetTexture;

            var rect = new TextureRect();
            rect.Texture = tex;
            rect.RectSize = new Vector2(24, 24);
            AddChild(rect);

            Objects[spatial] = rect;
        }
    }
}
