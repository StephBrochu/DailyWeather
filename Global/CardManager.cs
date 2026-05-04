using Godot;

namespace DailyWeather.Global;

public partial class CardManager : Node
{
    public static CardManager Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }

    public void Shuffle()
    {
        
    }

    public void Deal()
    {
        
    }
}