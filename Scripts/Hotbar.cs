using System;
using Godot;

public partial class Hotbar : HBoxContainer
{
    private PackedScene _hotbarCell;
    private InventoryManager _inventoryManager;

    public override void _Ready()
    {
        _inventoryManager = GetNode<InventoryManager>("/root/Scene/Player/Inventory");
        _hotbarCell = GD.Load<PackedScene>("res://Scenes/hotbar_cell.tscn");
        UpdateHotbar();
        _inventoryManager.InventoryUpdated += UpdateHotbar;
    }

    private void UpdateHotbar()
    {
        foreach (GodotObject child in GetChildren())
        {
            child.Free();
        }

        for (int i = 0; i < 10; i++)
        {
            HotbarCell button = _hotbarCell.Instantiate() as HotbarCell;
            button.Index = i;

            ItemResource item = _inventoryManager.Inventory[i].Item1;
            int count = _inventoryManager.Inventory[i].Item2;
            if (item is not null)
            {
                button.GetNode<TextureRect>("Sprite").Texture = item.Sprite;
                if (count > 1)
                {
                    button.GetNode<Label>("Amount").Text = _inventoryManager
                        .Inventory[i]
                        .Item2.ToString();
                }
            }

            AddChild(button);
        }
    }
}
