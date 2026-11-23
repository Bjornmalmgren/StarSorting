using Godot;
using System;

public interface IPickable 
{
    bool CanPickUp { get; }
    void OnPickUp(Node3D holder);
    void OnDrop();
}
