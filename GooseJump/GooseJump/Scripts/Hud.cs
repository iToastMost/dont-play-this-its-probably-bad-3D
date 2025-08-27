using Godot;
using System;

public partial class Hud : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();

	private float highScore;

	public void ShowGameOver()
	{
		ShowHud();
	}

	private void OnStartButtonPressed()
	{
		HideHud();
        EmitSignal(SignalName.StartGame);
	}

	public void HideHud()
	{
        GetNode<Button>("StartButton").Hide();
        GetNode<Label>("StartGameMessage").Hide();
        GetNode<ColorRect>("ColorRect").Hide();
		GetNode<Label>("HighScore").Hide();
        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);
    }

	public void ShowHud() 
	{
        GetNode<Label>("HighScore").Text = $"Highscore: {highScore}";
        GetNode<Label>("StartGameMessage").Show();
        GetNode<Button>("StartButton").Show();
        GetNode<ColorRect>("ColorRect").Show();
		GetNode<Label>("HighScore").Show();
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
    }

	public void UpdateScore(float score)
	{
		GetNode<Label>("Score").Text = $"Score: {score}";
		if(score > highScore) 
		{
			highScore = score;
		}
	}
}
