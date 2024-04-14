using System;
using Godot;

public partial class ItemResource : Resource
{
    [Export]
    public string Name;

    [Export]
    public int StackSize;

    [Export]
    public Texture2D Sprite;
}
