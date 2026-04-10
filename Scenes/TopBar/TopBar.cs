using Godot;
using System;

public partial class TopBar : TextureRect
{
	[Export] private TextureRect _weekCardDisplay;
	[Export] private TextureRect _dayCardDisplay;
	[Export] private Control _leftCardDisplay;
	[Export] private Control _rightCardDisplay;
	[Export] private PackedScene _availableCard;
	[Export] private PackedScene _playgrid;
	[Export] private WeekCards _weekCardsData;
	[Export] private DayConditionCards _dayCardData;
	[Export] private DeckOfCards _playCardData;
	[Export] private GameData _gameData;

	private DateTime _date;
	private DayOfWeek _today;
	
	private int _nextCard;
	private int _rightCard;
	private int _leftCard;
	private int[] _deck = [0, 1, 2, 3, 4, 5];
	private bool[] _cardSide = [true, true, true, true, true, true];
	private string _left = "left";
	private string _right = "right";
	private PlayedCard _card;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalManager.Instance.OnDealNextCard += DealNewCard;
		//SignalManager.Instance.OnDebug += DisplayGrid; 
		// this will be replaced with a menu on the start screen to allow the player to select any day they want
		_today = DateTime.Today.DayOfWeek; 
		_date = new DateTime(2026, 04, 06); // set up a date
		//_date = DateTime.Now; // today's date

		SetMonthCard(); // select the correct month card
		SetDayCard(); // select the correct day card
		SetStartCards(); // set up the initial two cards, depending on the week card used
		
		// shuffle DeckOfCard
		_deck = ShuffleCardsList(_deck);
		// and flip some of them
		_cardSide = FlipRandomCards(_cardSide);
		
		//Deal the first two cards
		_leftCard = _deck[_nextCard];
		DealPlayCard(_leftCardDisplay, _left);
		_rightCard = _deck[_nextCard];
		DealPlayCard(_rightCardDisplay, _right);
		
		//DisplayGrid(); // for debugging purposes

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
		
		_dayCardDisplay.Texture = _dayCardData.Card[(int)_today].Image[Math.DivRem(_gameData.Season, 2, out _)];
		_gameData.DayCard = (int)_today;
		_gameData.CellDay = (_date.Day -1) % 6 ;
	}
		
	private void SetStartCards()
	{
		var month = _weekCardsData.Card[_gameData.WeekCard].Month;
		var day = _weekCardsData.Card[_gameData.WeekCard].Day;
		
		SignalManager.EmitOnSetMonthCard(month.Orientation, month.Row, month.Column, _date.Month);
		SignalManager.EmitOnSetDayCard(day.Orientation, day.Row, day.Column, _date.Day);
	}

	private static int[] ShuffleCardsList(int[] array)
	{
		int count = array.Length;
		var arrayCopy = new int[count];
		Array.Copy(array, arrayCopy, count);
		while (count > 1)
		{
			int i = Random.Shared.Next(count--);
			(arrayCopy[i], arrayCopy[count]) = (arrayCopy[count], arrayCopy[i]);
		}
		return arrayCopy;
	}

	private static bool[] FlipRandomCards(bool[] array)
	{
		for (int i =0; i < array.Length; i++)
		{
			int randomNumber = Random.Shared.Next(2);
			array[i] = randomNumber == 0;
		}
		return array;
	}

	private void DealPlayCard(Control slot, string side)
	{
		var card = _deck[_nextCard];
		_card = _availableCard.Instantiate<PlayedCard>();
		_card.AddToGroup($"card{side}");
		_card.Card(0, card, _cardSide[_nextCard]);
		if (_cardSide[_nextCard])
			_card.EnableButtons(_playCardData.Cards[card].Front.Icon, card, true);
		else
			_card.EnableButtons(_playCardData.Cards[card].Back.Icon, card, false);
		_card.Position = slot.Position;
		AddChild(_card);
		GD.Print(side, ": ", card);
		_nextCard++;
	}

	private void DealNewCard(int card)
	{
		GD.Print(card);
		if (_rightCard == card)
		{
			var currentCard = GetTree().GetFirstNodeInGroup("card"+_right);
			currentCard.Free();
			_rightCard = _nextCard;
			DealPlayCard(_rightCardDisplay, _right);
		} else if (_leftCard == card)
		{
			var currentCard = GetTree().GetFirstNodeInGroup("card"+_left);
			currentCard.Free();
			_leftCard = _nextCard;
			DealPlayCard(_leftCardDisplay, _right);
		}
		else
		{
			GD.Print("error");
			return;
		}
	}
	
	private void DisplayGrid()
	{
		// little debugging
		for (int x = 0; x< _gameData.Grid.Count; x++)
		{
			for (int y = 0; y < _gameData.Grid[x].GridRow.Count; y++)
			{
				var cell = _gameData.Grid[x].GridRow[y];
				GD.Print($"Cell:{x:00}/{y:00}, Icon:{cell.Icon}, BG: {cell.Background}, GN: {cell.CardName}, Cell: {cell.CellIndex}, Locked: {cell.Locked} ");
			}
		}
		GD.Print($"Cell on Month Card: {_gameData.CellMonth+1}; Cell on Day card: {_gameData.CellDay+1}");
		GD.Print($"Week Card: {_gameData.WeekCard}; Day Card: {_gameData.DayCard}");
		GD.Print(($"Season: {_gameData.Season}"));
		GD.Print(String.Join("\n", _deck));
		GD.Print(String.Join("\n", _cardSide));
	}
}
