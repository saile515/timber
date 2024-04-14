using System;
using Godot;

public enum ToolType
{
    Axe
}

public partial class ToolResource : ItemResource
{
    [Export]
    public ToolType Type;

    [Export]
    public int Damage;
}
