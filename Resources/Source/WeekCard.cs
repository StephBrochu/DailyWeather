using Godot;
[GlobalClass]

public partial class WeekCard: Resource
{
    [Export] public Texture2D Image { get; set; }
    [Export] public WeekCardData Month { get; set; }
    [Export] public WeekCardData Day { get; set; }
    
    // icon data definition
    // each icon has a two aspects: weather type (sun - 1, snowflake - 2, rain drop - 3, cloud - 4)
    [Export] public int Icon1 { get; set; }
    [Export] public int Icon2 { get; set; }
    
    // and a background (solid - 1, stripes - 2, half - 3)
    [Export] public int Background1 { get; set; }
    [Export] public int Background2 { get; set; }
}