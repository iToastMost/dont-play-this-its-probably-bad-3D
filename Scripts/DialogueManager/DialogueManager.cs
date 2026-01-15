using Godot;
using System;

public partial class DialogueManager : Node
{
    public static DialogueManager Instance { get; private set; }
    
    private Dialogue _dialogue;
    private string _dialogueText;

    public override void _Ready()
    {
        Instance = this;
    }
    
    public void ShowDialogue(string name, string dialogueText)
    {
        _dialogue = GetParent().GetNode<Dialogue>("GameManager/PlayerSetup/UI/CanvasLayer/Dialogue");
        _dialogueText = dialogueText;
        _dialogue.ShowMessage(name, _dialogueText);
    }

    public void HideDialogue()
    {
        _dialogue = GetParent().GetNode<Dialogue>("GameManager/PlayerSetup/UI/CanvasLayer/Dialogue");
        _dialogue.HideMessage();
    }
    
}
