using Godot;
using System;

public interface IInteractable
{
    string promptMessage { get; }
    bool canInteract { get; }
    void Interact();
    void ShowPrompt();
    void HidePrompt();
}

public interface IPickable
{
    bool CanPickUp { get; }
    void OnPickUp(Node3D holder);
    void OnDrop();
}


