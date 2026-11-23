using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public struct GridSquare
{
	public Vector2 TLCorner;
    public Vector2 DRCorner;

	public string Designation;

	public GridSquare(Vector2 TLC, Vector2 DRC) { 
		TLCorner = TLC;
		DRCorner = DRC;
	}

	public void SetDesignation(string NewDesignation)
	{
		Designation = NewDesignation;
	}
}

public partial class Grid : MeshInstance3D
{
	

    
    GridSquare[] GridSquares;
	[Export]
	float GridSize = 1.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//make the grid
		var scale = Scale;


		GridSquares = new GridSquare[(int)(scale.X * scale.Y)];


		int Index = 0;

        for (int i = 0; i+1 < Math.Abs(scale.Y); i++)
		{
            for (int j = 0; j+1 < Math.Abs(scale.X); j++)
            {
                
                GridSquares[Index] =(new GridSquare(new Vector2(j * GridSize, i * GridSize), new Vector2(j+1 * GridSize, i+1 * GridSize)));
				string S = i.ToString();
				char c = (char)(65 + j);
				S = S + c.ToString();
                GridSquares[GridSquares.Length - 1].Designation = S;
				Index++;
            }
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
