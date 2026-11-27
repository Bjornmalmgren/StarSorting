using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GridSquare :Resource
{
    //stands for Bottom left corner
    [Export]
	public Vector2 BLCorner;
    //stands for Top right corner
    [Export]
    public Vector2 TRCorner;

	public string Designation;


	public (string, int)[] Neighbours;

	public GridSquare(Vector2 TLC, Vector2 DRC) {
        BLCorner = TLC;
        TRCorner = DRC;
	}
}

public partial class Grid : MeshInstance3D
{


    //the grid squares for the object, its based on scale as of now
    string GridLinePath = "res://Scenes/StarMapPrototype/GridLine.tscn";
    [ExportCategory("Variables")]
    [Export]
    public GridSquare[] GridSquares;
    public Array<Node3D> GridLines;
	[Export]
	public float GridSize = 4.0f;
    [Export]
    public int Index = 0;
    public Vector2 scale = new Vector2(22, 14);
    [Export]
    Vector2 StartingPosition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //make the grid
        scale = new Vector2(Scale.X, Scale.Y);
        scale /= GridSize;
        GridSquares = new GridSquare[(int)((scale.X * scale.Y))];
        StartingPosition = new Vector2(GlobalPosition.X- Scale.X * 0.5f, GlobalPosition.Y - Scale.Y *0.5f) ;
        GridLines = new Array<Node3D>();

        for (int i = 0; i < Math.Abs(scale.Y); i ++)
		{
            for (int j = 0; j < Math.Abs(scale.X); j++)
            {
                Vector2 LC, RC;

                LC = new Vector2(StartingPosition.X + j * GridSize, StartingPosition.Y + i * GridSize);

                RC = new Vector2(StartingPosition.X + (j + 1) * GridSize, StartingPosition.Y + (i + 1) * GridSize);
                GridSquares[Index] = (new GridSquare(LC, RC));
				GridSquares[Index].Neighbours = new (string, int)[8];
                //extremly bad way to do it and it looks horrendus but works
                //assigns the neightbours that each grid has
                if (j != 0)
				{
					//left
					GridSquares[Index].Neighbours[0] = ("LEFT", Index - 1);
                
                }
                if (j != Math.Abs(scale.X) -1)
                {
                    //right
                    GridSquares[Index].Neighbours[1] = ("RIGHT", Index + 1);
                }
				if(i != 0)
				{
                    //up
                    GridSquares[Index].Neighbours[2] = ("DOWN", Index - (int)Math.Abs(scale.X));
                }
				if(i != Math.Abs(scale.Y) - 1)
				{
                    //down
                    GridSquares[Index].Neighbours[3] = ("UP", Index + (int)Math.Abs(scale.X));
                }
                if (j != 0 && i != Math.Abs(scale.Y) - 1)
                {
                    //up left
                    GridSquares[Index].Neighbours[4] = ("CORNER_LT", Index + (int)Math.Abs(scale.X) - 1);
                
                }
                if (j != Math.Abs(scale.X) - 1 && i != 0)
                {
                    //up right
                    GridSquares[Index].Neighbours[5] = ("CORNER_RT", Index + (int)Math.Abs(scale.X) + 1);
                
                }
                
                if (j != 0 && i != 0)
                {
                    //down left
                    GridSquares[Index].Neighbours[6] = ("CORNER_LD", Index - (int)Math.Abs(scale.X) - 1);
                
                }
                if (j != Math.Abs(scale.X) - 1 && i != 0)
                {
                    //down right
                    GridSquares[Index].Neighbours[7] = ("CORNER_RD", Index - (int)Math.Abs(scale.X) + 1);
                
                }

                //makes i aka the y level into the Number and j aka the x value into the letter
                string S = i.ToString();
				char c = (char)(65 + j);
				S = S + c.ToString();
                GridSquares[Index].Designation = S;
				Index++;

                
            }
            
        }
        //foreach (GridSquare square in GridSquares)
        //{
        //    foreach (var N in square.Neighbours)
        //    {
        //        GD.Print(N);
        //    }
        //}

        PackedScene GridLineScene = GD.Load<PackedScene>(GridLinePath);


