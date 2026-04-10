using Godot;

public partial class DebugCell : TextureRect
{
	[Export] public Label _data;

	private string _location;
	
	private static DebugCell Instance { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	public void ShowCell()
	{
		Visible = true;
	}

	public void HideCell()
	{
		Visible = false;
	}

	public void SetUpCardData(string cell, string data)
	{
		_data.Text = $"C: {cell} \nD: {data}";
		_location = cell;
	}

	public void UpdateData(string data)
	{
		_data.Text = $"C: " + _location+ "\nD: " + data;
		GD.Print("C: ", _location, "D: ", data);
	}
}
