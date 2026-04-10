using Godot;

[GlobalClass]
public partial class SetUpCard : Resource
{
    [Export] public SetUpCardData Front; 
    public float Rotation { set; get; }
    public int[] Location { set; get; }
}