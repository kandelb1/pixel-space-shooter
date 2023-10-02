using Godot;
using System;

public partial class HitboxComponent : Area2D
{
    [Export] private int damage;

    public int GetDamage() => damage;
}
