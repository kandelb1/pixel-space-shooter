using Godot;
using System;

public partial class ScoreScreen : PanelContainer
{
    private Label scoreLabel;
    private Button quitButton;
    private Button restartButton;

    public int GameScore { get; set; }

    public override void _Ready()
    {
        scoreLabel = GetNode<Label>("%ScoreLabel");
        UpdateScoreText(GameScore);
        
        quitButton = GetNode<Button>("%QuitButton");
        quitButton.Pressed += HandleQuitPressed;
        
        restartButton = GetNode<Button>("%RestartButton");
        restartButton.Pressed += HandleRestartPressed;
    }

    private void UpdateScoreText(int value)
    {
        scoreLabel.Text = $"Your score was: {value}";
    }

    private void HandleQuitPressed()
    {
        GetTree().Quit();
    }

    private void HandleRestartPressed()
    {
        // couldn't use ChangeSceneToPacked with an [Export] PackedScene for some reason
        GetTree().ChangeSceneToFile("res://Scenes/Main/Main.tscn");
    }
}
