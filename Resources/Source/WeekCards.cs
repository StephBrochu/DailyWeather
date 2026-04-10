using Godot;
[GlobalClass]

public partial class WeekCards: Resource
{
    [Export] public Godot.Collections.Array<WeekCard> Card { get; set; } = new();
}