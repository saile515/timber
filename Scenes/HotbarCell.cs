using System;
using Godot;

public partial class HotbarCell : Button
{
    private Texture2D _icon;
    private Texture2D _activeIcon;
    private InventoryManager _inventoryManager;

    [Export]
    public int Index;

    public override void _Ready()
    {
        _icon = GD.Load<Texture2D>("res://Sprites/hotbar_cell.png");
        _activeIcon = GD.Load<Texture2D>("res://Sprites/hotbar_cell_active.png");
        _inventoryManager = GetNode<InventoryManager>("/root/Scene/Player/Inventory");

        OnActiveIndexUpdatedEvent(_inventoryManager.ActiveIndex);
        _inventoryManager.ActiveIndexUpdated += OnActiveIndexUpdatedEvent;
    }

    private void OnActiveIndexUpdatedEvent(int activeIndex)
    {
        if (Index == activeIndex)
        {
            Icon = _activeIcon;
        }
        else
        {
            Icon = _icon;
        }
    }

    public override void _ExitTree()
    {
        _inventoryManager.ActiveIndexUpdated -= OnActiveIndexUpdatedEvent;
    }
}
