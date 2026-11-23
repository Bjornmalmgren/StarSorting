using Godot;
using System;

public partial class StarPrototype : Node3D
{
	[Signal]
	public delegate void MouseEnteredEventHandler(int index);
    [Signal]
    public delegate void MouseExitedEventHandler(int index);
    [Export]
    public int index;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		(GetChild(0).GetChild(0) as Area3D).MouseEntered += MouseEnteredStar;
        (GetChild(0).GetChild(0) as Area3D).MouseExited += MouseExitedStar;
        index = GetNode<StarMapPrototype1>("/root/StarMapPrototype1").Index;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void MouseEnteredStar()
    {
		EmitSignal(SignalName.MouseEntered, index);
    }

    public void MouseExitedStar()
    {
        EmitSignal(SignalName.MouseExited, index);
    }

}
