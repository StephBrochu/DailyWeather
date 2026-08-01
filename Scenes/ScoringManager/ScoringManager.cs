using Godot;
using System;
using System.Collections.Generic;

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
		
		var card = _gameData.Season % 2 != 0
			? _dayCardData.Card[_gameData.DayCard].Front
			: _dayCardData.Card[_gameData.DayCard].Back;
		switch (card.CardScoringCondition)
		{
			case DayCardData.ScoringConditions.RainInBottomRow:
				GD.Print("Rain in Bottom Row");
				ShowBottomRow(3);
				break;
			
			case DayCardData.ScoringConditions.CloudInLargestCloudGroup:
				GD.Print("Number of Clouds in Largest Cloud Group");
				ShowLargestGroup(4);
				break;
			
			default:
				GD.Print("Condition not implemented yet");
				break;
		}
	}
	
	private void ShowBottomRow(int icon)
	{
		for (int row = (_gameData.Grid.Count-1); row >= 0; row--)
		{
			bool bottomRow = false;
			foreach (Cell cell in _gameData.Grid[row].GridRow)
			{
				if (cell.Icon == 0) continue;
				bottomRow = true;
				string name = "../" + cell.CardName;
				DragableCard card = GetNode<DragableCard>(name);
				var highlightCell = card.Cell[cell.CellIndex];
				if (cell.Icon == icon)
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

	private void ShowLargestGroup(int icon)
	{
		var allMatchingCellsList = new List<Tuple<int, int>>();
		var currentGroup = new List<Tuple<int, int>>();
		var largestGroup = new List<Tuple<int, int>>();
		
		//gather the location of all matching cells
		for (int row = 0; row < (_gameData.Grid.Count); row++)
		{
			for (int col = 0; col <_gameData.Grid[row].GridRow.Count; col++)
			{
				if (_gameData.Grid[row].GridRow[col].Icon == icon)
				{
					Tuple<int, int> coordinates = new Tuple<int, int>(row, col);
					allMatchingCellsList.Add(coordinates);
				}
			}
		}
		
		//Let's go through allMatchingCellsList and see if members are neighbours. 
		//If they are, add them to currentGroup and keep the largest
		while (allMatchingCellsList.Count != 0)
		{
			//move the top cell coordinate from AllMatchingCellList to the currentGroup list
			currentGroup.Add(allMatchingCellsList[0]);
			allMatchingCellsList.RemoveAt(0);

			//itterate through the rest of the list, checking if i
			int currentGroupCurrentItem = 0;
			int aMCLCurrentItem = 0; // currrent item from allMatchingCellsListCurrentItem
			while (currentGroupCurrentItem < currentGroup.Count)
			{
				int nextRow = currentGroup[currentGroupCurrentItem].Item1 + 1;
				int nextCol = currentGroup[currentGroupCurrentItem].Item2 + 1;
				int prevRow = currentGroup[currentGroupCurrentItem].Item1 - 1;
				int prevCol = currentGroup[currentGroupCurrentItem].Item2 + 1;
				// check if the next item on allMatchingCellList is either to the right or below the current item on currentGroup
				while (aMCLCurrentItem < allMatchingCellsList.Count)
				{
					if (allMatchingCellsList[aMCLCurrentItem].Item1 == nextRow && allMatchingCellsList[aMCLCurrentItem].Item2 == currentGroup[currentGroupCurrentItem].Item2   ||
					    allMatchingCellsList[aMCLCurrentItem].Item1 == currentGroup[currentGroupCurrentItem].Item1 && allMatchingCellsList[aMCLCurrentItem].Item2 == nextCol ||
						allMatchingCellsList[aMCLCurrentItem].Item1 == prevRow && allMatchingCellsList[aMCLCurrentItem].Item2 == currentGroup[currentGroupCurrentItem].Item2 ||
					    allMatchingCellsList[aMCLCurrentItem].Item1 == currentGroup[currentGroupCurrentItem].Item1 && allMatchingCellsList[aMCLCurrentItem].Item2 == prevCol )
					{
						currentGroup.Add(allMatchingCellsList[aMCLCurrentItem]);
						allMatchingCellsList.RemoveAt(aMCLCurrentItem);
					}
					else
					{
						aMCLCurrentItem++;
					}
				}
				currentGroupCurrentItem++;
			}

			if (currentGroup.Count > largestGroup.Count)
			{
				largestGroup.Clear();
				largestGroup.AddRange(currentGroup);
				
			}
			currentGroup.Clear();
		}
		
		// display the group
		foreach (var cell in largestGroup)
		{
			var row = cell.Item1;
			var col = cell.Item2;
			var info = _gameData.Grid[row].GridRow[col];
			string name = "../" + info.CardName;
			DragableCard card = GetNode<DragableCard>(name);
			var highlightCell = card.Cell[info.CellIndex];
			_highlightedScoringCells.Add(highlightCell);
			highlightCell.ScoringCellEnable();
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
