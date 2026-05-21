using Godot;
using Vector2 = Godot.Vector2;

public partial class DragableCard : PanelContainer
{
	[Export] private TextureRect _cardImage;
	[Export] public Godot.Collections.Array<CardCell> Cell = new();
	public enum CardState {Dealt, Drag, Rotate, Released, Snapped, Locked}
	private CardState _card;
	
	private Vector2 _eventStart = Vector2.Zero; // position of the cursor when the event starts
	private Vector2 _offset = Vector2.Zero; // offset between cursor and corner of card
	private float _currentRotation;

	private int _currentCellIcon;
	private int _currentCellBG;
	private Control _currentCellPivot;

	private Node _cardOverlaid;
	private StringName _nodeName;
	private Node _cellParent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_card = CardState.Dealt;
		SignalManager.Instance.OnMouseEntered += OnMouseEntered;
		SignalManager.Instance.OnMouseExit += OnMouseExit;
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
			
			case CardState.Snapped:
				_card = CardState.Dealt;
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
		if (_card == CardState.Locked) return; // if the card is locked, then don't allow dragging/rotating
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButtonEvent)
		{
			if (mouseButtonEvent.DoubleClick)
			{
				_eventStart = GetGlobalMousePosition();
				_card= CardState.Rotate;
				RotateCard();
			} else if (mouseButtonEvent.Pressed && _card== CardState.Dealt)
			{
				_offset = GetGlobalMousePosition() - Position;
				ZIndex = 99;
				_card= CardState.Drag;
			}
			else
			{
				if (_card== CardState.Drag) _card= CardState.Released;
				CheckForSnap();
			}
		} 
	}

	private void RotateCard()
	{
		var parentNode = _currentCellPivot.GetParent<PanelContainer>();
		PivotOffset = _currentCellPivot.Position + parentNode.Position;
		// got to figure out why the pivot is wrong when switching
		if (_currentRotation >= (float.Pi * 1.5f))
			_currentRotation = 0;
		else
			_currentRotation += float.Pi / 2;
		Rotation = _currentRotation;
		if (_card == CardState.Snapped) SignalManager.EmitOnCardPlaced(_cardOverlaid.Name,_nodeName, Name,_cellParent.Name);

	}

	private void CheckForSnap()
	{ 
		var snapPoints = GetTree().GetNodesInGroup("Snap"); //get all cells that are allowed to be overlaid
		foreach (var node in snapPoints) // let's go through each one to see if we can snap the card atop it
		{
			PanelContainer cardCellNode = GetNode<PanelContainer>(node.GetPath()); // get the parent of the anchor (PanelContainer), so we can get the data for this cell
			if (cardCellNode.Get("Available").AsBool()) // are we allowed to snap a card on this?
			{
				var icon = cardCellNode.Get("Icon").AsInt16(); 
				var bg = cardCellNode.Get("Bg").AsInt16();
				if (_currentCellIcon == icon && _currentCellBG == bg) // does the icon/bg match the icon/bg of the card? if not, can't snap there
				{
					var path = cardCellNode.GetPath() + "/Pivot"; // we need the center of the cell, which is stored in the pivot (control)
					var pivot = GetNode<Control>(path);
					var distance = (_currentCellPivot.GlobalPosition - pivot.GlobalPosition).Length(); // check the distance between the two control node
					if (distance <= 100) // snap distance is probably too great
					{
						_cellParent = _currentCellPivot.GetParent(); // we need to get the position of the Cell, so that we can add it to the pivot position
						var parentPath = _cellParent.GetPath();
						PanelContainer parentPosition = GetNode<PanelContainer>(parentPath);
						Position = pivot.GlobalPosition - (_currentCellPivot.Position + parentPosition.Position);
						_cardOverlaid = cardCellNode.GetParent().GetParent(); // needed to get the name of the card
						_card = CardState.Snapped;
						_nodeName = node.Name; 
						SignalManager.EmitOnCardPlaced(_cardOverlaid.Name,_nodeName, Name,_cellParent.Name); 
						break;
					}
					else
					{
						SignalManager.EmitOnLockDisabled();
						ZIndex = 10;
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
		_card= CardState.Locked;
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
	private void EnableSnapPoint(int snapPoint) // this may not be used
	{
		PanelContainer controlNode = GetNode<PanelContainer>($"Card/CardCell{snapPoint}");
		controlNode.AddToGroup("Snap"); // not sure on name yet. Should probably be Snap + icon/bg
	}

	public void DisableSnapPoint(int snapPoint)
	{
		PanelContainer controlNode = GetNode<PanelContainer>($"CardImage/CardCell{snapPoint}");
		controlNode.RemoveFromGroup("Snap"); // not sure on name yet. Should probably be Snap + icon/bg
	}
	
}
