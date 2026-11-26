using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Line : Resource
{
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public Vector3 Color;
    public Vector3 WorldSpaceDrawnIn;

   
}

public partial class Circle : Resource
{
    public Vector2 Center;
    public float Radius;
    public Vector3 Color;

    public Vector3 WorldSpaceDrawnIn;
}

public partial class CanvasDraw : Node2D
{

	bool InDrawMode = false;

	Vector2 LastMousePos;
    Vector2 CurrentMousePosition;
    
    [Export]
    bool CanDraw = true;
    Array<Line> Lines;
    Array<Circle> Circles;

    Vector2 CircleIndicatorPosition;
    float CircleIndicatorRadius;
    

    int SelectedBrush = -1;
    int SelectedColor = -1;
    Vector3 BrushColor;

    public override void _Ready()
	{

        Lines = new Array<Line>();
        Circles = new Array<Circle>();



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
        //this is for color test, probably hone in on one color later and remove this
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
        Lines.Clear();
        Circles.Clear();

    }

    public override void _Draw()
    {
        DrawSetTransformMatrix(GetGlobalTransform().AffineInverse());
        
		if(InDrawMode && CanDraw && Input.IsActionPressed("mouse_left_click"))
		{
            if(SelectedBrush == 0)
            {
                //line
                Lines.Add(new Line());
                Lines.Last<Line>().StartPosition = LastMousePos;
                Lines.Last<Line>().EndPosition = CurrentMousePosition;
                Lines.Last<Line>().Color = BrushColor;
                Vector3 WorldSpace = GetParent().GetNode<Camera3D>("Camera3D").GlobalPosition;
                Lines.Last<Line>().WorldSpaceDrawnIn = WorldSpace;
            }

            if (SelectedBrush == 1)
            {
                //circle
                CircleIndicatorRadius = (float)Math.Sqrt(Math.Pow(CurrentMousePosition.X - CircleIndicatorPosition.X, 2) + Math.Pow(CurrentMousePosition.Y - CircleIndicatorPosition.Y, 2));
            }

            if (SelectedBrush == 2)
            {
                //find the nearest line or circle and delete it
                //needs more research
                Array<int> LinesToDelete = new Array<int>();
                Array<int> CirclesToDelete = new Array<int>();
                Camera3D Cam = GetParent().GetNode<Camera3D>("Camera3D");
                Vector3 WorldSpace = Cam.GlobalPosition;
                Vector2 UnprojectedPositionChanger = Cam.UnprojectPosition(WorldSpace);
                CircleIndicatorPosition = CurrentMousePosition;
                CircleIndicatorRadius = 20;
                for (int i = 0; i < Lines.Count; i++) 
                {
                    //finding where we are erasing in refrence to the cameras position
                    Vector2 LineUnprojectedPositionChanger = Cam.UnprojectPosition(WorldSpace) - Cam.UnprojectPosition(Lines[i].WorldSpaceDrawnIn);
                    //we need to manually change it wo half viewport size as it was off by that much
                    //this also gets how far away the eraser is from the line
                    Vector2 V = (Lines[i].StartPosition - LineUnprojectedPositionChanger) - (CurrentMousePosition - UnprojectedPositionChanger + (GetViewport().GetVisibleRect().Size * 0.5f));
                    
                    if (Math.Abs(V.X) < 20 && Math.Abs(V.Y) < 20)
                    {
                        for (int j = i; j < Lines.Count - 1; j++)
                        {
                            //move everything one forward and pop the end
                            Lines[j].StartPosition = Lines[j + 1].StartPosition;
                            Lines[j].EndPosition = Lines[j + 1].EndPosition;
                            Lines[j].Color = Lines[j + 1].Color;
                            Lines[j].WorldSpaceDrawnIn = Lines[j + 1].WorldSpaceDrawnIn;
                        }
                        Lines.RemoveAt(Lines.Count - 1);
                    }
                }

                //same as line
                for (int i = 0; i < Circles.Count; i++)
                {
                    Vector2 LineUnprojectedPositionChanger = Cam.UnprojectPosition(WorldSpace) - Cam.UnprojectPosition(Circles[i].WorldSpaceDrawnIn);
                    Vector2 V = (Circles[i].Center - LineUnprojectedPositionChanger) - (CurrentMousePosition - UnprojectedPositionChanger + (GetViewport().GetVisibleRect().Size * 0.5f));
                    //except we add the circles radius
                    if (Math.Abs(V.X) < 20 + Circles[i].Radius && Math.Abs(V.Y) < 20 + Circles[i].Radius)
                    {
                        for (int j = i; j < Circles.Count - 1; j++)
                        {
                            //
                            Circles[j].Center = Circles[j + 1].Center;
                            Circles[j].Radius = Circles[j + 1].Radius;
                            Circles[j].Color = Circles[j + 1].Color;
                            Circles[j].WorldSpaceDrawnIn = Circles[j + 1].WorldSpaceDrawnIn;
                        }
                        Circles.RemoveAt(Circles.Count - 1);
                    }
                }


            }
        }

        //permanent circle
        if (InDrawMode && Input.IsActionJustPressed("mouse_left_click") && CanDraw)
        {
            if (SelectedBrush == 1)
            {
                //circle
                Circles.Add(new Circle());
                Circles.Last<Circle>().Center = CurrentMousePosition;
                CircleIndicatorPosition = CurrentMousePosition;
            }
        }
        if (InDrawMode && Input.IsActionJustReleased("mouse_left_click") && CanDraw)
        {
            if (SelectedBrush == 1 && CircleIndicatorPosition != Vector2.Zero)
            {
                //circle

                if (Circles.Count != 0)
                {

                    float distance = (float)Math.Sqrt(Math.Pow(CurrentMousePosition.X - Circles.Last<Circle>().Center.X, 2) + Math.Pow(CurrentMousePosition.Y - Circles.Last<Circle>().Center.Y, 2));

                    Circles.Last<Circle>().Radius = distance;
                    Circles.Last<Circle>().Color = BrushColor;
                    CircleIndicatorPosition = Vector2.Zero;
                    CircleIndicatorRadius = -1;
                    Vector3 WorldSpace = GetParent().GetNode<Camera3D>("Camera3D").GlobalPosition;
                    Circles.Last<Circle>().WorldSpaceDrawnIn = WorldSpace;
                }
                
            }
            if(SelectedBrush == 2)
            {
                CircleIndicatorPosition = Vector2.Zero;
                CircleIndicatorRadius = -1;
            }
        }



        for (int i = 0; i < Lines.Count; i++)
		{
			if(i % 2 != 0)
			{
                
				
            }
            Camera3D Cam = GetParent().GetNode<Camera3D>("Camera3D");
            Vector3 WorldSpace = Cam.GlobalPosition;
            Vector2 UnprojectedPositionChanger = Cam.UnprojectPosition(WorldSpace) - Cam.UnprojectPosition(Lines[i].WorldSpaceDrawnIn);
            DrawDashedLine(Lines[i].StartPosition - UnprojectedPositionChanger, Lines[i].EndPosition - UnprojectedPositionChanger, new Color(Lines[i].Color.X / 255.0f, Lines[i].Color.Y / 255.0f, Lines[i].Color.Z / 255.0f, 1), 1.0f, 2, true, true);

        }
       
        for (int i = 0; i < Circles.Count; i++)
        {
            if(Circles.Last<Circle>().Radius == 0 && i == Circles.Count - 1)
            {
                break;
            }
            Camera3D Cam = GetParent().GetNode<Camera3D>("Camera3D");
            Vector3 WorldSpace = Cam.GlobalPosition;
            Vector2 UnprojectedPositionChanger = Cam.UnprojectPosition(WorldSpace) - Cam.UnprojectPosition(Circles[i].WorldSpaceDrawnIn);
            DrawCircle(Circles[i].Center - UnprojectedPositionChanger, Circles[i].Radius, new Color(Circles[i].Color.X / 255.0f, Circles[i].Color.Y / 255.0f, Circles[i].Color.Z / 255.0f, 1), false, 1, true);
            
        }
        

        if (CircleIndicatorPosition != Vector2.Zero && CircleIndicatorRadius != -1)
        {
            DrawCircle(CircleIndicatorPosition, CircleIndicatorRadius, new Color(BrushColor.X / 255.0f, BrushColor.Y / 255.0f, BrushColor.Z / 255.0f, 0.5f), false, 1, true);
            
        }
       
        
    }

    public void HoveringGUI()
    {
        CanDraw = false;
    }
    public void NotHoveringGUI()
    {
        CanDraw = true;
    }
}
