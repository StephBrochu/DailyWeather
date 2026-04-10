using Godot;

[GlobalClass]
public partial class DeckOfCards : Resource
{
	// Called when the node enters the scene tree for the first time.
	[Export] public Godot.Collections.Array<Card> Cards { get; set; } = new();
}
