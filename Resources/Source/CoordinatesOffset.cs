using Godot;
[GlobalClass]

public partial class CoordinatesOffset: Resource
{
    [Export] public int rowOffset { get; set; }
    [Export] public int colOffset { get; set; }
}