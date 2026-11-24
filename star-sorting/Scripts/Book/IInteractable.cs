using Godot;
using System;

public  interface IInteractable
{
    string promptMessage { get; }
    bool canInteract { get; }
    void Interact(Node interactor);
    void ShowPrompt();
    void HidePrompt();

}
