using Godot;

public partial class CardCell : PanelContainer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MouseEntered += HighlightCell;
        MouseExited += UnHighlightCell;
    }

    public void HighlightCell()
    {
        TextureRect highlight = GetNode<TextureRect>("Highlight");
        highlight.Visible = true;
    }

    public void UnHighlightCell()
    {
        TextureRect highlight = GetNode<TextureRect>("Highlight");
        highlight.Visible = false;
    }
}
