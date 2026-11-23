using Godot;
using System;

public  interface IInteractable
{
    string PromptMessage { get; }
    bool CanInteract { get; }
    void Interact(Node interactor);

}
