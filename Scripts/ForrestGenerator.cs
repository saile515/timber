using System;
using Godot;

[Tool]
public partial class ForrestGenerator : Node2D
{
    private Vector2I _size = new(100, 100);
    private PackedScene _treeScene = GD.Load<PackedScene>("res://Scenes/tree.tscn");

    [Export]
    public Vector2I Size
    {
        get { return _size; }
        set
        {
            _size = value;
            QueueRedraw();
        }
    }

    [Export]
    public float Density = 0.15f;

    [Export]
    public Vector2I StumpCoordinates = new(1, 0);

    [Export]
    public TileMap TileMap;

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        Random random = new();
        float step = 1 / Density;

        for (float x = 0; x <= Size.X - step + 1; x += step)
        {
            for (float y = 0; y <= Size.Y - step + 1; y += step)
            {
                float worldX =
                    (x + (float)random.NextDouble() * (step - 2) + 1 - Size.X / 2) * 16
                    + Position.X;
                float worldY =
                    (y + (float)random.NextDouble() * (step - 2) + 1 - Size.Y / 2) * 16
                    + Position.Y;

                Vector2I tileMapPosition = TileMap.LocalToMap(new Vector2(worldX, worldY));
                TileMap.SetCell(1, tileMapPosition, 0, StumpCoordinates);
                StaticBody2D tree = _treeScene.Instantiate() as StaticBody2D;
                tree.Position = TileMap.MapToLocal(tileMapPosition) - Position - new Vector2(0, 8);
                AddChild(tree);
            }
        }
    }

    public override void _Draw()
    {
        if (!Engine.IsEditorHint())
        {
            return;
        }

        DrawRect(
            new Rect2(-Size.X * 8, -Size.Y * 8, Size.X * 16, Size.Y * 16),
            Colors.Green,
            false
        );
    }
}
