using Godot;
using System;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ScoringManager : Control
{
	[Export] private GameData _gameData;
	[Export] private DayConditionCards _dayCardData;
	
	private readonly List<CardCell> _highlightedConditionCells = []; // cells that are highlighted as a scoring condition
	private readonly List<CardCell> _highlightedScoringCells = []; // cells that are highlighted as scoring
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalManager.Instance.OnScoringLabelEntered += CurrentScoring;
		SignalManager.Instance.OnScoringLabelExited += HideScoringCell; // we don't need to know what the condition was since we stored the info when we highlighted the cells

		SignalManager.Instance.OnGameEnd += FinalScoring;
	}

	private void CurrentScoring()
	{
		
		var card = (Math.DivRem(_gameData.Season, 2, out _) == 0)
			? _dayCardData.Card[_gameData.DayCard].Front
			: _dayCardData.Card[_gameData.DayCard].Back;
		switch (card.CardScoringCondition)
		{
			case DayCardData.ScoringConditions.RainInBottomRow:
				GD.Print("Rain in Bottom Row");
				ShowBottomRow();
				break;
			
			default:
				GD.Print("Condition not implemented yet");
				break;
		}
	}

	private void ShowBottomRow()
	{
		for (int row = (_gameData.Grid.Count-1); row >= 0; row--)
		{
			bool bottomRow = false;
			foreach (Cell cell in _gameData.Grid[row].GridRow)
			{
				if (cell.Icon == 0) continue;
				GD.Print(cell.Icon);
				bottomRow = true;
				string name = "../" + cell.CardName;
				DragableCard card = GetNode<DragableCard>(name);
				var highlightCell = card.Cell[cell.CellIndex];
				if (cell.Icon == 3)
				{
					_highlightedScoringCells.Add(highlightCell);
					highlightCell.ScoringCellEnable();
					
				}
				else
				{
					_highlightedConditionCells.Add(highlightCell);
					highlightCell.ScoringConditionEnable();
				}
			}
			if (bottomRow) break;
		}
	}

	private void HideScoringCell()
	{
		foreach (CardCell cell in _highlightedConditionCells)
		{
			cell.ScoringConditionDisable();
		}
		_highlightedConditionCells.Clear();
		foreach (CardCell cell in _highlightedScoringCells)
		{
			cell.ScoringCellDisable();
		}
		_highlightedScoringCells.Clear();
	}

	public void FinalScoring()
	{
		// check if able to score
		CurrentScoring();
		GD.Print($"Final Score is {_highlightedScoringCells.Count}");
		_gameData.FinalScore = _highlightedScoringCells.Count;
		SignalManager.EmitOnFinalScoring();
	}
	
}
