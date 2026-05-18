using Godot;
[GlobalClass]

public partial class DayCardData: Resource
{
    [Export] public Texture2D Image;
    [Export] public int ArrowPosition;
    [Export] public Godot.Collections.Array<ConditionGrid> Season { get; set; } = new();

    [Export] public string ScoringCondition;
    // missing scoring rule
}