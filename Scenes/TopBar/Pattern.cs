using Godot;
using System;

public partial class Pattern : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MouseEntered += OnLabelEntered;
		MouseExited += OnLabelExited;
	}

	private void OnLabelEntered()
	{
		SignalManager.EmitOnPatternLabelEntered();
	}

	private void OnLabelExited()
	{
		SignalManager.EmitOnPatternLabelExited();
	}
	
}
