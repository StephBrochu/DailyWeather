using Godot;

[GlobalClass]
public partial class CardData : Resource
{
    [Export] public Texture2D Image { get; set; }
    [Export] public Godot.Collections.Array<CardIcon> Icon { get; set; } = new();

}