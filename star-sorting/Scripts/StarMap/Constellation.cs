using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Constellation : Node3D
{
    // Called when the node enters the scene tree for the first time.

    string StarPath = "res://Scenes/StarMapPrototype/Star.tscn";

    [Export]
    int StarsInConstellation;
    [Export]
    Vector2[] CorrectPositions;
    [Export]
    string[] CorrectStars;
    [Export]
    Array<Node3D> Stars;

    public override void _Ready()
	{
        //CorrectPositions = new Vector2[StarsInConstellation];
        //CorrectStars = new string[StarsInConstellation];
        CreateStars();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    void CreateStars()
    {
        PackedScene StarScene = GD.Load<PackedScene>(StarPath);
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Seed = 12441512;
        for (int i = 0; i < StarsInConstellation; i++)
        {
            
            Stars.Add(StarScene.Instantiate() as Node3D);
            AddChild(Stars.Last<Node3D>());

            Godot.Vector3 Pos = new Godot.Vector3(rand.RandfRange(7, -7), rand.RandfRange(4, -4), -10);
            Stars.Last<Node3D>().GlobalPosition = Pos;
            Stars.Last<Node3D>().Scale *= 0.5f;
            (Stars.Last<Node3D>() as StarPrototype).StarName = CorrectStars[i];
        }
    }

    public void CheckCorrectPositions()
    {
        int CurrentlyCorrectPositions = 0;
        foreach (var star in Stars)
        {

            foreach (var pos in CorrectPositions)
            {
                if (star.Position.DistanceTo(new Vector3(pos.X, pos.Y, -6)) < 0.2f)
                {

                    CurrentlyCorrectPositions++;
                }

            }

        }
        if (CurrentlyCorrectPositions == StarsInConstellation)
        {
            //constellation done lock the stars positions
            GD.Print("Constellation is done");
            foreach (var star in Stars)
            {

                foreach (var pos in CorrectPositions)
                {
                    if (star.Position.DistanceTo(new Vector3(pos.X, pos.Y, -6)) < 0.2f)
                    {
                        star.Position = new Vector3(pos.X, pos.Y, -6);
                        star.SetMeta("_edit_lock_", true);
                    }


                }

            }
        }
    }
}
