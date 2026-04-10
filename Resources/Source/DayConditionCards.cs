using Godot;
[GlobalClass]

public partial class DayConditionCards: Resource
{
    [Export] public Godot.Collections.Array<DayConditionCard> Card { get; set; } = new();
}