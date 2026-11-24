using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public struct GridSquare
{
	public Vector2 TLCorner;
    public Vector2 DRCorner;

	public string Designation;

	public (string, int)[] Neighbours;

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
	

    //the grid squares for the object, its based on scale as of now
    public GridSquare[] GridSquares;
	public System.Collections.Generic.Dictionary<(Vector2 LC, Vector2 RC), string> GridDictonary;
	[Export]
	float GridSize = 1.0f;
    [Export]
    int Index = 0;
    Vector2 scale = new Vector2(22, 14);
    Vector2 StartingPosition = new Vector2(0, 0);
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //make the grid

        scale /= GridSize;
        GridDictonary = new System.Collections.Generic.Dictionary<(Vector2 LC, Vector2 RC), string>();
        GridSquares = new GridSquare[(int)((scale.X * scale.Y))];

		

        for (int i = 0; i < Math.Abs(scale.Y); i ++)
		{
            for (int j = 0; j < Math.Abs(scale.X); j++)
            {
                
                Vector2 LC, RC;

                LC = new Vector2(StartingPosition.X + j * GridSize, StartingPosition.Y + i * GridSize);

                RC = new Vector2(StartingPosition.X + (j + GridSize) * GridSize, StartingPosition.Y + (i + GridSize) * GridSize);
                GridSquares[Index] = (new GridSquare(LC, RC));
				GridSquares[Index].Neighbours = new (string, int)[8];
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

                //GD.Print(LC + " : " + RC);
                string S = i.ToString();
				char c = (char)(65 + j);
				S = S + c.ToString();
                GridSquares[Index].Designation = S;
				//GD.Print(Index, S);
				GridDictonary.Add((LC, RC), S);
				Index++;

                
            }
            
        }
        GD.Print(Index);
        //foreach(var a in GridSquares[76].Neighbours){
        //    GD.Print(a);
        //}



        //for (int i = 0; 0 < GridSquares.Length; i++)
        //{
        //	//if (i-VerticalAmount > 0)
        //	//{
        //	//	//upwards
        //	//	GD.Print("up");
        //	//
        //	//}
        //    //if (i - VerticalAmount < GridSquares.Length)
        //    //{
        //    //    //downwards
        //    //    GD.Print("down");
        //    //}
        //	//if((i-CurrentCollum*HorizontalAmount)-1 > 0)
        //	//{
        //    //    //left
        //    //    GD.Print("left");
        //    //}
        //    //if ((i - CurrentCollum * HorizontalAmount) + 1 < HorizontalAmount)
        //    //{
        //    //    //right
        //    //    GD.Print("right");
        //    //}
        //
        //    if (i == HorizontalAmount)
        //	{
        //		CurrentCollum++;
        //	}
        //	
        //}
    }

	public string[] PostionsToGrid(Vector2[] Positions)
	{
		string[] ReturnGrid = new string[Positions.Length];

        

        for (int i = 0;	i < Positions.Length; i++)
		{
            int StartIndexX = 0;
            int StartIndexY = 0;
            while (Positions[i].X > GridSquares[StartIndexX].DRCorner.X)
            {
                //move to the right
                StartIndexX++;
            }
            while (Positions[i].Y > GridSquares[StartIndexY * (int)scale.X].DRCorner.Y)
            {
                //move to the right
                StartIndexY++;
            }
            //GD.Print(StartIndexX + StartIndexY * (int)scale.X);
            ReturnGrid[i] = GridSquares[StartIndexX + StartIndexY * (int)scale.X].Designation;
        }
		return ReturnGrid;
	}

	public int GridPosition(int CurrentIndex, Vector2 Position)
	{
		int NewIndex = CurrentIndex;
        if( Position.X > GridSquares[CurrentIndex].DRCorner.X)
		{
			if(Position.Y > GridSquares[CurrentIndex].TLCorner.Y)
			{
                //right
                NewIndex = GridSquares[CurrentIndex].Neighbours[1].Item2;
                
            
            }
            else if (Position.Y < GridSquares[CurrentIndex].DRCorner.Y)
            {
                //bottom right
                NewIndex = GridSquares[CurrentIndex].Neighbours[7].Item2;
			}
			else
			{
                //top right
                if (GridSquares[CurrentIndex].Neighbours[5].Item2 != 0)
                    NewIndex = GridSquares[CurrentIndex].Neighbours[5].Item2;
            }
            //NewIndex = GridSquares[CurrentIndex].Neighbours[1].Item2;
        }
        else if (Position.X < GridSquares[CurrentIndex].TLCorner.X)
		{
            if (Position.Y > GridSquares[CurrentIndex].TLCorner.Y)
            {
                //left
                
                NewIndex = GridSquares[CurrentIndex].Neighbours[0].Item2;

            }
            else if (Position.Y < GridSquares[CurrentIndex].TLCorner.Y)
            {
                //bottom left
                NewIndex = GridSquares[CurrentIndex].Neighbours[6].Item2;
            }
            else
            {
                //top left
                NewIndex = GridSquares[CurrentIndex].Neighbours[4].Item2;
            }
            //NewIndex = GridSquares[CurrentIndex].Neighbours[0].Item2;
        }
        else if(Position.Y > GridSquares[CurrentIndex].DRCorner.Y)
        {
            //up
            GD.Print("up");
            NewIndex = GridSquares[CurrentIndex].Neighbours[3].Item2;
        }
        else if (Position.Y < GridSquares[CurrentIndex].TLCorner.Y)
        {
            //down
            GD.Print("down");
            NewIndex = GridSquares[CurrentIndex].Neighbours[2].Item2;
        }

        return NewIndex;


    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
