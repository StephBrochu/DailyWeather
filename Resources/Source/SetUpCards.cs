using Godot;

[GlobalClass]
public partial class SetUpCards : Resource
{
    // Called when the node enters the scene tree for the first time.
    [Export] public Godot.Collections.Array<SetUpCard> Card { get; set; } = new();
}