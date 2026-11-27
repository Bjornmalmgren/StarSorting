using Godot;
using Godot.Collections;
using System;

public partial class GameEvents : Node
{
    public static GameEvents Instance { get; private set; }

    [Signal]
    public delegate void ReadingStartedEventHandler();

    [Signal]
    public delegate void ReadingEndedEventHandler();

    [Signal]
    public delegate void StarmapStartedEventHandler();

    [Signal]
    public delegate void StarmapEndedEventHandler();
    [Signal]
    public delegate void ConstellationDoneEventHandler(string[] OrderToDrawBetweenStars, Vector2[] Positions, Array<Node3D> Stars);


    public override void _Ready()
    {
        Instance = this;
    }

    public void EmitReadingStarted()
    {
        EmitSignal(SignalName.ReadingStarted);
    }

    public void EmitReadingEnded()
    {
        EmitSignal(SignalName.ReadingEnded);
    }

    public void EmitStarmapStarted()
    {
        EmitSignal(SignalName.StarmapStarted);
    }

    public void EmitStarmapEnded()
    {
        EmitSignal(SignalName.StarmapEnded);
    }

    public void EmitConstellationDone(string[] OrderToDrawBetweenStars, Vector2[] Positions, Array<Node3D> Stars)
    {
        EmitSignal(SignalName.ConstellationDone, OrderToDrawBetweenStars,Positions,Stars);
    }
}
