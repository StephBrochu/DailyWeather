using Godot;

public partial class GameGrid : GridContainer
{
	[Export] private PackedScene _debugCell;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void UpdateDebugCell(string name, string data)
	{
		DebugCell cellToUpdate = GetNode<DebugCell>(name);
		cellToUpdate.UpdateData(data);
	}
	
}
