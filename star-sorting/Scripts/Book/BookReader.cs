using Godot;
using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


    private Texture2D coverPageTexture;
    private List<Control> pages = new();
    private int currentState = 0; // 0 = Closed cover, 1+ = Open spreads
   
    public void Initialize(BookData data)
    {
        if (data == null) 
        {
            return;
        }
        if (data.RotateCover90Degrees)
        {
            coverPageTexture = RotateTexture90(data.FrontCoverTexture, data.RotateClockwise);
        }
        else
        {
            coverPageTexture = data.FrontCoverTexture;
        }
        if (rightPageContainer != null)
        {
            rightPageContainer.PivotOffset = new Vector2(0, rightPageContainer.Size.Y / 2f);
        }
        pages.Clear();
        if (data.PagesContentScene != null)
        {
            Control contentRoot = data.PagesContentScene.Instantiate<Control>();

            foreach (Node child in contentRoot.GetChildren())
            {
                if (child is Control pageNode)
                    pages.Add(pageNode);
            }

            // Even indices (0, 2, 4...) -> Left container | Odd indices (1, 3, 5...) -> Right container
            for (int i = 0; i < pages.Count; i++)
            {
                Control targetContainer = (i % 2 == 0) ? leftPageContainer : rightPageContainer;
                pages[i].Owner = null;
                pages[i].Reparent(targetContainer);
                pages[i].Visible = false;
            }

            contentRoot.QueueFree();
        }

        
        currentState = 0;
        UpdateBookDisplay();
    }

    public void OnButtonPreviousPressed()
    {
        if(currentState<=0)
        {
            return;
        }
        currentState--;
        GD.Print("previous button pressed");
        UpdateBookDisplay();
    }
    public void OnButtonNextPressed()
    {
        int totalPages = Mathf.CeilToInt(pages.Count / 2f);
        if (currentState >= totalPages)
        {
            return;
        }
        currentState++;
        GD.Print("Next button pressed");
        UpdateBookDisplay();
    }
    private void UpdateBookDisplay()
    {
        bool isClosed = (currentState == 0);
        if(isClosed)
        {
            prevButton.Visible = false;
            leftPageRect.Visible = false;
            rightPageRect.Visible = true;
            rightPageRect.Texture = coverPageTexture;
        }
        else
        {

            if (rightPageRect != null)
            {
                rightPageRect.Visible = false;
            }
        }
        int leftIndex = (currentState - 1) * 2;
        int rightIndex = leftIndex + 1;
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].Visible = !isClosed && (i == leftIndex || i == rightIndex);
        }
        int totalSpreads = Mathf.CeilToInt(pages.Count / 2f);
        if (prevButton != null)
        {
            prevButton.Visible = currentState > 0;
        }
        if (nextButton != null)
        {
            nextButton.Visible = currentState < totalSpreads;
        }
    }

    //Rotate texture helper
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
