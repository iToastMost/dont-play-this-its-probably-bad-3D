using Godot;
using System;

public partial class DeathScreenUI : Node
{
    [Signal]
    public delegate void LoadGameButtonPressedEventHandler();
    
    [Signal]
    public delegate void MainMenuButtonPressedEventHandler();
    
    [Signal]
    public delegate void QuitButtonPressedEventHandler();
    
    private Button _loadGameButton;
    private Button _mainMenuButton;
    private Button _quitGameButton;
    private CanvasLayer _canvasLayer;

    public override void _Ready()
    {
        _loadGameButton = GetNode<Button>("CanvasLayer/HBoxContainer/LoadGame");
        _mainMenuButton = GetNode<Button>("CanvasLayer/HBoxContainer/MainMenu");
        _quitGameButton = GetNode<Button>("CanvasLayer/HBoxContainer/QuitGame");
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");

        _canvasLayer.Visible = false;
        
        _loadGameButton.Pressed += LoadGamePressed;
        _mainMenuButton.Pressed += MainMenuPressed;
        _quitGameButton.Pressed += QuitGamePressed;
    }

    public void ShowDeathScreen()
    {
        _canvasLayer.Visible = true;
        _loadGameButton.GrabFocus();
    }
    
    public void HideDeathScreen()
    {
        _canvasLayer.Visible = false;
    }
    private void LoadGamePressed()
    {
        EmitSignal(SignalName.LoadGameButtonPressed);
        HideDeathScreen();
    }

    private void MainMenuPressed()
    {
        EmitSignal(SignalName.MainMenuButtonPressed);
        HideDeathScreen();
    }

    private void QuitGamePressed()
    {
        EmitSignal(SignalName.QuitButtonPressed);
    }
    
    
}
