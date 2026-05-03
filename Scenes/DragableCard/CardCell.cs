using Godot;

public partial class CardCell : PanelContainer
{
    public int Icon { get; set; }
    public int Bg { get; set; }
    public bool Available { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    private int _rowOffset;
    private int _colOffset;
    private TextureRect _highlight;
    private TextureRect _unavailable;
    private Control _pivot;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _highlight = GetNode<TextureRect>("Highlight");
        _unavailable = GetNode<TextureRect>("Unavailable");
        _pivot = GetNode<Control>("Pivot");
        
        MouseEntered += OnMouseOver;
        MouseExited += OnMouseExit;
        Available = true;
        Row = -1;
        Col = -1;
    }
    
    private void OnMouseOver()
    {
        if (!Available) return;
        _highlight.Visible = true;
        SignalManager.EmitOnMouseEntered(Icon, Bg, _pivot);
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
