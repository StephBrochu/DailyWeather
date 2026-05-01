using Godot;
using Vector2 = Godot.Vector2;

public partial class DragableCard : PanelContainer
{
	[Export] private TextureRect _cardImage;
	[Export] public Godot.Collections.Array<CardCell> Cell = new();
	public enum CardState {Dealt, Drag, Rotate, Released, Locked}
	public CardState Card;
	
	private Vector2 _eventStart = Vector2.Zero; // position of the cursor when the event starts
	private Vector2 _offset = Vector2.Zero; // offset between cursor and corner of card
	private float _currentRotation;

	private int _currentCellIcon;
	private int _currentCellBG;
	private Control _currentCellPivot;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Card = CardState.Dealt;
		SignalManager.Instance.OnMouseEntered += OnMouseEntered;
		SignalManager.Instance.OnMouseExit += OnMouseExit;
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

	private void OnMouseEntered(int icon, int bg, Control pivot )
	{
		_currentCellBG = bg;
		_currentCellIcon = icon;
		_currentCellPivot = pivot;
	}

	private void OnMouseExit()
	{
		_currentCellBG = 0;
		_currentCellIcon = 0;
		_currentCellPivot = null;
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
		var parentNode = _currentCellPivot.GetParent<PanelContainer>();
		PivotOffset = _currentCellPivot.Position + parentNode.Position;
		GD.Print(PivotOffset);
		if (_currentRotation >= (float.Pi * 1.5f))
			_currentRotation = 0;
		else
			_currentRotation += float.Pi / 2;
		Rotation = _currentRotation;
	}

	private void CheckForSnap()
	{ // defaulting to CardCell0 of dragged card. why?
		var snapPoints = GetTree().GetNodesInGroup("Snap");
		foreach (var node in snapPoints)
		{
			PanelContainer cardCellNode = GetNode<PanelContainer>(node.GetPath());
			if (cardCellNode.Get("Available").AsBool())
			{
				var icon = cardCellNode.Get("Icon").AsInt16();
				var bg = cardCellNode.Get("Bg").AsInt16();
				if (_currentCellIcon == icon && _currentCellBG == bg)
				{
					var path = cardCellNode.GetPath() + "/Pivot";
					var pivot = GetNode<Control>(path);
					var distance = (_currentCellPivot.GlobalPosition - pivot.GlobalPosition).Length(); // let's get the correct pivot position
					if (distance <= 200) // snap distance is probably too great
					{
						GD.Print(_currentCellPivot.GetParent().Name);
						var parent = _currentCellPivot.GetParent();
						var parentPath = parent.GetPath();
						PanelContainer parentPosition = GetNode<PanelContainer>(parentPath);
						Position = pivot.GlobalPosition - (_currentCellPivot.Position + parentPosition.Position);
						break;
					}
				}
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
		int i = 0;
		PanelContainer cell = GetNodeOrNull<PanelContainer>($"CardImage/CardCell{i}");
		while (cell is not null)
		{
			cell.AddToGroup("Snap");
			i++;
			cell = GetNodeOrNull<PanelContainer>($"CardImage/CardCell{i}");
		}
	}
	
// Snap points are enabled by Topbar based on what cell is being held by player
	public void EnableSnapPoint(int snapPoint)
	{
		PanelContainer controlNode = GetNode<PanelContainer>($"Card/CardCell{snapPoint}");
		controlNode.AddToGroup("Snap"); // not sure on name yet. Should probably be Snap + icon/bg
	}

	public void DisableSnapPoint(int snapPoint)
	{
		PanelContainer controlNode = GetNode<PanelContainer>($"Card/CardCell{snapPoint}");
		controlNode.RemoveFromGroup("Snap"); // not sure on name yet. Should probably be Snap + icon/bg
	}
	
}
