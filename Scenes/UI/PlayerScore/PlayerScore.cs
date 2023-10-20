using Godot;
using System;

public partial class PlayerScore : HBoxContainer
{
    private Label scoreLabel;
    
    public override void _Ready()
    {
        scoreLabel = GetNode<Label>("Score");
        GameManager.Instance.ScoreUpdated += UpdateScoreText;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreLabel.Text = GameManager.Instance.Score.ToString();
    }
}
