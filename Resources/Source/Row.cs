using Godot;
[GlobalClass]

public partial class Row: Resource
{
    [Export] public Godot.Collections.Array<Cell> GridRow { get; set; }= new ();
}