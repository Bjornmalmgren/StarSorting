using Godot;
using System;


[Tool]
[GlobalClass]
public partial class BookData : Resource
{
    [Export] public string Title { get; set; } = "Untitled Book";

   
    // Drag a unique 2D Control scene (with images, rich text, layouts) here
    [Export] 
    public Texture2D FrontCoverTexture { get; set; }    // 2D cover shown before opening
    [Export]
    public PackedScene PagesContentScene { get; set; }
    [Export] 
    public bool RotateCover90Degrees { get; set; } = false;
    [Export]
    public bool RotateClockwise { get; set; } = false;

    [ExportCategory("If Book Is Scene")]
    /// <summary>
    /// Visual model if the book is imported as a scene
    /// </summary>
    [Export] public PackedScene VisualModelScene { get; set; }
    [ExportCategory("If Book Is Mesh")]
    /// <summary>
    /// BookMesh if its just a mesh 
    /// </summary>
    [Export] public Mesh BookMesh { get; set; }
}