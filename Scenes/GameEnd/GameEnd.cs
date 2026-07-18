using Godot;
using System;

public partial class GameEnd : CanvasLayer
{
	[Export] private Control _scoring;
	[Export] private Label _finalScore;
	[Export] private Control _notScoring;
	[Export] private Label _missingConditions;
	[Export] private GameData _gameData;
	[Export] private TextureButton _endGame;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalManager.Instance.OnFinalScoring += FinalCondition;
		_endGame.Pressed += EndGameButtonPressed;
	}

	public void FinalCondition()
	{
		Visible = true;
		string failText = "";
		if (!_gameData.DayComplete) failText = "Today's Date is NOT \nthe only date cell visible \n";
		if (!_gameData.MonthComplete) failText += "Today's Month is NOT \nthe only month cell visible \n";
		if (!_gameData.PatternComplete) failText += "There is no matching \nseasonal weather pattern \n";
		if (failText == "")
		{
			_scoring.Visible = true;
			_finalScore.Text = $"Final Score is {_gameData.FinalScore}"; // may want to add details
		}
		else
		{
			_notScoring.Visible = true;
			_missingConditions.Text = failText;
		}
		
		// need to check to see if high score table needs to be updated
		// need to create high score table
	}

	private void EndGameButtonPressed()
	{
		SignalManager.EmitOnTriggerNewGameMenu();
		Visible = false;
	}
	
}
