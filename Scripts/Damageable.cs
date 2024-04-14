using System;
using Godot;

public partial class Damageable : StaticBody2D
{
    private CharacterBody2D _player;

    [Export]
    public int Hp = 100;

    [Export]
    public ToolType Tool;

    [Export]
    public ItemResource item;

    [Export]
    public int amount = 1;

    public override void _Ready()
    {
        _player = GetNode<Player>("/root/Scene/Player");
    }

    public override void _Input(InputEvent @event)
    {
        if (
            @event is InputEventMouseButton mouseEvent
            && mouseEvent.ButtonIndex == MouseButton.Left
            && mouseEvent.Pressed
        )
        {
            if (_player is null)
            {
                return;
            }

            Tool tool = _player.GetNodeOrNull<Tool>("Tool");

            if (
                (_player.GlobalPosition - GlobalPosition).Length() < 20
                && tool is not null
                && tool.Type == Tool
            )
            {
                Hp -= tool.Damage;
                if (Hp <= 0)
                {
                    _player.GetNode<InventoryManager>("Inventory").AddItem(item, amount);
                    QueueFree();
                }
            }
        }
    }
}
