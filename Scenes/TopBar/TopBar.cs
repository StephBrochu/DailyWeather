using Godot;
using System;

public partial class TopBar : TextureRect
{
	[Export] private TextureRect _weekCardDisplay;
	[Export] private TextureRect _dayCardDisplay;
	[Export] private WeekCards _weekCardsData;
	[Export] private DayConditionCards _dayCardData;
	[Export] private DeckOfCards _playCardData;
	[Export] private GameData _gameData;
	[Export] private Label _monthLabel;
	[Export] private Label _dayLabel;
	[Export] private Label _patternLabel;

	private DateTime _date;
	private DayOfWeek _today;
	
	private int _nextCard;
	private int _rightCard;
	private int _leftCard;
	private int[] _deck = [0, 1, 2, 3, 4, 5];
	private string _left = "left";
	private string _right = "right";
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalManager.Instance.OnDebugDisplayGrid += DisplayGrid;
		SignalManager.Instance.OnDayComplete += DayComplete;
		SignalManager.Instance.OnMonthComplete += MonthComplete;
		SignalManager.Instance.OnPatternComplete += PatternComplete;
		SignalManager.Instance.OnPatternNotMatching += PatternNotMatching;
		
		// this will be replaced with a menu on the start screen to allow the player to select any day they want
		_date = new DateTime(2026, 04, 27); // set up a date
		_today = _date.DayOfWeek; 
		//_date = DateTime.Now; // today's date

		SetMonthCard(); // select the correct month card
		SetDayCard(); // select the correct day card
		SetStartCards(); // set up the initial two cards, depending on the week card used
		
		ShuffleDeck();
		SignalManager.EmitOnDealCard(); // deal one card
		SignalManager.EmitOnDealCard(); // and a second
		DisplayGrid(); // for debugging purposes

	}

	private void SetMonthCard()
	{
		_gameData.WeekCard = _date.Day switch
		{
			<= 7 => 0,
			<= 14 => 1,
			<= 21 => 2,
			_ => 3
		};
		
		_weekCardDisplay.Texture = _weekCardsData.Card[_gameData.WeekCard].Image;
		var cell = _date.Month > 6 ? _date.Month - 7 : _date.Month - 1;
		_gameData.CellMonth = cell;
	}

	private void SetDayCard()
	{
		
		_gameData.Season = _date.Month switch
		{
			>= 3 and < 5 => 0,
			>= 6 and < 9 => 1,
			>= 10 and < 12 => 2,
			_ => 3
		};

		int seasonUsed = (_gameData.Season - 2 > 0) ? _gameData.Season - 2 : _gameData.Season;
		var card = (Math.DivRem(_gameData.Season, 2, out _) == 0)
			? _dayCardData.Card[(int)_today].Front
			: _dayCardData.Card[(int)_today].Back;
		_dayCardDisplay.Texture = card.Image;
		
		switch ( card.ArrowPosition)
		{
			case 0:
				_gameData.PatternType = "Icon";
				_gameData.PatternNumber = _weekCardsData.Card[_gameData.WeekCard].Icon1;
			break;
			
			case 1:
				_gameData.PatternType = "Icon";
				_gameData.PatternNumber = _weekCardsData.Card[_gameData.WeekCard].Icon2;
				break;
			
			case 2:
				_gameData.PatternType = "BG";
				_gameData.PatternNumber = _weekCardsData.Card[_gameData.WeekCard].Background1;
				break;
			
			case 3:
				_gameData.PatternType = "BG";
				_gameData.PatternNumber = _weekCardsData.Card[_gameData.WeekCard].Background2;
				break;
		}

		// gather + store pattern used
		foreach (ConditionPattern pair in card.Season[seasonUsed].Grid)
		{
			_gameData.PatternUsed.Add(pair);
		}
		
		_gameData.DayCard = (int)_today;
		_gameData.CellDay = (_date.Day -1) % 6 ;
	}
		
	private void SetStartCards()
	{
		var month = _weekCardsData.Card[_gameData.WeekCard].Month;
		var day = _weekCardsData.Card[_gameData.WeekCard].Day;
		
		SignalManager.EmitOnSetMonthCard(month.Orientation, month.Row, month.Column, month.Anchor, _date.Month);
		SignalManager.EmitOnSetDayCard(day.Orientation, day.Row, day.Column, day.Anchor, _date.Day);
	}

	private void ShuffleDeck()
	{
		int count = _playCardData.Cards.Count;
		var arrayCopy = _playCardData.Cards;
		// first, shuffle the deck
		while (count > 1)
		{
			int i = Random.Shared.Next(count --);
			(arrayCopy[i], arrayCopy[count]) = (arrayCopy[count], arrayCopy[i]);
		}
		// now, flip some cards
		foreach (var card in arrayCopy)
		{
			int randomNum = Random.Shared.Next(2);
			if (randomNum == 1)
			{
				_gameData.Deck.Add(card.Front);
			}
			else
			{
				_gameData.Deck.Add(card.Back);
			}
		}
	}

	private void DayComplete() // should change color of the cell
	{
		_dayLabel.Text = "Only current Day is visible";
		_dayLabel.LabelSettings.FontColor = Color.Color8(0, 255, 0);
		_gameData.DayComplete = true;
	}
	
	private void MonthComplete() // should change color of the cell
	{
		_monthLabel.Text = "Only current Day is visible";
		_monthLabel.LabelSettings.FontColor = Color.Color8(0, 255, 0);
		_gameData.MonthComplete = true;
	}

	private void PatternComplete() // need to add visual feedback on grid
	{
		_patternLabel.Text = "Pattern is matched!";
		_patternLabel.LabelSettings.FontColor = Color.Color8(0, 255, 0);
		_gameData.PatternComplete = true;
	}

	private void PatternNotMatching()
	{
		_patternLabel.Text = "Pattern not matching";
		_patternLabel.LabelSettings.FontColor = Color.Color8(255, 0, 0);
		_gameData.PatternComplete = false;
	}
	
	private void DisplayGrid()
	{
		// little debugging
		GD.Print("    [00][01][02][03][04][05][06][07][08][09][10][11][12][13][14]");
		for (int x = 0; x< _gameData.Grid.Count; x++)
		{
			string line = $"[{x:D2}]";
			foreach (var cell in _gameData.Grid[x].GridRow)
			{
				if (cell.Locked)
				{
					line += $"*{cell.Icon}{cell.Background}*";
				}
				else
				{
					line += $" {cell.Icon}{cell.Background} ";
				}
			}
			GD.Print(line);
		}
		//GD.Print($"Cell on Month Card: {_gameData.CellMonth+1}; Cell on Day card: {_gameData.CellDay+1}");
		//GD.Print($"Week Card: {_gameData.WeekCard}; Day Card: {_gameData.DayCard}");
		//GD.Print(($"Season: {_gameData.Season}"));
		//GD.Print(String.Join("\n", _deck));
		//GD.Print(String.Join("\n", _cardSide));
	}
}
