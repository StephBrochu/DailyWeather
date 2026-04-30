using Godot;

public partial class CardCell : PanelContainer
{
    public int Icon { get; set; }
    public int Bg { get; set; }
    public bool Available;
    public int Row = -1;
    public int Col = -1;
    private int _rowOffset;
    private int _colOffset;
    private TextureRect _highlight;
    private TextureRect _unavailable;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _highlight = GetNode<TextureRect>("Highlight");
        _unavailable = GetNode<TextureRect>("Unavailable");
        MouseEntered += OnMouseOver;
        MouseExited += OnMouseExit;
        Available = true;
    }
    
    private void OnMouseOver()
    {
        if (!Available) return;
        _highlight.Visible = true;
        SignalManager.EmitOnMouseEntered(Icon, Bg);
    }

    private void OnMouseExit()
    {
        if (!Available) return;
        _highlight.Visible = false;
        SignalManager.EmitOnMouseExit();
    }

    public void HighlightCell()
    {
        if (Available) _highlight.Visible = true;
    }

    public void UnHighlightCell()
    {
        if (Available) _highlight.Visible = false;
    }
    
    public void CellUnavailable()
    {
        Available = false;
        _unavailable.Visible = true;
        SignalManager.EmitOnMouseExit();
    }
}
