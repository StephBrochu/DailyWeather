using Godot;
[GlobalClass]

public partial class ConditionGrid: Resource
{
    [Export] public Godot.Collections.Array<ConditionPattern> Grid { get; set; } = [];
}