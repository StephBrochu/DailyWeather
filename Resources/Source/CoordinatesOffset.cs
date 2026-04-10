using Godot;
[GlobalClass]

public partial class CoordinatesOffset: Resource
{
    [Export] public int xOffset { get; set; }
    [Export] public int yOffset { get; set; }
}