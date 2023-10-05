using Godot;
using System;

public abstract partial class BaseWeapon : Node2D
{
    protected bool equipped;
    
    public bool IsEquipped() => equipped;

    public void SetEquipped(bool equipped)
    {
        this.equipped = equipped;
        Visible = equipped;
        SetProcess(equipped);
    }

    public abstract Texture2D GetWeaponImage();
}
