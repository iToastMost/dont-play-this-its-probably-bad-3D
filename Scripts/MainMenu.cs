using Godot;
using System;

public partial class MainMenu : Control
{
    Button startButton;
    Button quitButton;

    private Node ThisScene;
    private Node GameScene;
    public override void _Ready() 
    {
        startButton = GetNode<Button>("StartButton");
        quitButton = GetNode<Button>("QuitButton");
        
        ThisScene = GetNode<Node>("/root/MainMenu");
        

        startButton.Pressed += StartGame;
        quitButton.Pressed += QuitGame;
    }

    private void StartGame() 
    {
        if(GetTree() != null) 
        {
            //GameScene = ResourceLoader.Load<PackedScene>("res://Scenes/GameManager.tscn").Instantiate();
            GetTree().ChangeSceneToFile("res://Scenes/GameManager.tscn");
            // GetTree().Root.AddChild(GameScene);
            // ThisScene.QueueFree();
        }
        
    }

    private void QuitGame() 
    {
        if(GetTree() != null)
         GetTree().Quit();
    }

}
