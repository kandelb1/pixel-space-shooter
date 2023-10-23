using Godot;
using System;

public partial interface IPowerup
{
    public Texture2D GetIcon();
    
    public float GetTimeRemaining();
}
