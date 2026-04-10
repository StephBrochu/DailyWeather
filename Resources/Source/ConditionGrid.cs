using Godot;
[GlobalClass]

public partial class ConditionGrid: Resource
{
    [Export] private Godot.Collections.Array<ConditionPattern> Grid { get; set; } = new();
}