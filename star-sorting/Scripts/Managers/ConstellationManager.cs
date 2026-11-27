using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class ConstellationManager : Node3D
{
	[Export]
	Array<Constellation> Constellations = new Array<Constellation>();
	[Export]
	Array<Array<Vector2>> ConstellationPositions = new Array<Array<Vector2>>();

    

    [Export]
    Array<Array<string>> ConstellationStars = new Array<Array<string>>();
    [Export]
    Array<Array<string>> ConstellationDrawOrder = new Array<Array<string>>();

    [Export]
    Array<Array<Vector2>> SafetyConstellationPositions = new Array<Array<Vector2>>();



    [Export]
    Array<Array<string>> SafetyConstellationStars = new Array<Array<string>>();
    [Export]
    Array<Array<string>> SafetyConstellationDrawOrder = new Array<Array<string>>();

    [ExportToolButton("Refresh Constellations")]
    public Callable RefreshConstellations => Callable.From(GetConstellations);

    //choses the position of the stars that are to be created
    [Export]
    Node3D PositionChoser;
    

    public Callable GetPositionAndDrawArray
    {
        get => Callable.From(ChangeExistingConstellations);
        set
        { 

        }
    }

    public Callable FixConstellations
    {
        get => Callable.From(FixExistingConstellations);
        set
        {

        }
    }

    public Callable CreateStar
    {
        get => Callable.From(AddNewStar);
        set
        {

        }
    }

    public Callable RevertToOriginal
    {
        get => Callable.From(RevertToOriginalArrays);
        set
        {

        }
    }

    [ExportCategory("ConstellationChoice")]
    private bool _ChangeExistingConstellation = false;

    [Export]
    public bool CreateNewConstellation = false;
    
    [Export]
    public bool ChangeExistingConstellation
    {
        get => _ChangeExistingConstellation;
        set
        {
            _ChangeExistingConstellation = value;
            NotifyPropertyListChanged();
        }
    }

    private int CurrentConstellation = 0;

    private Array<string> CurrentConstellationsOrder;
    private string StarName;
    private Vector2 NewStarPosition;
    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> Properties = [];
    
        if(_ChangeExistingConstellation == true)
        {
            Properties.Add(new Dictionary()
            {
                {"name", "GetPositionAndDrawArray" },
                {"type", (int)Variant.Type.Callable},
                {"hint", (int)PropertyHint.ToolButton},
                {"hint_string", "GetPositionAndDrawArray" },
                {"usage", (int)PropertyUsageFlags.Editor }
            });

            Properties.Add(new Dictionary()
            {
                {"name", "FixExistingConstellations" },
                {"type", (int)Variant.Type.Callable},
                {"hint", (int)PropertyHint.ToolButton},
                {"hint_string", "FixExistingConstellations" },
                {"usage", (int)PropertyUsageFlags.Editor }
            });

            Properties.Add(new Dictionary()
            {
                {"name", "CurrentConstellation" },
                {"type", (int)Variant.Type.Int}
            });

            Properties.Add(new Dictionary()
            {
                {"name", "StarName" },
                {"type", (int)Variant.Type.String}
            });

            Properties.Add(new Dictionary()
            {
                {"name", "NewStarPosition" },
                {"type", (int)Variant.Type.Vector2}
            });

            Properties.Add(new Dictionary()
            {
                {"name", "CurrentConstellationsOrder" },
                {"type", (int)Variant.Type.Array}
            });

            Properties.Add(new Dictionary()
            {
                {"name", "CreateStar" },
                {"type", (int)Variant.Type.Callable},
                {"hint", (int)PropertyHint.ToolButton},
                {"hint_string", "CreateStar" },
                {"usage", (int)PropertyUsageFlags.Editor }
            });

            Properties.Add(new Dictionary()
            {
                {"name", "RevertToOriginal" },
                {"type", (int)Variant.Type.Callable},
                {"hint", (int)PropertyHint.ToolButton},
                {"hint_string", "RevertToOriginal" },
                {"usage", (int)PropertyUsageFlags.Editor }
            });
        }
        
    
    
        return Properties;
    }


    public override Variant _Get(StringName property)
    {
        return base._Get(property);
    }

    public void ChangeExistingConstellations()
    {
        NewStarPosition = new Vector2(PositionChoser.GlobalPosition.X, PositionChoser.GlobalPosition.Y);
        CurrentConstellationsOrder = ConstellationDrawOrder[CurrentConstellation];
        string s = "";
        CurrentConstellationsOrder.Add(s);
    }

    public void AddNewStar()
    {
        var TempPositions = ConstellationPositions[CurrentConstellation];
        TempPositions.Add(NewStarPosition);
        var TempName = ConstellationStars[CurrentConstellation];
        TempName.Add(StarName);
        var TempOrder = ConstellationDrawOrder[CurrentConstellation];
       

        ConstellationPositions[CurrentConstellation] = TempPositions;
        ConstellationDrawOrder[CurrentConstellation] = CurrentConstellationsOrder;
        ConstellationStars[CurrentConstellation] = TempName;

        FixExistingConstellations();
    }

    public void RevertToOriginalArrays()
    {
        ConstellationPositions = SafetyConstellationPositions;
        ConstellationDrawOrder = SafetyConstellationDrawOrder;
        ConstellationStars = SafetyConstellationStars;
    }

    //fixes existing constellations
    public void FixExistingConstellations()
    {
        
        
        for (int i = 0; i < Constellations.Count; i++)
        {

            
            Constellations[i].CorrectPositions = new Vector2[ConstellationPositions[i].Count];
            Constellations[i].CorrectStars = new string[ConstellationPositions[i].Count];
            Constellations[i].OrderToDrawBetweenStars = new string[ConstellationPositions[i].Count];
            Constellations[i].StarsInConstellation = ConstellationPositions[i].Count;
            
            for (int j = 0; j < Constellations[i].StarsInConstellation; j++)
            {
                Constellations[i].CorrectPositions[j] = ConstellationPositions[i][j];
                Constellations[i].CorrectStars[j] = ConstellationStars[i][j];
                Constellations[i].OrderToDrawBetweenStars[j] = ConstellationDrawOrder[i][j];
            }
            
        }
    }

    public override void _Ready()
	{
		if (Engine.IsEditorHint())
		{

            GetConstellations();
        }
		else
		{
            //GameEvents.Instance.SendConstellations += GetConstellations;

        }

		
		
	}


	public override void _Process(double delta)
	{
       
       

    }

	void GetConstellations()
	{
        Constellations.Clear();
        var NodeArray = GetParent().FindChildren("*Constellation*");
        foreach (var node in NodeArray)
        {
            Constellations.Add(node as Constellation);
        }
       
    }
}
