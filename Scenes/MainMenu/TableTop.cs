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
	private List<string> _dayCellsUncovered = [];
	private List<string> _monthCellsUncovered = [];
	public List<string> PatternCells = [];
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
		if (_gameData.Deck.Count == 0) {
			GD.Print("Deck is empty");
			return; // deck is empty. May want to let user know, skip for now
		}
		DragableCard card = _playCard.Instantiate<DragableCard>();
		var topCard = _gameData.Deck.First();
		_gameData.Deck.RemoveAt(0); // card is dealt, so remove it from the list
		card.SetUpData(topCard.Image, topCard.Icon);
		card.Name = $"Card{cardDealt}";
		cardDealt++;
		card.Rotation = 0;
		string anchor = "FirstCard";
		// is there another card dealt?
		var checkForPlayableCard = GetTree().GetNodesInGroup("PlayableCard");
		if ( checkForPlayableCard.Count != 0)
		{
			// we will need to check if a card is present on/near the anchor before using it
			Control check = GetNode<Control>(anchor);
			DragableCard cardPresent = GetNode<DragableCard>(GetTree().GetFirstNodeInGroup("PlayableCard").GetPath());
			var distance = (check.GlobalPosition - cardPresent.GlobalPosition).Length();
			if (distance <= cardPresent.Size.Y)
			{
				anchor = "SecondCard";
			}
		}
		card.AddToGroup("PlayableCard");
		Control selectAnchor = GetNode<Control>(anchor); // select the first anchor 
		card.Position = selectAnchor.Position;
		AddChild(card);
	}
	
	private void UpdateGrid(DragableCard cardSelected, Godot.Collections.Array<CardCell> iconData, string group, int row, int col, float rotation, string type)
	{
		bool onlyMonthVisible = true; // may need to keep track of all cells instead
		bool onlyDayVisible = true;
		int offsetTableToUse = (int)rotation / 90;
		for (int i = 0; i < 6; i++)
		{
			var offsetTable = _offsetTables.Table[offsetTableToUse].Cell[i];
			var offsetRow = row + offsetTable.rowOffset;
			var offsetCol = col + offsetTable.colOffset;
			var gameData = _gameData.Grid[offsetRow].GridRow[offsetCol];
			// need to check if there was info in the cell before and if yes, then disable the cell
			bool cellEmpty = (gameData.Icon == 0) ? true: false;
			if (!cellEmpty)
			{
				var cardName = gameData.CardName;
				var cellIndex = gameData.CellIndex;
				
				DragableCard card = GetNode<DragableCard>(cardName);
				card.DisableSnapPoint(cellIndex);
			}
			
			switch (type) // depending on the type of card being "locked in", a few things need to be checked
			{
				case "day": // if it's a day card, check to see if the current cell is the day's cell, ie one that shouldn't be covered
					if (i == _gameData.CellDay)
					{
						gameData.Locked = true;
					}
					else
					{
						_dayCellsUncovered.Add($"{offsetRow}{offsetCol}");
					}
					break;
				
				case "month": // if it's a month card, check to see if the current cell is the month's cell, ie one that shouldn't be covered
					if (i == _gameData.CellMonth)
					{
						gameData.Locked = true;
					}
					else
					{
						_monthCellsUncovered.Add($"{offsetRow}{offsetCol}");
					}

					break;
				
				default: // if it is a regular card, check if the cell being covered is a Day or Month cell, and if it is, remove it from that type's list
					if (gameData.CardName == "DayCard")
					{
						_dayCellsUncovered.Remove($"{offsetRow}{offsetCol}");
						if(_dayCellsUncovered.Count == 0) SignalManager.EmitOnDayComplete(); // if all the day's card cell have been covered, let the player know that they have completed this condition
					} else if (gameData.CardName == "MonthCard")
					{
						_monthCellsUncovered.Remove($"{offsetRow}{offsetCol}");
						if(_monthCellsUncovered.Count == 0 ) SignalManager.EmitOnMonthComplete(); // if all the month's card cell have been covered, let the player know that they have completed this condition
					}
					break;
			}
			
			// set up some variables to check if the new cell is one that we need for the pattern or if we're overwriting a cell that is valid, but with something that isn't
			var patternToMatch = (_gameData.PatternType == "Icon") ? gameData.Icon : gameData.Background;
			var cardPatternToMatch = (_gameData.PatternType == "Icon") ? cardSelected.Cell[i].Icon : cardSelected.Cell[i].Bg;
			var location = $"{offsetRow:D2}{offsetCol:D2}";
			
			if (cellEmpty)
			{
				if (_gameData.PatternNumber == cardPatternToMatch) PatternCells.Add(location);
			} else {
				if (_gameData.PatternNumber == patternToMatch) {
					if (_gameData.PatternNumber != cardPatternToMatch) PatternCells.Remove(location);
					
				} else {
					if (_gameData.PatternNumber == cardPatternToMatch) PatternCells.Add(location); 
				}
			}
			
			gameData.Icon = iconData[i].Icon;
			gameData.Background = iconData[i].Bg;
			gameData.CardName = group;
			gameData.CellIndex = i;
			cardSelected.Cell[i].Col = offsetCol;
			cardSelected.Cell[i].Row = offsetRow;
			UpdateCellDictionary(offsetRow, offsetCol,iconData[i].Icon, iconData[i].Bg);
		}
		CheckForPatternCondition();
	}

	private void UpdateCellDictionary(int row, int column, int icon, int bg)
	{
		string key = $"{row:D2}{column:D2}";
		string value = $"{icon}{bg}";
		_cellsOccupied[key] = value;
	}

	private void CheckForPatternCondition()
	{
		bool patternMatched = false;
		foreach (String coordinates in PatternCells)
		{
			patternMatched = true;
			int row = int.Parse((coordinates.Substring(0,2)));
			int col = int.Parse(coordinates.Substring(2));
			foreach (ConditionPattern cell in _gameData.PatternUsed)
			{
				int offsetRow = row + cell.rowOffset;
				int offsetCol = col + cell.colOffset;
				var patternToMatch = (_gameData.PatternType == "Icon") 
					? _gameData.Grid[offsetRow].GridRow[offsetCol].Icon 
					: _gameData.Grid[offsetRow].GridRow[offsetCol].Background;
				if (_gameData.PatternNumber != patternToMatch)
				{
					patternMatched = false;
					break;
				}
			}
			if (patternMatched)
			{
				SignalManager.EmitOnPatternComplete();
				break;
			}
		}
		if (!patternMatched) SignalManager.EmitOnPatternNotMatching(); // either the pattern was never matched or is no longer matching
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
		_cardPlayed.RemoveFromGroup("PlayableCard");
		SignalManager.EmitOnDebugDisplayGrid();
		DealCard();
	}
}
