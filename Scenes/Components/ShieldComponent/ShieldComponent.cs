using Godot;
using System;

public partial class ShieldComponent : Area2D
{
    public override void _Ready()
    {
        AreaEntered += HandleAreaEntered;
    }

    private void HandleAreaEntered(Area2D other)
    {
        // basic shield that deletes any projectile that enters it
        other.Owner.Free();
    }
}
