using Godot;
using System;

public partial class StarPrototype : Node3D
{
	[Export]
	public string StarName;
    [Export]
    public int index;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

        index = GetParent().GetParent().GetParent().GetNode<StarMapPrototype1>("StarMapPrototype1").Index;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}



}
