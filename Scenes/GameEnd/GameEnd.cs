using Godot;
using System;

public partial class GameEnd : Control
{
	[Export] private Control _scoring;
	[Export] private Label _finalScore;
	[Export] private Control _notScoring;
	[Export] private Label _missingConditions;
	[Export] private  GameData _gameData;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalManager.Instance.OnFinalScoring += FinalCondition;
	}

	public void FinalCondition()
	{
		Visible = true;
		string failText = "";
		if (!_gameData.DayComplete) failText = "Today's Date is NOT the only date cell visible \n";
		if (!_gameData.MonthComplete) failText += "Today's Month is NOT the only month cell visible \n";
		if (!_gameData.PatternComplete) failText += "There is no matching seasonal weather pattern";
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
	}
	
}
