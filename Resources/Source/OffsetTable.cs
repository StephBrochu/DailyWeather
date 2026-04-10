using Godot;
[GlobalClass]

public partial class OffsetTable: Resource
{
    [Export] public Godot.Collections.Array<CoordinatesOffset> Cell { get; set; } = new();
}