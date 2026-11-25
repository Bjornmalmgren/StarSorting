using Godot;
using System;
using System.Collections.Generic;

public partial class BookReader : CanvasLayer
{
    [Export] 
    private Control leftPageContainer;   // Left MarginContainer
    
    [Export] 
    private Control rightPageContainer;  // Right MarginContainer
    [Export]
    private TextureRect leftPageRect;   // left TextureRect
    [Export] 
    private TextureRect rightPageRect;   // Right TextureRect


    [Export] 
    private Button prevButton;
    [Export] 
    private Button nextButton;



    private List<Control> pages = new();
    private int currentState = 0; // 0 = Closed cover, 1+ = Open spreads
    private bool isFlipping = false;
    public void Initialize(BookData data)
    {
        if (data == null) 
        {
            return;
        }
        if (data.RotateCover90Degrees)
        {
            rightPageRect.Texture = RotateTexture90(data.FrontCoverTexture,data.RotateClockwise);
        }
        else
        {
            rightPageRect.Texture = data.FrontCoverTexture;
        }
        if (rightPageContainer != null)
        {
            rightPageContainer.PivotOffset = new Vector2(0, rightPageContainer.Size.Y / 2f);
        }
        prevButton.Visible = false;
        leftPageRect.Visible = false;
        currentState = 0;
      
    }

    public void OnButtonPreviousPressed()
    {
        GD.Print("previous button pressed");
    }
    public void OnButtonNextPressed()
    {
        GD.Print("Next button pressed");
    }
    private Texture2D RotateTexture90(Texture2D source, bool clockwise = true)
    {
        if (source == null) return null;

        Image img;

        // If it's an AtlasTexture, decompress the full atlas first before cropping
        if (source is AtlasTexture atlas && atlas.Atlas != null)
        {
            Image fullImg = atlas.Atlas.GetImage();
            if (fullImg.IsCompressed()) fullImg.Decompress();
            img = fullImg.GetRegion((Rect2I)atlas.Region);
        }
        else
        {
            img = source.GetImage();
            if (img.IsCompressed())
            {
                img.Decompress();
            }
        }

        img.Rotate90(clockwise ? ClockDirection.Clockwise : ClockDirection.Counterclockwise);
        return ImageTexture.CreateFromImage(img);
    }
}
