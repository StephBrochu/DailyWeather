using Godot;
[GlobalClass]

public partial class CardIcon: Resource
{
    // icon data definition
    // each icon has a two aspects: weather type (sun - 1, snowflake - 2, rain drop - 3, cloud - 4)
    // and a background (solid - 1, stripes - 2, half - 3)
    [Export] public int Icon { get; set; }
    [Export] public int Background { get; set; }
}