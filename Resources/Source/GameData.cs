using System.Collections.Generic;
using Godot;
[GlobalClass]

public partial class GameData: Resource
{
    [Export] public Godot.Collections.Array<Row> Grid { get; set; } = [];
    
    public readonly List<CardData> Deck = [];
    public int WeekCard { get; set; } // which card is used for the week -> set up card, not play area card
    public int DayCard { get; set; } // which card is used for the day -> set up card, not play area card
    public int Season { get; set; } // which season (and side)
    public int CellDay { get; set; }
    public int CellMonth { get; set; }
    
    // the data for the scoring pattern requirement
    public string PatternType { get; set; }
    public int PatternNumber { get; set; }
    public readonly List<ConditionPattern> PatternUsed = [];
    public bool DayComplete = false;
    public bool MonthComplete = false;
    public bool PatternComplete = false;
    public int FinalScore;

    // a few constants so that they can be used everywhere
    public const string MonthCardName = "MonthCard";
    public const string DayCardName = "DayCard"; // day card that is played to the table
    public List<string> PatternKey = ["Icon 1", "Icon 2", "Background 1", "Background 2"];
    
}