        //make the gridlines along the x axis
        for (int i = 0; i < scale.Y; i++)
        {
            //instatiate meshes
            var Node = GridLineScene.Instantiate() as Node3D;
        
            GridLines.Add(Node);
            GetParent().CallDeferred(Node3D.MethodName.AddChild, GridLines.Last<Node3D>());



            GridLines.Last<Node3D>().Scale = new Vector3(scale.X * GridSize,0.2f, 1);
            GridLines.Last<Node3D>().Position += new Vector3(scale.X * GridSize * 0.5f,i * GridSize, -10.5f);
        }
        
        for (int i = 0; i < scale.X; i++)
        {
            //instatiate meshes
            var Node = GridLineScene.Instantiate() as Node3D;
        
            GridLines.Add(Node);
            GetParent().CallDeferred(Node3D.MethodName.AddChild, GridLines.Last<Node3D>());
        
        
            GridLines.Last<Node3D>().Scale = new Vector3(0.2f, scale.Y * GridSize, 1);
            GridLines.Last<Node3D>().Position += new Vector3(i * GridSize, scale.Y * GridSize * 0.5f, -10.5f);
        
        }
    }



	public string[] PostionsToGrid(Vector2[] Positions)
	{

        //finds the designation for each position sent in, currently used by the constellations to know where a star is located
		string[] ReturnGrid = new string[Positions.Length];

        for (int i = 0;	i < Positions.Length; i++)
		{
            int StartIndexX = 0;
            int StartIndexY = 0;
            
            //while loops that run until they find the correct position
            while (Positions[i].X > GridSquares[StartIndexX].TRCorner.X)
            {
                //move to the right
                StartIndexX++;

            }
            while (Positions[i].Y > GridSquares[StartIndexY * (int)scale.X].TRCorner.Y)
            {
                //move to the right
                StartIndexY++;
            }

            ReturnGrid[i] = GridSquares[StartIndexX + StartIndexY * (int)scale.X].Designation;
        }

		return ReturnGrid;
	}

	public int GridPosition(int CurrentIndex, Vector2 Position)
	{
		int NewIndex = CurrentIndex;
        //this is used by the player to tell them which square they are looking at, it checks the current position of the camera in refrence to all neighbours of the current square
        //if it finds that the player moved to a new one it finds which one based where the new position is located in relation to the two corners of the grid
        if( Position.X > GridSquares[CurrentIndex].TRCorner.X)
		{
			if(Position.X > GridSquares[CurrentIndex].TRCorner.X)
			{
                //right
                NewIndex = GridSquares[CurrentIndex].Neighbours[1].Item2;
                
            
            }
            else if (Position.Y < GridSquares[CurrentIndex].BLCorner.Y)
            {
                //bottom right
                NewIndex = GridSquares[CurrentIndex].Neighbours[7].Item2;
			}
			else
			{
                //top right
                NewIndex = GridSquares[CurrentIndex].Neighbours[5].Item2;
            }
        }
        else if (Position.X < GridSquares[CurrentIndex].BLCorner.X)
		{
            if (Position.X < GridSquares[CurrentIndex].BLCorner.X)
            {
                //left
                
                NewIndex = GridSquares[CurrentIndex].Neighbours[0].Item2;

            }
            else if (Position.Y < GridSquares[CurrentIndex].BLCorner.Y)
            {
                //bottom left
                NewIndex = GridSquares[CurrentIndex].Neighbours[6].Item2;
            }
            else
            {
                //top left
                NewIndex = GridSquares[CurrentIndex].Neighbours[4].Item2;
            }

        }
        else if(Position.Y > GridSquares[CurrentIndex].TRCorner.Y)
        {
            //up
            //GD.Print("up");
            NewIndex = GridSquares[CurrentIndex].Neighbours[3].Item2;
        }
        else if (Position.Y < GridSquares[CurrentIndex].BLCorner.Y)
        {
            //down
            //GD.Print("down");
            NewIndex = GridSquares[CurrentIndex].Neighbours[2].Item2;
        }

        return NewIndex;


    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{


    }
}
