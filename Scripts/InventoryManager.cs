using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;

public partial class InventoryManager : Node2D
{
    private PackedScene _toolScene;
    private List<(ItemResource, int)> _inventory = new();
    private int _inventorySize = 10;
    private int _activeIndex = -1;
    public int ActiveIndex
    {
        get { return _activeIndex; }
        set
        {
            _activeIndex = value;
            EmitSignal(SignalName.ActiveIndexUpdated, _activeIndex);
            UpdateHand();
        }
    }
    public ReadOnlyCollection<(ItemResource, int)> Inventory
    {
        get { return _inventory.AsReadOnly(); }
    }

    public override void _Ready()
    {
        _toolScene = GD.Load<PackedScene>("res://Scenes/tool.tscn");
        for (int i = 0; i < _inventorySize; i++)
        {
            _inventory.Add((null, 0));
        }
        AddItem(GD.Load<ToolResource>("res://Items/Tools/basic_axe.tres"));
    }

    public override void _Input(InputEvent @event)
    {
        for (int i = 0; i < 10; i++)
        {
            if (@event.IsActionPressed($"inventory_{i + 1}"))
            {
                ActiveIndex = ActiveIndex == i ? -1 : i;
            }
        }
    }

    public int AddItem(ItemResource item, int amount = 1)
    {
        int firstEmptySlot = -1;
        int amountLeft = amount;
        for (int slot = 0; slot < _inventorySize; slot++)
        {
            (ItemResource, int) slotItem = _inventory[slot];
            if (slotItem.Item1 is null)
            {
                firstEmptySlot = firstEmptySlot == -1 ? slot : firstEmptySlot;
                continue;
            }

            if (slotItem.Item1.Name == item.Name)
            {
                if (slotItem.Item2 + amountLeft <= item.StackSize)
                {
                    _inventory[slot] = (slotItem.Item1, slotItem.Item2 + amountLeft);
                    EmitSignal(SignalName.InventoryUpdated);
                    return 0;
                }
                else
                {
                    int spaceLeftInStack = item.StackSize - slotItem.Item2;
                    _inventory[slot] = (slotItem.Item1, item.StackSize);
                    amountLeft -= spaceLeftInStack;
                }
            }
        }

        if (firstEmptySlot >= 0)
        {
            _inventory[firstEmptySlot] = (item, amountLeft);
            EmitSignal(SignalName.InventoryUpdated);
            return 0;
        }

        EmitSignal(SignalName.InventoryUpdated);
        return amountLeft;
    }

    public void MoveItem(int from, int to)
    {
        if (
            _inventory[from].Item1 is null
            || (_inventory[to].Item1 is not null && _inventory[to].Item1 != _inventory[from].Item1)
        )
        {
            return;
        }

        if (_inventory[to].Item1 == _inventory[from].Item1)
        {
            int spaceLeftInStack = _inventory[to].Item2 - _inventory[to].Item1.StackSize;
            _inventory[to] = (
                _inventory[to].Item1,
                _inventory[to].Item2
                    + (
                        spaceLeftInStack > _inventory[from].Item2
                            ? _inventory[from].Item2
                            : spaceLeftInStack
                    )
            );
            _inventory[from] = (
                _inventory[from].Item1,
                Math.Max(_inventory[from].Item2 - spaceLeftInStack, 0)
            );
            EmitSignal(SignalName.InventoryUpdated);
            return;
        }

        _inventory[to] = _inventory[from];
        _inventory[from] = (null, 0);

        EmitSignal(SignalName.InventoryUpdated);
        return;
    }

    public void UpdateHand()
    {
        Tool currentTool = GetParent().GetNodeOrNull<Tool>("Tool");
        if (_activeIndex != -1 && _inventory[_activeIndex].Item1 is ToolResource toolResource)
        {
            if (currentTool is not null)
            {
                if (currentTool.Name == toolResource.Name)
                {
                    return;
                }
                else
                {
                    currentTool.QueueFree();
                }
            }

            if (_toolScene is null)
            {
                return;
            }

            Tool tool = _toolScene.Instantiate() as Tool;
            tool.CreateToolFromResource(toolResource);
            GetParent().AddChild(tool);
        }
        else
        {
            currentTool?.QueueFree();
        }
    }

    [Signal]
    public delegate void ActiveIndexUpdatedEventHandler(int activeIndex);

    [Signal]
    public delegate void InventoryUpdatedEventHandler();
}
