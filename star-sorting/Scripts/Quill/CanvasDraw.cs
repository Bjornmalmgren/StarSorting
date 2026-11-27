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
    public float Width;
   
}

public partial class Circle : Resource
{
    public Vector2 Center;
    public float Radius;
    public Vector3 Color;
    public Vector3 WorldSpaceDrawnIn;
    public float Width;
}

public partial class CanvasDraw : Node2D
{

	bool InDrawMode = false;

    [ExportCategory("Refrences")]
    [Export]
    Camera3D Camera;
    [Export]
    Control Menu;
    [Export]
    HSlider Slider;
    [Export]
    OptionButton Brush;
    [Export]
    OptionButton Color;
    [ExportCategory("Variables")]
    Vector2 LastMousePos;
    Vector2 CurrentMousePosition;
    
    [Export]
    bool CanDraw = true;
    Array<Line> Lines;
    Array<Circle> Circles;
    [Export]
    Array<Line> LinesBetweenStars = new Array<Line>();

    Vector2 CircleIndicatorPosition;
    float CircleIndicatorRadius;
    

    int SelectedBrush = -1;
    int SelectedColor = -1;
    Vector3 BrushColor;


    public override void _Ready()
	{

        Lines = new Array<Line>();
        Circles = new Array<Circle>();
        GameEvents.Instance.StarmapEnded += ExitQuill;

        CircleIndicatorPosition = Vector2.Zero;
        CircleIndicatorRadius = -1;
        GameEvents.Instance.ConstellationDone += AddContellationsToDraw;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("change_to_drawing_mode"))
        {
            InDrawMode = !InDrawMode;
        }
        if(Brush.Selected != SelectedBrush)
        {
            SelectedBrush = Brush.Selected;
        }

        if (Color.Selected != SelectedColor)
        {
            SelectedColor = Color.Selected;
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

    public void AddContellationsToDraw(string[] OrderToDrawBetweenStars, Vector2[] Positions, Array<Node3D> Stars)
    {
        
        

        for (int i = 0; i < OrderToDrawBetweenStars.Length; i++)
        {
            GD.Print("Draw star path");
            string[] parts = OrderToDrawBetweenStars[i].Split(',');
            LinesBetweenStars.Add(new Line());
            LinesBetweenStars.Last<Line>().StartPosition = Camera.UnprojectPosition(new Vector3(Stars[Convert.ToInt32(parts[0])].GlobalPosition.X, Stars[Convert.ToInt32(parts[0])].GlobalPosition.Y, Stars[Convert.ToInt32(parts[0])].GlobalPosition.Z));
            LinesBetweenStars.Last<Line>().EndPosition = Camera.UnprojectPosition(new Vector3(Stars[Convert.ToInt32(parts[1])].GlobalPosition.X, Stars[Convert.ToInt32(parts[1])].GlobalPosition.Y, Stars[Convert.ToInt32(parts[1])].GlobalPosition.Z));
            LinesBetweenStars.Last<Line>().Color = new Vector3(255, 255, 255);
            LinesBetweenStars.Last<Line>().Width = 2;
            LinesBetweenStars.Last<Line>().WorldSpaceDrawnIn = Camera.GlobalPosition;
        }
    }


    public void ExitQuill()
    {
        InDrawMode = false;
        GD.Print(InDrawMode);
    }

    public override void _Draw()
    {
        if (!(GetParent() as StarMapPrototype1).OperateStarMap)
        {
            Menu.Visible = false;
            return;
        }
        if ((GetParent() as StarMapPrototype1).OperateStarMap)
        {
           Menu.Visible = true;
        }
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
                Vector3 WorldSpace = Camera.GlobalPosition;
                Lines.Last<Line>().WorldSpaceDrawnIn = WorldSpace;
                Lines.Last<Line>().Width = (float)Slider.Value;
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
                
                Vector3 WorldSpace = Camera.GlobalPosition;
                Vector2 UnprojectedPositionChanger = Camera.UnprojectPosition(WorldSpace);
                CircleIndicatorPosition = CurrentMousePosition;
                CircleIndicatorRadius = 20;
                for (int i = 0; i < Lines.Count; i++) 
                {
                    //finding where we are erasing in refrence to the cameras position
                    Vector2 LineUnprojectedPositionChanger = Camera.UnprojectPosition(WorldSpace) - Camera.UnprojectPosition(Lines[i].WorldSpaceDrawnIn);
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
                            Lines[j].Width = Lines[j + 1].Width;
                        }
                        Lines.RemoveAt(Lines.Count - 1);
                    }
                }

                //same as line
                for (int i = 0; i < Circles.Count; i++)
                {
                    Vector2 LineUnprojectedPositionChanger = Camera.UnprojectPosition(WorldSpace) - Camera.UnprojectPosition(Circles[i].WorldSpaceDrawnIn);
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
                            Circles[j].Width = Circles[j + 1].Width;
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
                    Vector3 WorldSpace = Camera.GlobalPosition;
                    Circles.Last<Circle>().WorldSpaceDrawnIn = WorldSpace;
                    Circles.Last<Circle>().Width = (float)Slider.Value;
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
            
            Vector3 WorldSpace = Camera.GlobalPosition;
            Vector2 UnprojectedPositionChanger = Camera.UnprojectPosition(WorldSpace) - Camera.UnprojectPosition(Lines[i].WorldSpaceDrawnIn);
            DrawDashedLine(Lines[i].StartPosition - UnprojectedPositionChanger, Lines[i].EndPosition - UnprojectedPositionChanger, new Color(Lines[i].Color.X / 255.0f, Lines[i].Color.Y / 255.0f, Lines[i].Color.Z / 255.0f, 1), Lines[i].Width, 2, true, true);

        }
       
        for (int i = 0; i < Circles.Count; i++)
        {
            if(Circles.Last<Circle>().Radius == 0 && i == Circles.Count - 1)
            {
                break;
            }
            
            Vector3 WorldSpace = Camera.GlobalPosition;
            Vector2 UnprojectedPositionChanger = Camera.UnprojectPosition(WorldSpace) - Camera.UnprojectPosition(Circles[i].WorldSpaceDrawnIn);
            DrawCircle(Circles[i].Center - UnprojectedPositionChanger, Circles[i].Radius, new Color(Circles[i].Color.X / 255.0f, Circles[i].Color.Y / 255.0f, Circles[i].Color.Z / 255.0f, 1), false, Circles[i].Width, true);
            
        }

        for (int i = 0; i < LinesBetweenStars.Count; i++)
        {

           
            Vector3 WorldSpace = Camera.GlobalPosition;
            Vector2 UnprojectedPositionChanger = Camera.UnprojectPosition(WorldSpace) - Camera.UnprojectPosition(LinesBetweenStars[i].WorldSpaceDrawnIn);
            DrawDashedLine(LinesBetweenStars[i].StartPosition - UnprojectedPositionChanger, LinesBetweenStars[i].EndPosition - UnprojectedPositionChanger, new Color(LinesBetweenStars[i].Color.X / 255.0f, LinesBetweenStars[i].Color.Y / 255.0f, LinesBetweenStars[i].Color.Z / 255.0f, 1), LinesBetweenStars[i].Width, 2, true, true);

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
