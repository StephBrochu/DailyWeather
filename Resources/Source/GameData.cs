using System.Collections.Generic;
using Godot;
[GlobalClass]

public partial class GameData: Resource
{
    [Export] public Godot.Collections.Array<Row> Grid { get; set; } = [];
    
    public List<CardData> Deck = [];
    public int WeekCard { get; set; }
    public int DayCard { get; set; }
    public int Season { get; set; }
    public int CellDay { get; set; }
    public int CellMonth { get; set; }

    public const string MonthCardName = "MonthCard";
    public const string DayCardName = "DayCard";
}