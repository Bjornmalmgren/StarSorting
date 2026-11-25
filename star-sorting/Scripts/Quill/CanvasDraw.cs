using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class CanvasDraw : Node2D
{

	bool InDrawMode = true;

	Vector2 LastMousePos;
    Vector2 CurrentMousePosition;
	// Called when the node enters the scene tree for the first time.
	Array<Vector2> StartPositionsLine;
    Array<Vector2> EndPositionsLine;

    Array<Vector2> PositionCircle;
    Array<float> RadiusCircle;
    
    Vector2 CircleIndicatorPosition;
    float CircleIndicatorRadius;

    int SelectedBrush = -1;
    int SelectedColor = -1;
    Vector3 BrushColor;

    public override void _Ready()
	{
        StartPositionsLine = new Array<Vector2>();
        EndPositionsLine = new Array<Vector2>();
        PositionCircle = new Array<Vector2>();
        RadiusCircle = new Array<float>();
        CircleIndicatorPosition = Vector2.Zero;
        CircleIndicatorRadius = -1;

        

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("change_to_drawing_mode"))
        {
            InDrawMode = !InDrawMode;
        }
        if(GetNode<OptionButton>("Menu/Brush/BrushOptions").Selected != SelectedBrush)
        {
            SelectedBrush = GetNode<OptionButton>("Menu/Brush/BrushOptions").Selected;
        }

        if (GetNode<OptionButton>("Menu/Color/OptionButton").Selected != SelectedColor)
        {
            SelectedColor = GetNode<OptionButton>("Menu/Color/OptionButton").Selected;
            ColorPicker();
        }

        LastMousePos = CurrentMousePosition;
        CurrentMousePosition = GetGlobalMousePosition();
        QueueRedraw();
		
    }

    void ColorPicker()
    {
        switch(SelectedColor)
        {
            case 0:
                //emerald
                BrushColor = new Vector3(69, 203, 133);
                break;
            case 1:
                //neon ice
                BrushColor = new Vector3(87, 226, 229);
                break;
            case 2:
                //cerulean
                BrushColor = new Vector3(0, 129, 167);
                break;
            case 3:
                //Harvest gold
                BrushColor = new Vector3(218, 153, 47);
                break;
            case 4:
                //pink mist
                BrushColor = new Vector3(221, 154, 194);
                break;
        }
    }

    public void ClearDrawing()
    {
        StartPositionsLine.Clear();
        EndPositionsLine.Clear();
        PositionCircle.Clear();
        RadiusCircle.Clear();
    }

    public override void _Draw()
    {
        base._Draw();
		if(InDrawMode && Input.IsActionPressed("mouse_left_click"))
		{
            if(SelectedBrush == 0)
            {
                //line
                StartPositionsLine.Add(LastMousePos);
                EndPositionsLine.Add(CurrentMousePosition);
            }

            if (SelectedBrush == 1)
            {
                //square
                CircleIndicatorRadius = (float)Math.Sqrt(Math.Pow(CurrentMousePosition.X - CircleIndicatorPosition.X, 2) + Math.Pow(CurrentMousePosition.Y - CircleIndicatorPosition.Y, 2));
            }


        }

        //permanent circle
        if (InDrawMode && Input.IsActionJustPressed("mouse_left_click"))
        {
            if (SelectedBrush == 1)
            {
                //circle
                PositionCircle.Add(CurrentMousePosition);
                CircleIndicatorPosition = CurrentMousePosition;
            }
        }
        if (InDrawMode && Input.IsActionJustReleased("mouse_left_click"))
        {
            if (SelectedBrush == 1 && CircleIndicatorPosition != Vector2.Zero)
            {
                //circle
                if(PositionCircle.Count != 0)
                {
                    float distance = (float)Math.Sqrt(Math.Pow(CurrentMousePosition.X - PositionCircle[PositionCircle.Count - 1].X, 2) + Math.Pow(CurrentMousePosition.Y - PositionCircle[PositionCircle.Count - 1].Y, 2));
                    
                    RadiusCircle.Add(distance);
                    CircleIndicatorPosition = Vector2.Zero;
                    CircleIndicatorRadius = -1;
                }
                
            }
        }



        for (int i = 0; i < StartPositionsLine.Count; i++)
		{
			if(i % 2 != 0)
			{
                DrawDashedLine(StartPositionsLine[i], EndPositionsLine[i], new Color(BrushColor.X/255.0f, BrushColor.Y / 255.0f, BrushColor.Z / 255.0f, 1), 1.0f, 2, true, true);
				
            }
            
			
        }
       
        for (int i = 0; i < PositionCircle.Count; i++)
        {
            if(PositionCircle.Count != RadiusCircle.Count && i == PositionCircle.Count - 1)
            {
                break;
            } 
            
            DrawCircle(PositionCircle[i], RadiusCircle[i], new Color(BrushColor.X / 255.0f, BrushColor.Y / 255.0f, BrushColor.Z / 255.0f, 1), false, 1, true);
            
        }
        
        if(CircleIndicatorPosition != Vector2.Zero && CircleIndicatorRadius != -1)
        {
            DrawCircle(CircleIndicatorPosition, CircleIndicatorRadius, new Color(BrushColor.X / 255.0f, BrushColor.Y / 255.0f, BrushColor.Z / 255.0f, 0.5f), false, 1, true);
            
        }
       
        
    }
}
