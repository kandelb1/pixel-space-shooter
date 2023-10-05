using Godot;
using System;

public partial class WeaponSelectUI : Control
{
    [Export] private Ship playerShip;

    private TextureRect weaponImage;
    
    public override void _Ready()
    {
        weaponImage = GetNode<TextureRect>("WeaponImage");
        playerShip.WeaponEquipped += HandleWeaponEquipped;
        HandleWeaponEquipped(playerShip.GetEquippedWeapon());
    }

    private void HandleWeaponEquipped(BaseWeapon weapon)
    {
        weaponImage.Texture = weapon.GetWeaponImage();
    }
}
