using Godot;
using System;
using System.Collections.Generic;

public partial class PlayspaceGrid : Control
{
	[Export] private PackedScene _monthCard; // this is PlayedCard
	[Export] private SetUpCards _monthData; // this is the data, thus SetUpMonth
	[Export] private PackedScene _dayCard;
	[Export] private SetUpCards _dayData;
	[Export] private PackedScene _playCard;
	[Export] private DeckOfCards _playData;
	[Export] private PackedScene _gridRef;
	[Export] private GridContainer _grid;
	[Export] private GameData _gameData;
	[Export] private OffsetTables _offsetTables;
	[Export] private PackedScene _cell;
	
	private int _cardPlayedSoFar;
	private Godot.Collections.Array<CardIcon> _cardIcons = new();
	private Dictionary<string, string> _cellsOccupied = new();
	private int _currentCell;
	private List<IconButton> _highlightedCells = new();

	private bool _currentCardFront;
	private PlayedCard _currentCardSelected;
	private IconButton _currentCellSelected;
	private int _cardSelectedNumber;

	private bool _debug;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//SignalManager.Instance.OnSetMonthCard += SetMonthCard;
		//SignalManager.Instance.OnSetDayCard += SetDayCard;
		//SignalManager.Instance.OnMouseEntered += HighlightCells;
		//SignalManager.Instance.OnMouseExit += UnHighlightCell;
		SignalManager.Instance.OnPlaceCard += AddCard;
		SignalManager.Instance.OnSelectNextCard += AddNextCard;
		SignalManager.Instance.OnLockCard += LockCardIn;
		if (_debug) DebugGrid();
	}

	private void SetMonthCard(float rotation, int row, int col, int date)
	{
		PlayedCard cardSelected = _monthCard.Instantiate<PlayedCard>();
		int card = Math.DivRem(date, 6, out _);
		cardSelected.SetUpMonth(rotation, card);
		cardSelected.Name = GameData.MonthCardName;
		SetCard(cardSelected, rotation, row, col);
		AddChild(cardSelected);
		cardSelected.Cell[_gameData.CellMonth].UnavailableCell();
		UpdateGrid(cardSelected, _monthData.Card[card].Front.Icon, cardSelected.Name, row, col, rotation, "month");
	}
	
	private void SetDayCard(float rotation, int row, int col, int date)
	{
		PlayedCard cardSelected = _dayCard.Instantiate<PlayedCard>();
		int card = Math.DivRem(date, 6, out _);
		cardSelected.SetUpDay(rotation, card);
		cardSelected.Name = GameData.DayCardName;
		SetCard(cardSelected, rotation, row, col);
		AddChild(cardSelected);
		cardSelected.Cell[_gameData.CellDay].UnavailableCell();
		UpdateGrid(cardSelected, _dayData.Card[card].Front.Icon, cardSelected.Name, row, col, rotation, "day");
	}

	private void AddCard(int card, bool front, int rowOffset, int colOffset)
	{
		if (_highlightedCells.Count == 0) return;
		var currentCard =  GetNodeOrNull($"card{_cardPlayedSoFar}");
		if (currentCard != null) AddNextCard(card, front,rowOffset, colOffset);
		var cellButton = _highlightedCells[_currentCell];
		PlayedCard cardSelected = _playCard.Instantiate<PlayedCard>();
		cardSelected.Card(0, card, front);
		cardSelected.Name = $"card{_cardPlayedSoFar}";
		SetCard(cardSelected, 0, cellButton.Row+rowOffset, cellButton.Col+colOffset);
		AddChild(cardSelected);
		
		// store the variables if we want to lock this card in
		_currentCardFront = front;
		_currentCardSelected = cardSelected;
		_currentCellSelected = _highlightedCells[_currentCell];
		_cardSelectedNumber = card;

		var button = Math.Abs(colOffset + (rowOffset*3));
		cardSelected.EnableButtonAndSetPivot(button);

	}

	private void AddNextCard(int card, bool front, int rowOffset, int colOffset) 
	{
		var currentCard = GetNode($"card{_cardPlayedSoFar}");
		currentCard.Free();
		if (_currentCell > (_highlightedCells.Count-1)) _currentCell = 0;
		AddCard(card, front, rowOffset, colOffset);
	}

	private void LockCardIn()
	{
		 var sideOfCard = _currentCardFront switch
		 {
		 	true => _playData.Cards[_cardSelectedNumber].Front.Icon,
		 	_ => _playData.Cards[_cardSelectedNumber].Back.Icon
		 };
		UpdateGrid(_currentCardSelected, sideOfCard, _currentCardSelected.Name, _currentCellSelected.Row, _currentCellSelected.Col, 0, "");
		
		SignalManager.EmitOnDealNextCard(_cardSelectedNumber);
		SignalManager.EmitOnLockDisabled();
		_cardPlayedSoFar++;
		//SignalManager.EmitOnDebug();

	}
	
	private void SetCard(PlayedCard cardSelected, float orientation, int rowSet, int colSet)
	{
		int offsetX = _grid.GetThemeConstant("h_separation");
		int offsetY = _grid.GetThemeConstant("v_separation");
		int gridSize = _grid.Columns;
		int cellSizeX = (Convert.ToInt32(Size.X) - ((gridSize - 1) * offsetX))/gridSize;
		int cellSizeY = (Convert.ToInt32(Size.Y) - ((gridSize - 1) * offsetX))/gridSize;
		int positionX = (rowSet*cellSizeX)+(offsetX*rowSet);
		int positionY = (colSet*cellSizeY)+(offsetY*colSet);
		Vector2 position = new Vector2(positionY, positionX);
		cardSelected.Position = position;
		cardSelected.Rotation = float.DegreesToRadians(orientation);
	}

	private void UpdateGrid(PlayedCard cardSelected, Godot.Collections.Array<CardIcon> iconData, string group, int row, int col, float rotation, string type)
	{
		int offsetTableToUse = (int)rotation % 90;
		for (int i = 0; i < 6; i++)
		{
			var offsetTable = _offsetTables.Table[offsetTableToUse].Cell[i];
			var offsetRow = row + offsetTable.xOffset;
			var offsetCol = col + offsetTable.yOffset;
			var gameData = _gameData.Grid[offsetRow].GridRow[offsetCol];
			gameData.Icon = iconData[i].Icon;
			gameData.Background = iconData[i].Background;
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

			UpdateCellDictionary(offsetRow, offsetCol,iconData[i].Icon, iconData[i].Background);
		}
	}

	private void UpdateCellDictionary(int row, int column, int icon, int bg)
	{
		string key = $"{row:D2}{column:D2}";
		string value = $"{icon}{bg}";
		_cellsOccupied[key] = value;
	}
	
	private void HighlightCells(int icon, int bg)
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
				PlayedCard card = GetNode<PlayedCard>(name);
				if (_gameData.Grid[row].GridRow[col].Locked) continue;
				var cell = card.Cell[_gameData.Grid[row].GridRow[col].CellIndex];
				cell.HighlightCell();
				_highlightedCells.Add(cell);
			}
		}
	}

	private void UnHighlightCell()
	{
		foreach (IconButton highlightedButton in _highlightedCells)
		{
			highlightedButton.UnHighlightCell();
		}
		_highlightedCells.Clear();
	}
	private void DebugGrid()
	{
		for (int row = 1; row <= 15; row++)
		{
			for (int col = 1; col <= 15; col++)
			{
				DebugCell debugCell = _cell.Instantiate<DebugCell>();
				debugCell.SetName($"{row:D2}{col:D2}");
				debugCell.ShowCell();
				debugCell.SetUpCardData($"{row:D2}{col:D2}","00");
				_grid.AddChild(debugCell);
				
			}
		}
	}
}
