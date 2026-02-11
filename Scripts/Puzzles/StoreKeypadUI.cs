using Godot;
using System;

public partial class StoreKeypadUI : Control
{
    [Signal]
    public delegate void SendCodeEventHandler(string code);
    
    private Button _button5;
    private Button _buttonDel;
    private Button _poundButton;
    
    private RichTextLabel _code;
    private string _enteredText;
    
    
    public override void _Ready()
    {
        _code = GetNode<RichTextLabel>("CanvasLayer/VBoxContainer/CodeDisplay/RichTextLabel");
        _button5 = GetNode<Button>("CanvasLayer/VBoxContainer/Buttons4to6/Button5");
        _buttonDel = GetNode<Button>("CanvasLayer/VBoxContainer/Buttons0toEnter/ButtonDelete");
        _poundButton = GetNode<Button>("CanvasLayer/VBoxContainer/Buttons0toEnter/ButtonPound");
        _button5.GrabFocus();

        var buttons = GetTree().GetNodesInGroup("Buttons0to9");
        
        foreach (Button button in buttons)
        {
            button.Pressed += () => ButtonPressed(button);
        }
        
        _enteredText = "";
        _buttonDel.Pressed += DeletePressed;
        _poundButton.Pressed += PoundPressed;
    }

    private void ButtonPressed(Button button)
    {
        //ReplaceAsterisk();
        if(_enteredText.Length < 4)
            _enteredText += button.Text;
        _code.SetText(_enteredText);
    }

    private void DeletePressed()
    {
        if(_enteredText.Length > 0)
            _enteredText = _enteredText.Substring(0, _enteredText.Length - 1);
        _code.SetText(_enteredText);
    }

    private void PoundPressed()
    {
        if(_enteredText.Length == 4)
            EmitSignal(SignalName.SendCode, _enteredText);
        
        QueueFree();
    }

    private void ReplaceAsterisk()
    {
        for (int i = 0; i < _enteredText.Length - 1; i++)
        {
            if (_enteredText[i] == '*')
            {
                _code.SetText(_enteredText);
                break;
            }
        }
    }
}
