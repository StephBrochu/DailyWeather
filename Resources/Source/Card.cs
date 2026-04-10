using Godot;

[GlobalClass]
public partial class Card : Resource
{
    [Export] public CardData Front; 
    [Export] public CardData Back;
    [Export] public bool DoubleSided; // is there game data on both sides of the card?
    public float Rotation { set; get; }
    public int[] Location { set; get; }
}