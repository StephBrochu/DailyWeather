using Godot;

public partial class SignalManager : Node
{
	public static SignalManager Instance { get; private set; }
	[Signal] public delegate void OnSetMonthCardEventHandler(float rotation, int row, int column, int card);
	[Signal] public delegate void OnSetDayCardEventHandler(float rotation, int row, int column, int card);
	[Signal] public delegate void OnMouseEnteredEventHandler(int icon, int bg);
	[Signal] public delegate void OnMouseExitEventHandler();
	[Signal] public delegate void OnPlaceCardEventHandler(int card, bool front, int rowOffset, int colOffset);
	[Signal] public delegate void OnSelectNextCardEventHandler(int card, bool front, int rowOffset, int colOffset);
	[Signal] public delegate void OnRotateCardEventHandler();
	[Signal] public delegate void OnLockEnabledEventHandler();
	[Signal] public delegate void OnLockDisabledEventHandler();
	[Signal] public delegate void OnLockCardEventHandler();
	[Signal] public delegate void OnDealNextCardEventHandler(int card);
	
	//Debug stuff
	[Signal] public delegate void OnDebugEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	public static void EmitOnSetMonthCard(float rotation, int row, int col, int card)
	{
		Instance.EmitSignal(SignalName.OnSetMonthCard, rotation, row, col, card);
	}

	public static void EmitOnSetDayCard(float rotation, int row, int col, int card)
	{
		Instance.EmitSignal(SignalName.OnSetDayCard, rotation, row, col, card);
	}

	public static void EmitOnMouseEntered(int icon, int bg)
	{
		Instance.EmitSignal(SignalName.OnMouseEntered, icon, bg);
	}

	public static void EmitOnMouseExit()
	{
		Instance.EmitSignal(SignalName.OnMouseExit);
	}

	public static void EmitOnPlaceCard(int card, bool front, int rowOffset, int colOffset)
	{
		Instance.EmitSignal(SignalName.OnPlaceCard, card, front, rowOffset, colOffset);
	}

	public static void EmitOnSelectNextCard(int card,bool front, int rowOffset, int colOffset)
	{
		Instance.EmitSignal(SignalName.OnSelectNextCard, card, front, rowOffset, colOffset);
	}
	
	public static void EmitOnRotateCard()
	{
		Instance.EmitSignal(SignalName.OnRotateCard);
	}

	public static void EmitOnLockEnabled()
	{
		Instance.EmitSignal(SignalName.OnLockEnabled);
	}

	public static void EmitOnLockDisabled()
	{
		Instance.EmitSignal(SignalName.OnLockDisabled);
	}
	
	public static void EmitOnLockCard()
	{
		Instance.EmitSignal(SignalName.OnLockCard);
	}

	public static void EmitOnDebug()
	{
		Instance.EmitSignal(SignalName.OnDebug);
	}

	public static void EmitOnDealNextCard(int card)
	{
		Instance.EmitSignal(SignalName.OnDealNextCard, card);
	}
}
