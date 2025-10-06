using Godot;
using System;

public partial class MainMenu : Control
{
    Button startButton;
    Button quitButton;
    public override void _Ready() 
    {
        startButton = GetNode<Button>("StartButton");
        quitButton = GetNode<Button>("QuitButton");

        startButton.Pressed += StartGame;
        quitButton.Pressed += QuitGame;
    }

    private void StartGame() 
    {
        GetTree().ChangeSceneToFile("res://Scenes/GameManager.tscn");
    }

    private void QuitGame() 
    {
        GetTree().Quit();
    }

}
