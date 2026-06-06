using Godot;
[GlobalClass]

public partial class DayCardData: Resource
{
    public enum ScoringConditions
    {
    SunInTopRow,
    RainInBottomRow,
    CloudInLargestCloudGroup,
    SnowInLargestSnowGroup,
    SunGroups,
    RainGroups,
    CellsInLongestColumn,
    CellsInLongestRow,
    StripGroupOfSize1,
    FullInLongestDiagonal,
    SplitOnTheBorder,
    IconGroupsOfSize3
}
    [Export] public Texture2D Image;
    [Export] public int ArrowPosition;
    [Export] public Godot.Collections.Array<ConditionGrid> Season { get; set; } = new();
    
    [Export] public ScoringConditions CardScoringCondition { get; set; }
    // missing scoring rule
}