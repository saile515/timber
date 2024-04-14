using System;
using Godot;

public partial class Tool : Node2D
{
    private ToolType _type;
    private int _damage;
    public ToolType Type
    {
        get { return _type; }
    }
    public int Damage
    {
        get { return _damage; }
    }

    public void CreateToolFromResource(ToolResource resource)
    {
        _type = resource.Type;
        _damage = resource.Damage;
        GetNode<Sprite2D>("Sprite").Texture = resource.Sprite;
    }
}
