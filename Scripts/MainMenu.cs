using Godot;
using System;

public partial class MainMenu : Control
{
    [Signal]
    public delegate void NewGameButtonEventHandler();
    
    [Signal]
    public delegate void SandboxButtonEventHandler();
    
    [Signal]
    public delegate void LoadGameButtonEventHandler();
    
    [Signal]
    public delegate void QuitButtonEventHandler();
    
    private Button _newGameButton;
    private Button _quitButton;
    private Button _sandboxButton;
    private Button _loadButton;
    
    private Node _thisScene;
    private Node _gameScene;
    public override void _Ready() 
    {
        _newGameButton = GetNode<Button>("NewGameButton");
        _quitButton = GetNode<Button>("QuitButton");
        _sandboxButton = GetNode<Button>("SandboxButton");
        _loadButton = GetNode<Button>("LoadGameButton");
        
        //_thisScene = GetNode<Node>("/root/MainMenu");
        _newGameButton.GrabFocus();
        

        _newGameButton.Pressed += NewGame;
        _sandboxButton.Pressed += SandboxButtonPressed;
        _loadButton.Pressed += LoadGame;
        _quitButton.Pressed += QuitGame;
    }

    private void NewGame()
    {
        EmitSignal(SignalName.NewGameButton);
        // if(GetTree() != null) 
        // {
        //     //GameScene = ResourceLoader.Load<PackedScene>("res://Scenes/GameManager.tscn").Instantiate();
        //     GetTree().ChangeSceneToFile("res://Scenes/GameManager.tscn");
        //     // GetTree().Root.AddChild(_gameScene);
        //     // ThisScene.QueueFree();
        // }
    }

    private void SandboxButtonPressed()
    {
        EmitSignal(SignalName.SandboxButton);
        // GetTree().ChangeSceneToFile("res://Scenes/Environments/Sandbox.tscn");
    }

    private void LoadGame()
    {
        EmitSignal(SignalName.LoadGameButton);
        // SaveFileManager.LoadGame();
    }
    
    private void QuitGame() 
    {
        EmitSignal(SignalName.QuitButton);
        // if(GetTree() != null)
        //  GetTree().Quit();
    }

}
