using Godot;

public partial class PlayedCard : Control
{
	public static PlayedCard Instance { get; private set; }
	
	[Export] private TextureRect _cardImage;
	[Export] public Godot.Collections.Array<IconButton> Cell = new();

	[Export] private SetUpCards _setUpCardsMonth;
	[Export] private SetUpCards _setUpCardsDay;
	[Export] private DeckOfCards _cards;
	
	[Export] private GameData _gameData;

	private int _orientation;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		//SignalManager.Instance.OnRotateCard += RotateCard;
	}
	
	public void SetUpMonth(float orientation, int card)
	{
		_setUpCardsMonth.Card[card].Rotation = orientation;
		_cardImage.Texture = _setUpCardsMonth.Card[card].Front.Image;
	}

	public void SetUpDay(float orientation, int card)
	{
		_setUpCardsDay.Card[card].Rotation = orientation;
		_cardImage.Texture = _setUpCardsDay.Card[card].Front.Image; 
	}

	public void Card(float orientation, int card, bool front)
	{
		_cardImage.Texture = front switch
		{
			true => _cards.Cards[card].Front.Image,
			false => _cards.Cards[card].Back.Image
		};
		_cards.Cards[card].Rotation = orientation;
	}

	public void EnableButtons(Godot.Collections.Array<CardIcon> icons, int card, bool front)
	{
		for (int i =0; i< (Cell.Count); i++)
		{
			Cell[i].EnableButton(icons[i], card, front) ;
		}
	}

	public void EnableButtonAndSetPivot(int cell)
	{
		Cell[cell].EnablePivotButton();
		// using the position of the cell + the position of the control pivot point to find the pivot point of the card
		PivotOffset = new Vector2((Cell[cell].PivotPoint.Position.X)+(Cell[cell].Position.X), (Cell[cell].PivotPoint.Position.Y)+(Cell[cell].Position.Y));
	}

	private void RotateCard()
	{
		_orientation += 90;
		if (_orientation == 360) _orientation = 0;
		Instance.Rotation = float.DegreesToRadians(_orientation);
	}
}
