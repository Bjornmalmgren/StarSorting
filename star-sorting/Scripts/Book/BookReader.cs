using Godot;
using System;
using System.Collections.Generic;

public partial class BookReader : CanvasLayer
{
    [Export] 
    private Control leftPageContainer;   // Left MarginContainer
    [Export]
    private TextureRect leftPageRect;    // Left TextureRect
    [Export] 
    private Control rightPageContainer;  // Right MarginContainer
    [Export] 
    private TextureRect rightPageRect;   // Right TextureRect

    [Export] 
    private Button prevButton;
    [Export] 
    private Button nextButton;

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
