using Godot;
using System;

public partial class Main : Node
{
    public override void _Ready()
    {
        GameManager gameManager = GetNode<GameManager>("GameManager");
        gameManager.StartGame();
    }
}
