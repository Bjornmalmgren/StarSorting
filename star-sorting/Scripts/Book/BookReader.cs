using Godot;
using System;
using System.Collections.Generic;

public partial class BookReader : CanvasLayer
{
    [Export] private TextureRect coverPage;   
    [Export] private Control openPage;  
    [Export] private Button prevButton;
    [Export] private Button nextButton;

    private List<Control> pages = new();
    private int currentState = 0; //0 = closed book, 1 or more = open page
    public void Initialize(BookData data)
    {
        if (data == null) 
        {
            return;
        }
    }
    public void OnButtonPreviousPressed()
    {
        GD.Print("previous button pressed");
    }
    public void OnButtonNextPressed()
    {
        GD.Print("Next button pressed");
    }
}
