using Godot;
using System;

public partial class ScoringControl : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MouseEntered += OnLabelEntered;
		MouseExited += OnLabelExited;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private void OnLabelEntered()
	{
		SignalManager.EmitOnScoringLabelEntered();
	}

	private void OnLabelExited()
	{
		SignalManager.EmitOnScoringLabelExited();
	}
}
