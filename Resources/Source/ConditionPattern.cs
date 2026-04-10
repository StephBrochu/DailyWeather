using Godot;
[GlobalClass]

public partial class ConditionPattern: Resource
{
    [Export] public Godot.Collections.Array<int> Cell { get; set; } = new();
}