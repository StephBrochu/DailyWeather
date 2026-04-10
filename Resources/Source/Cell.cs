using Godot;
[GlobalClass]

public partial class Cell: Resource
{
    public int Icon { get; set; }
    public int Background { get; set; }
    public string CardName { get; set; }
    public int CellIndex { get; set; }
    public bool Locked { get; set; }
}