using Godot;

public partial class SignalManager : Node
{
	public static SignalManager Instance { get; private set; }
	[Signal] public delegate void OnSetMonthCardEventHandler(float rotation, int row, int column, int anchor, int card); 
	[Signal] public delegate void OnSetDayCardEventHandler(float rotation, int row, int column, int anchor, int card);
	[Signal] public delegate void OnDealCardEventHandler();
	[Signal] public delegate void OnMouseEnteredEventHandler(int icon, int bg, Control pivot);
	[Signal] public delegate void OnMouseExitEventHandler();
	[Signal] public delegate void OnCardPlacedEventHandler(string cardOverlaid, string cellOverlaid, string newCard, string newCell);
	[Signal] public delegate void OnDebugDisplayGridEventHandler();
	[Signal] public delegate void OnRotateCardEventHandler();
	[Signal] public delegate void OnLockEnabledEventHandler();
	[Signal] public delegate void OnLockDisabledEventHandler();
	[Signal] public delegate void OnLockCardEventHandler();
	[Signal] public delegate void OnDealNextCardEventHandler(int card);
	[Signal] public delegate void OnDayCompleteEventHandler();
	[Signal] public delegate void OnMonthCompleteEventHandler();
	[Signal] public delegate void OnPatternCompleteEventHandler();
	[Signal] public delegate void OnPatternNotMatchingEventHandler();
	[Signal] public delegate void OnPatternLabelEnteredEventHandler();
	[Signal] public delegate void OnPatternLabelExitedEventHandler();
	[Signal] public delegate void OnScoringLabelEnteredEventHandler();
	[Signal] public delegate void OnScoringLabelExitedEventHandler();
	[Signal] public delegate void OnGameEndEventHandler();
	[Signal] public delegate void OnFinalScoringEventHandler();
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	public static void EmitOnSetMonthCard(float rotation, int row, int col, int anchor, int card)
	{
		Instance.EmitSignal(SignalName.OnSetMonthCard, rotation, row, col, anchor, card);
	}

	public static void EmitOnSetDayCard(float rotation, int row, int col, int anchor, int card)
	{
		Instance.EmitSignal(SignalName.OnSetDayCard, rotation, row, col, anchor, card);
	}

	public static void EmitOnDealCard()
	{
		Instance.EmitSignal(SignalName.OnDealCard);
	}
	
	public static void EmitOnMouseEntered(int icon, int bg, Control pivot)
	{
		Instance.EmitSignal(SignalName.OnMouseEntered, icon, bg, pivot);
	}

	public static void EmitOnMouseExit()
	{
		Instance.EmitSignal(SignalName.OnMouseExit);
	}

	public static void EmitOnCardPlaced(string cardOverlaid, string cellOverlaid, string newCard, string newCell)
	{
		Instance.EmitSignal(SignalName.OnCardPlaced, cardOverlaid, cellOverlaid, newCard, newCell);
	}

	public static void EmitOnDebugDisplayGrid()
	{
		Instance.EmitSignal(SignalName.OnDebugDisplayGrid);
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

	public static void EmitOnDealNextCard(int card)
	{
		Instance.EmitSignal(SignalName.OnDealNextCard, card);
	}

	public static void EmitOnDayComplete()
	{
		Instance.EmitSignal(SignalName.OnDayComplete);
	}

	public static void EmitOnMonthComplete()
	{
		Instance.EmitSignal(SignalName.OnMonthComplete);
	}

	public static void EmitOnPatternComplete()
	{
		Instance.EmitSignal(SignalName.OnPatternComplete);
	}

	public static void EmitOnPatternNotMatching()
	{
		Instance.EmitSignal(SignalName.OnPatternNotMatching);
	}

	public static void EmitOnPatternLabelEntered()
	{
		Instance.EmitSignal(SignalName.OnPatternLabelEntered);
	}
	
	public static void EmitOnPatternLabelExited()
	{
		Instance.EmitSignal(SignalName.OnPatternLabelExited);
	}

	public static void EmitOnScoringLabelEntered()
	{
		Instance.EmitSignal(SignalName.OnScoringLabelEntered);
	}

	public static void EmitOnScoringLabelExited()
	{
		Instance.EmitSignal(SignalName.OnScoringLabelExited);
	}

	public static void EmitOnGameEnd()
	{
		Instance.EmitSignal(SignalName.OnGameEnd);
	}

	public static void EmitOnFinalScoring()
	{
		Instance.EmitSignal(SignalName.OnFinalScoring);
	}
}
