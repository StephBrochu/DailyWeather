using Godot;
using Vector2 = Godot.Vector2;

public partial class DragableCard : PanelContainer
{
	public static DragableCard Instance { get; private set; }
	[Export] private TextureRect _cardImage;
	[Export] public Godot.Collections.Array<CardCell> Cell = new();
	public enum CardState {Dealt, Drag, Rotate, Released, Locked}
	public CardState Card;
	
	private Vector2 _eventStart = Vector2.Zero; // position of the cursor when the event starts
	private Vector2 _offset = Vector2.Zero; // offset between cursor and corner of card
	private float _currentRotation;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		Card = CardState.Dealt;
	}

	public override void _Process(double delta)
	{
		if (Card == CardState.Locked) return; // if the card is locked, then don't allow dragging/rotating
		switch (Card)
		{
			case CardState.Drag:
				Position = GetGlobalMousePosition() - _offset;
				break;
			
			case CardState.Rotate:
				Card = CardState.Dealt;
				break;
			
			case CardState.Released:
				CheckForSnap();
				Card = CardState.Dealt;
				break;
		}
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (Card == CardState.Locked) return; // if the card is locked, then don't allow dragging/rotating
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButtonEvent)
		{
			if (mouseButtonEvent.DoubleClick)
			{
				_eventStart = GetGlobalMousePosition();
				Card= CardState.Rotate;
				RotateCard();
			} else if (mouseButtonEvent.Pressed && Card== CardState.Dealt)
			{
				_offset = GetGlobalMousePosition() - Position;
				Card= CardState.Drag;
			}
			else
			{
				if (Card== CardState.Drag) Card= CardState.Released;
			}
		}
	}

	private void RotateCard()
	{
		var pivotPoints = GetTree().GetNodesInGroup("Pivot");
		
		// find the closest pivot point
		var closestPivot = Vector2.Zero;
		var bestDistance = 99999f;
		var nameOfPivot = "None";
		foreach(var node in pivotPoints)
		{
			Control controlNode = GetNode<Control>(node.GetPath());
			var distance = (_eventStart - controlNode.GlobalPosition).Length();
			if (distance < bestDistance)
			{
				closestPivot = controlNode.Position;
				bestDistance = distance;
			//	nameOfPivot = controlNode.Name;
			}
		}
		GD.Print(Position);
		PivotOffset = closestPivot;
		
		GD.Print(float.Pi*1.5f);
		if (_currentRotation >= (float.Pi * 1.5f))
			_currentRotation = 0;
		else
			_currentRotation += float.Pi / 2;
		Rotation = _currentRotation;
	}

	private void CheckForSnap()
	{
		// two potential ways of doing things: checking for a snap on only the cell type that the player is dragging the card from  (might not be obvious) OR
		// use all the snap points on the card (longer and will give "illegal" card placements)
		var snapPoints = GetTree().GetNodesInGroup("Snap"); 
		// temporary until viable snap points is established
		var cardSnapPointNode = GetNode<Control>("CardImage/Pivot01"); // need to get all of this card's pivot point and then cycle through them
		foreach (var node in snapPoints)
		{
			Control controlNode = GetNode<Control>(node.GetPath());
			var distance = (cardSnapPointNode.GlobalPosition - controlNode.GlobalPosition).Length();
			GD.Print($"distance: {distance}");
			if (distance <= 100) // snap distance is probably too great
			{
				Position = controlNode.GlobalPosition - cardSnapPointNode.Position; 
				break;
			}
		}
	}

	public void SetUpData(Texture2D image, Godot.Collections.Array<CardIcon> icons)
	{
		_cardImage.Texture = image;
		for (int i =0; i<Cell.Count; i++)
		{
			Cell[i].Bg = icons[i].Background;
			Cell[i].Icon = icons[i].Icon;
		}
	}
	
	public void LockCard()
	{
		Card= CardState.Locked;
		int i = 1;
		Control pivot = GetNodeOrNull<Control>($"CardImage/Pivot0{i}");
		while (pivot is not null)
		{
			pivot.AddToGroup("Snap");
			i++;
			pivot = GetNodeOrNull<Control>($"CardImage/Pivot0{i}");
		}
	}
	
// Snap points are enabled by Topbar based on what cell is being held by player
	public void EnableSnapPoint(int snapPoint)
	{
		Control controlNode = GetNode<Control>($"Card/Pivot0{snapPoint}");
		controlNode.AddToGroup("Snap"); // not sure on name yet. Should probably be Snap + icon/bg
	}

	public void DisableSnapPoint(int snapPoint)
	{
		Control controlNode = GetNode<Control>($"Card/Pivot0{snapPoint}");
		controlNode.RemoveFromGroup("Snap"); // not sure on name yet. Should probably be Snap + icon/bg
	}
	
}
