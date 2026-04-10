using Godot;
[GlobalClass]

public partial class DayConditionCard: Resource
{
    [Export] public Godot.Collections.Array<Texture2D> Image { get; set; } = new();
    [Export] public Godot.Collections.Array<int> ArrowPosition { get; set; } = new();
    [Export] public Godot.Collections.Array<ConditionGrid> Season { get; set; } = new();
    // missing scoring rule
}