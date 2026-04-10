using Godot;

[GlobalClass]

public partial class SetUpCardData : Resource
{
    [Export] public Texture2D Image { get; set; }
    // special days have the following code: birthday - 32; valentine day - 33; halloween - 34; thanksgiving - 35;
    // birthday - 36
    [Export] public Godot.Collections.Array<CardIcon> Icon { get; set; } = new();
    [Export] public Godot.Collections.Array<int> Date { get; set; } = new();
}