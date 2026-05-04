using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TableTop : TextureRect
{
	[Export] private SetUpCards _monthData; // this is the data, thus SetUpMonth
	[Export] private SetUpCards _dayData;
	[Export] private PackedScene _playCard;
	[Export] private DeckOfCards _playData;
	[Export] private GameData _gameData;
	[Export] private OffsetTables _offsetTables;

	private Godot.Collections.Dictionary<string, string> _cellsOccupied = new();
	private List<CardCell> _highlightedCells = [];
	private bool _debug;
	private int cardDealt = 1;

	private DragableCard _cardPlayed;
	private int _cell0LocationRow;
	private int _cell0LocationCol;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalManager.Instance.OnSetMonthCard += SetMonthCard;
		SignalManager.Instance.OnSetDayCard += SetDayCard;
		SignalManager.Instance.OnDealCard += DealCard;
		SignalManager.Instance.OnMouseEntered += HighlightCells;
		SignalManager.Instance.OnMouseExit += UnHighlightCell;
		SignalManager.Instance.OnCardPlaced += CheckForValidPlacement;
		SignalManager.Instance.OnLockCard += LockCardInAndDealNewCard;
	}

	private void SetMonthCard(float rotation, int row, int col, int anchor, int date)
	{
		DragableCard cardSelected = _playCard.Instantiate<DragableCard>();
		int card = Math.DivRem(date, 6, out _);
		var currentCard = _monthData.Card[card].Front;
		cardSelected.SetUpData(currentCard.Image, currentCard.Icon);
		cardSelected.Name = GameData.MonthCardName;
		cardSelected.Rotation = float.DegreesToRadians(rotation);
		Control selectedAnchor = GetNode<Control>($"Anchor{anchor}");
		cardSelected.Position = selectedAnchor.Position;
		AddChild(cardSelected);
		cardSelected.LockCard();
		cardSelected.Cell[_gameData.CellMonth].CellUnavailable();
		UpdateGrid(cardSelected, cardSelected.Cell, cardSelected.Name, row, col, rotation, "month");
	}
	
	private void SetDayCard(float rotation, int row, int col, int anchor, int date)
	{
		DragableCard cardSelected = _playCard.Instantiate<DragableCard>();
		int card = Math.DivRem(date, 6, out _);
		var currentCard = _dayData.Card[card].Front;
		cardSelected.SetUpData(currentCard.Image, currentCard.Icon);
		cardSelected.Name = GameData.DayCardName;
		cardSelected.Rotation = float.DegreesToRadians(rotation);
		Control selectedAnchor = GetNode<Control>($"Anchor{anchor}");
		cardSelected.Position = selectedAnchor.Position;
		AddChild(cardSelected);
		cardSelected.LockCard();
		cardSelected.Cell[_gameData.CellDay].CellUnavailable();
		UpdateGrid(cardSelected, cardSelected.Cell, cardSelected.Name, row, col, rotation, "day");
	}

	public void DealCard()
	{
		if (_gameData.Deck.Count == 0) return; // deck is empty. May want to let user know, skip for now
		DragableCard card = _playCard.Instantiate<DragableCard>();
		var topCard = _gameData.Deck.First();
		_gameData.Deck.RemoveAt(0); // card is dealt, so remove it from the list
		card.SetUpData(topCard.Image, topCard.Icon);
		card.Name = $"Card{cardDealt}";
		cardDealt++;
		card.Rotation = 0;
		Control selectAnchor = GetNode<Control>("FirstCard"); // we will need to check if a card is present on/near the anchor before using it
		card.Position = selectAnchor.Position;
		AddChild(card);
	}
	
	private void UpdateGrid(DragableCard cardSelected, Godot.Collections.Array<CardCell> iconData, string group, int row, int col, float rotation, string type)
	{ 
		int offsetTableToUse = (int)rotation / 90;
		for (int i = 0; i < 6; i++)
		{
			var offsetTable = _offsetTables.Table[offsetTableToUse].Cell[i];
			var offsetRow = row + offsetTable.rowOffset;
			var offsetCol = col + offsetTable.colOffset;
			var gameData = _gameData.Grid[offsetRow].GridRow[offsetCol];
			// need to check if there was info in the cell before and if yes, then disable the cell
			if (gameData.Icon !=0)
			{
				GD.Print($"There is data in cell {offsetRow}/{offsetCol}");
				var cardName = gameData.CardName;
				var cellIndex = gameData.CellIndex;
				DragableCard card = GetNode<DragableCard>(cardName);
				card.DisableSnapPoint(cellIndex);
			}
			
			gameData.Icon = iconData[i].Icon;
			gameData.Background = iconData[i].Bg;
			gameData.CardName = group;
			gameData.CellIndex = i;
			if ((type == "month" && i == _gameData.CellMonth) || (type == "day" && i == _gameData.CellDay))
			{
				gameData.Locked = true;
				continue;
			}

			cardSelected.Cell[i].Col = offsetCol;
			cardSelected.Cell[i].Row = offsetRow;

			if (_debug) { var parent = GetNode<GridContainer>("GameGrid");
				DebugCell cellToUpdate = parent.GetNode<DebugCell>($"{offsetRow:D2}{offsetCol:D2}");
				cellToUpdate.UpdateData($"{gameData.Icon}{gameData.Background}");
			}
			UpdateCellDictionary(offsetRow, offsetCol,iconData[i].Icon, iconData[i].Bg);
		}
	}

	private void UpdateCellDictionary(int row, int column, int icon, int bg)
	{
		string key = $"{row:D2}{column:D2}";
		string value = $"{icon}{bg}";
		_cellsOccupied[key] = value;
	}
	
	private void HighlightCells(int icon, int bg, Control pivot)
	{
		foreach (KeyValuePair<string,string> pair in _cellsOccupied)
		{
			var row = int.Parse(pair.Key[..2]);
			var col = int.Parse(pair.Key.Substring(2, 2));
			var cellIcon = pair.Value[0] - '0';
			var cellBg = pair.Value[1] - '0';
			if (cellIcon == icon && cellBg == bg)
			{
				var name = _gameData.Grid[row].GridRow[col].CardName;
				DragableCard card = GetNode<DragableCard>(name);
				if (_gameData.Grid[row].GridRow[col].Locked) continue;
				var cell = card.Cell[_gameData.Grid[row].GridRow[col].CellIndex];
				cell.HighlightCell();
				_highlightedCells.Add(cell);
			}
		}
	}

	private void UnHighlightCell()
	{
		foreach (CardCell highlightedButton in _highlightedCells)
		{
			highlightedButton.UnHighlightCell();
		}
		_highlightedCells.Clear();
	}

	private void CheckForValidPlacement(string cardOverlaid, string cellOverlaid, string newCard, string newCell)
	{
		_cardPlayed = GetNode<DragableCard>(newCard);
		int rotationTableToUse = (int)((180/Math.PI) * _cardPlayed.Rotation)/90; // we need the rotation of the card being played
		
		PanelContainer cardOverlaidCell = GetNode<PanelContainer>($"{cardOverlaid}/CardImage/{cellOverlaid}"); // cell of the card being overlaid
		int cardOverlaidRow = (int)cardOverlaidCell.Get("Row"); // and the location in the grid
		int cardOverlaidCol = (int)cardOverlaidCell.Get("Col");
		
		// we need the location on the grid of CardCell0 of the played card
		int cellIndex = newCell.Last().ToString().ToInt();
		var offsetTable = _offsetTables.Table[rotationTableToUse].Cell[cellIndex];
		_cell0LocationRow = cardOverlaidRow - offsetTable.rowOffset;
		_cell0LocationCol = cardOverlaidCol - offsetTable.colOffset;

		bool valid = true;
		//GD.Print($"Offset Table: {rotationTableToUse}; Cell0 location: R{_cell0LocationRow:d2}, C{_cell0LocationCol:d2}");
		for (int i = 0; i < 6; i++)
		{
			var offsetRow = _cell0LocationRow + _offsetTables.Table[rotationTableToUse].Cell[i].rowOffset;
			var offsetCol = _cell0LocationCol + _offsetTables.Table[rotationTableToUse].Cell[i].colOffset;

			//GD.Print($"Cell{i}; OffsetX: {_offsetTables.Table[rotationTableToUse].Cell[i].rowOffset}; OffsetY{_offsetTables.Table[rotationTableToUse].Cell[i].colOffset}");	
			//GD.Print($"Cell{i}; Row: {offsetRow}; Col: {offsetCol}");
			if (_gameData.Grid[offsetRow].GridRow[offsetCol].Locked)
			{
				valid = false;
				break;
			}
		}

		if (valid)
		{
			SignalManager.EmitOnLockEnabled();
		}
		else
		{
			SignalManager.EmitOnLockDisabled();
			_cardPlayed = null;
			_cell0LocationRow = -1;
			_cell0LocationCol = -1;
		}
	}

	private void LockCardInAndDealNewCard()
	{
		_cardPlayed.LockCard();
		_cardPlayed.ZIndex = cardDealt;
		float rotation = float.RadiansToDegrees(_cardPlayed.Rotation);
		UpdateGrid(_cardPlayed, _cardPlayed.Cell, _cardPlayed.Name, _cell0LocationRow, _cell0LocationCol, rotation, "Card");
		SignalManager.EmitOnDebugDisplayGrid();
		DealCard();
	}
}
