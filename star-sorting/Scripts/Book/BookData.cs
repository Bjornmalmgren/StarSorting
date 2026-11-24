using Godot;
using System;



[GlobalClass]
public partial class BookData : Resource
{
    [Export] public string Title { get; set; } = "Untitled Book";
    [Export] public Material CoverMaterial { get; set; }

    // Drag a unique 2D Control scene (with images, rich text, layouts) here
    [Export] public PackedScene ReadingContentScene { get; set; }
}