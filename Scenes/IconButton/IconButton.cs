using Godot;

public partial class IconButton : TextureButton
{
	[Export] private TextureRect _highlight;
	[Export] private TextureRect _unavailableCell;
	[Export] private TextureRect _pivotCell;
	[Export] public Control PivotPoint;
	private static IconButton Instance { get; set; }
	private bool _available = false;
	private int _icon;
	private int _bg;
	public int Row = -1;
	public int Col = -1;
	private int _rowOffset;
	private int _colOffset;
	private bool _pivot = false;
	public int Card;
	public bool Front;
	private bool _clickedOnce = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Disabled = true;
		_highlight.Visible = false;
		_unavailableCell.Visible = false;
		_pivotCell.Visible = false;
		Instance = this;
		Instance.Pressed += Clicked;
		Instance.MouseEntered += OnMouseEnter;
		Instance.MouseExited += OnMouseExit;
		switch (Name)
		{
			case "TopLeft":
				_rowOffset = 0;
				_colOffset = 0;
				break;
			case "TopMiddle":
				_rowOffset = 0;
				_colOffset = -1;
				break;
			case "TopRight":
				_rowOffset = 0;
				_colOffset = -2;
				break;
			case "BottomLeft":
				_rowOffset = -1;
				_colOffset = 0;
				break;
			case "BottomMiddle":
				_rowOffset = -1;
				_colOffset = -1;
				break;
			case "BottomRight":
				_rowOffset = -1;
				_colOffset = -2;
				break;
		}
	}

	public void EnableButton(CardIcon icons, int card, bool front)
	{
		_highlight.Visible = true;
		_icon = icons.Icon;
		_bg = icons.Background;
		_available = true;
		Card = card;
		Front = front;
	}

	public void EnablePivotButton()
	{
		_highlight.Visible = true;
		_available = true;
		_pivotCell.Visible = true;
		_pivot = true;
	}

	public void UnavailableCell()
	{
		_unavailableCell.Visible = true;
	}

	private void Clicked()
	{
		SignalManager.EmitOnLockEnabled();
		if (!_pivot)
		{
			if (_clickedOnce)
			{
				SignalManager.EmitOnSelectNextCard(Card, Front, _rowOffset, _colOffset);
			}
			else
			{
				Instance._clickedOnce = true;
				SignalManager.EmitOnPlaceCard(Card, Front, _rowOffset, _colOffset);
			}
		}
		else
		{
			SignalManager.EmitOnRotateCard();
		}
		
	}

	public void HighlightCell()
	{
		_highlight.Visible = true;
	}

	public void UnHighlightCell()
	{
		_highlight.Visible = false;
	}
	
	private void OnMouseEnter()
	{
		if (!_available) return;
		_highlight.Visible = true;
		//SignalManager.EmitOnMouseEntered(_icon, _bg);
		//GD.Print($"Icon: {_icon}; BG: {_bg}");
	}

	private void OnMouseExit()
	{
		if (!_available) return;
		_highlight.Visible = false;
		SignalManager.EmitOnMouseExit();
	}
	
}
