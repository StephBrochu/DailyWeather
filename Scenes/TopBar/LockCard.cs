using Godot;

public partial class LockCard : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Disabled = true;
		SignalManager.Instance.OnLockEnabled += EnableButton;
		SignalManager.Instance.OnLockDisabled += DisableButton;
		Pressed += ButtonClicked;
	}

	private void EnableButton()
	{
		Disabled = false;
	}

	private void DisableButton()
	{
		Disabled = true;
	}
	
	private void ButtonClicked()
	{
		SignalManager.EmitOnLockCard();
	}
}
