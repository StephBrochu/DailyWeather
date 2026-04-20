using Godot;
using Vector2 = Godot.Vector2;

public partial class DragableCard : PanelContainer
{
	public enum CardState {Dealt, Drag, Rotate, Released, Locked}
	private CardState _card = CardState.Dealt;
	
	private Vector2 _eventStart = Vector2.Zero; // position of the cursor when the event starts
	private Vector2 _offset = Vector2.Zero; // offset between cursor and corner of card
	private float _currentRotation;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		if (_card == CardState.Locked) return; // if the card is locked, then don't allow dragging/rotating
		switch (_card)
		{
			case CardState.Drag:
				Position = GetGlobalMousePosition() - _offset;
				break;
			
			case CardState.Rotate:
				_card = CardState.Dealt;
				break;
			
			case CardState.Released:
				CheckForSnap();
				_card = CardState.Dealt;
				break;
		}
	}

	public override void _GuiInput(InputEvent @event)
	{
		
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButtonEvent)
		{
			if (mouseButtonEvent.DoubleClick)
			{
				_eventStart = GetGlobalMousePosition();
				_card = CardState.Rotate;
				RotateCard();
			} else if (mouseButtonEvent.Pressed && _card == CardState.Dealt)
			{
				_offset = GetGlobalMousePosition() - Position;
				_card = CardState.Drag;
			}
			else
			{
				if (_card == CardState.Drag) _card = CardState.Released;
			}
		}
	}

	private void RotateCard()
	{
		var pivotPoints = GetTree().GetNodesInGroup("Pivot");
		//first, find the closest pivot point by finding out where the mouse is on the card
		//Vector2 mousePosition = _eventStart - Position;
		
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
		var snapPoints = GetTree().GetNodesInGroup("Snap"); // snap or snap + icon+bg?
		// temporary until viable snap points is established
		var cardSnapPointNode = GetNode<Control>("CardImage/Pivot01");
		foreach (var node in snapPoints)
		{
			Control controlNode = GetNode<Control>(node.GetPath());
			var distance = (cardSnapPointNode.GlobalPosition - controlNode.GlobalPosition).Length();
			GD.Print($"distance: {distance}");
			if (distance <= 100) Position = controlNode.GlobalPosition - cardSnapPointNode.Position; // snap distance is probably too great
			break;
		}
	}

	public void LockCard()
	{
		_card = CardState.Locked;
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
