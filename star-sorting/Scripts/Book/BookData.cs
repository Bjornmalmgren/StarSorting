using Godot;
using System;



[GlobalClass]
public partial class BookData : Resource
{
    [Export] public string Title { get; set; } = "Untitled Book";
    [Export] public PackedScene VisualModelScene { get; set; }

    // Drag a unique 2D Control scene (with images, rich text, layouts) here
    [Export] public Texture2D FrontCoverTexture { get; set; }    // 2D cover shown before opening
    [Export] public PackedScene PagesContentScene { get; set; }
}