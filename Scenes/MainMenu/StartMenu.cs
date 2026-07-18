using Godot;
using System;

public partial class StartMenu : CanvasLayer
{
	[Export] private TextureButton _start;
	
	private DateTime _date;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SignalManager.Instance.OnTriggerNewGameMenu += ClearGameData;
		_start.Pressed += StartNewGame;
	}

	private void ClearGameData()
	{
		TableTop.Instance.NewGame(); // clear the table
		StartNewGame();
	}

	private void StartNewGame()
	{
		// this will be replaced with a menu on the start screen to allow the player to select any day they want
		_date = new DateTime(2026, 04, 27); // set up a date
		
		//_date = DateTime.Now; // today's date

		Visible = false;
		TopBar.Instance.StartNewGame(_date);
	}
}
