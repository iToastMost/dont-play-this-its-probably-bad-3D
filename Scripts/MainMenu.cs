using Godot;
using System;

public partial class MainMenu : Control
{
    private Button _startButton;
    private Button _quitButton;
    private Button _sandboxButton;
    private Button _loadButton;
    
    private Node _thisScene;
    private Node _gameScene;
    public override void _Ready() 
    {
        _startButton = GetNode<Button>("StartButton");
        _quitButton = GetNode<Button>("QuitButton");
        _sandboxButton = GetNode<Button>("SandboxButton");
        _loadButton = GetNode<Button>("LoadGameButton");
        
        _thisScene = GetNode<Node>("/root/MainMenu");
        

        _startButton.Pressed += StartGame;
        _sandboxButton.Pressed += SandboxButtonPressed;
        _loadButton.Pressed += LoadGame;
        _quitButton.Pressed += QuitGame;
    }

    private void StartGame() 
    {
        if(GetTree() != null) 
        {
            //GameScene = ResourceLoader.Load<PackedScene>("res://Scenes/GameManager.tscn").Instantiate();
            GetTree().ChangeSceneToFile("res://Scenes/GameManager.tscn");
            // GetTree().Root.AddChild(_gameScene);
            // ThisScene.QueueFree();
        }
    }

    private void SandboxButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Environments/Sandbox.tscn");
    }

    private void LoadGame()
    {
        SaveFileManager.LoadGame();
    }
    
    private void QuitGame() 
    {
        if(GetTree() != null)
         GetTree().Quit();
    }

}
