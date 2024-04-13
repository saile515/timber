using System;
using Godot;

[Tool]
public partial class ForrestGenerator : Node2D
{
    private Vector2I _size = new(100, 100);

    [Export]
    public Vector2I size
    {
        get { return _size; }
        set
        {
            _size = value;
            QueueRedraw();
        }
    }

    [Export]
    public float density = 0.15f;

    [Export]
    public Vector2I stumpCoordinates = new(1, 0);

    [Export]
    public TileMap tileMap;
    private PackedScene _treeScene = GD.Load<PackedScene>("res://tree.tscn");

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        Random random = new();
        float area = size.X * size.Y;
        float step = 1 / density;

        for (float x = 0; x <= size.X - step + 1; x += step)
        {
            for (float y = 0; y <= size.Y - step + 1; y += step)
            {
                float world_x =
                    (x + (float)random.NextDouble() * (step - 2) + 1 - size.X / 2) * 16
                    + Position.X;
                float world_y =
                    (y + (float)random.NextDouble() * (step - 2) + 1 - size.Y / 2) * 16
                    + Position.Y;

                Vector2I tileMapPosition = tileMap.LocalToMap(new Vector2(world_x, world_y));
                tileMap.SetCell(0, tileMapPosition, 0, stumpCoordinates);
                Sprite2D tree = _treeScene.Instantiate() as Sprite2D;
                tree.Position = tileMap.MapToLocal(tileMapPosition) - Position - new Vector2(0, 8);
                tree.Offset -= new Vector2(0, tree.GetRect().Size.Y / 2 - 8);
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
            new Rect2(-size.X * 8, -size.Y * 8, size.X * 16, size.Y * 16),
            Colors.Green,
            false
        );
    }
}
