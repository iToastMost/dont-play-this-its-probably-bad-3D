using Godot;
using System;

public partial class StoreKeypadUI : Control
{
    private Button _button5;
    private RichTextLabel _code;
    private string _enteredText;
    private Button _buttonDel;
    
    public override void _Ready()
    {
        _code = GetNode<RichTextLabel>("CanvasLayer/VBoxContainer/CodeDisplay/RichTextLabel");
        _button5 = GetNode<Button>("CanvasLayer/VBoxContainer/Buttons4to6/Button5");
        _buttonDel = GetNode<Button>("CanvasLayer/VBoxContainer/Buttons0toEnter/ButtonDelete");
        _button5.GrabFocus();

        _enteredText = "";
        
        _button5.Pressed += () => ButtonPressed(_button5);
        _buttonDel.Pressed += DeletePressed;
    }

    private void ButtonPressed(Button button)
    {
        //ReplaceAsterisk();
        if(_enteredText.Length < 4)
            _enteredText += _button5.Text;
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
