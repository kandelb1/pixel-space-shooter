using Godot;
using System;

public partial class ScoreComponent : Node
{
    [Export] public int ScoreValue { get; private set; }
}
