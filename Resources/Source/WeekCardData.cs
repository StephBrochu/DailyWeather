using Godot;

[GlobalClass]

public partial class WeekCardData: Resource
{
    
    [Export] public float Orientation { get; set; }
    [Export] public int Row { get; set; }
    [Export] public int Column { get; set; }
    [Export] public int Anchor { get; set; }
    
}