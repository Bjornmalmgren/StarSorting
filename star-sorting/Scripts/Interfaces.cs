using Godot;
using System;

public interface IInteractable
{
    string promptMessage { get; }
    bool canInteract { get; }
    void Interact(Player player);
    void ShowPrompt();
    void HidePrompt();
}

public interface IPickable
{
    bool CanPickUp { get; }
    void OnPickUp(Node3D holder);
    void OnDrop();
}

public interface IReadable
{
    void OpenReadingUI();
    void CloseReadingUI();
}
