using Godot;
[GlobalClass]

public partial class OffsetTables: Resource
{
    [Export] public Godot.Collections.Array<OffsetTable> Table = new();
}