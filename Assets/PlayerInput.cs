using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Directly interacts with PlayerControls and sends out events / stores input info.
/// </summary>
public static class PlayerInput
{
	public static UnityEvent<bool> OnSprint = new UnityEvent<bool>();
	public static UnityEvent<bool> OnAim = new UnityEvent<bool>();
	public static UnityEvent<bool> OnFire = new UnityEvent<bool>();
	public static UnityEvent OnSwitchWeapons = new UnityEvent();
	public static UnityEvent OnReload = new UnityEvent();
	public static UnityEvent OnUseEquipment= new UnityEvent();
	public static UnityEvent OnInteract = new UnityEvent();
	public static UnityEvent OnConfirm = new UnityEvent();
	public static UnityEvent OnSelect = new UnityEvent();
	public static UnityEvent<Vector2> OnNavigate = new UnityEvent<Vector2>();

	public static Vector2 MovementInput => movementInput;
	private static Vector2 movementInput;
	public static Vector2 RotationInput => rotationInput;
	private static Vector2 rotationInput;
	public static bool UsingMouseForRotation => usingMouseForRotation;
	private static bool usingMouseForRotation;

	private static PlayerControls controls;

	public static void InitializeControls()
    {
		if (controls == null)
		{
			controls = new PlayerControls();
		}

		controls.Gameplay.Move.performed += HandleMovementInput;
        controls.Gameplay.Move.canceled += ZeroMovementInput;
        controls.Gameplay.Sprint.performed += HandleSprintPerformedInput;
        controls.Gameplay.Sprint.canceled += HandleSprintStopInput;
        controls.Gameplay.Rotate.performed += HandleRotationInput;
        controls.Gameplay.RotateMouse.performed += HandleRotationInputMouse;
        controls.Gameplay.Aim.performed += HandleAimBeginInput;
        controls.Gameplay.Aim.canceled += HandleAimEndInput;
        controls.Gameplay.Fire.started += HandleFireStartInput;
        controls.Gameplay.Fire.canceled += HandleFireStopInput;
        controls.Gameplay.SwitchWeapons.performed += HandleSwitchWeaponsInput;
        controls.Gameplay.SwitchWeaponsMouse.performed += HandleSwitchWeaponsInput;
        controls.Gameplay.Reload.performed += HandleReloadInput;
        controls.Gameplay.UseEquipment.performed += HandleUseEquipmentInput;
        controls.Gameplay.Interact.performed += HandleInteractInput;
		controls.UI.Confirm.performed += HandleConfirmInput;
		controls.UI.Select.performed += HandleSelectInput;
		controls.UI.Navigate.started += HandleNavigationInput;
	}

	private static void HandleMovementInput(InputAction.CallbackContext cntxt)
	{
		movementInput = cntxt.ReadValue<Vector2>();
	}

	private static void ZeroMovementInput(InputAction.CallbackContext cntxt)
	{
		movementInput = Vector2.zero;
	}

	private static void HandleSprintPerformedInput(InputAction.CallbackContext cntxt)
	{
		OnSprint.Invoke(true);
	}

	private static void HandleSprintStopInput(InputAction.CallbackContext cntxt)
	{
		OnSprint.Invoke(false);
		// not really sure this is necessary any more...
		// since rotation is based on the movement input when sprinting, clean it up so we don't pop back to previous rot input value.
		rotationInput = MovementInput;
	}

	private static void HandleRotationInput(InputAction.CallbackContext cntxt)
	{
		rotationInput = cntxt.ReadValue<Vector2>();
		usingMouseForRotation = false;
	}

	private static void HandleRotationInputMouse(InputAction.CallbackContext cntxt)
	{
		Vector3 mousePosition = cntxt.ReadValue<Vector2>();
		Vector3 playerPositionScreen = Camera.main.WorldToScreenPoint(GameManager.Instance.GetPlayerGO().transform.position);
		Vector3 normalizedMouseInput = (mousePosition - playerPositionScreen).normalized;
		rotationInput = normalizedMouseInput;
		usingMouseForRotation = true;
	}

	private static void HandleAimBeginInput(InputAction.CallbackContext cntxt)
	{
		OnAim.Invoke(true);
	}

	private static void HandleAimEndInput(InputAction.CallbackContext cntxt)
	{
		OnAim.Invoke(false);
	}

	private static void HandleFireStartInput(InputAction.CallbackContext cntxt)
	{
		OnFire.Invoke(true);
	}

	private static void HandleFireStopInput(InputAction.CallbackContext cntxt)
	{
		OnFire.Invoke(false);
	}

	private static void HandleSwitchWeaponsInput(InputAction.CallbackContext cntxt)
	{
		OnSwitchWeapons.Invoke();
	}

	private static void HandleReloadInput(InputAction.CallbackContext cntxt)
	{
		OnReload.Invoke();
	}

	private static void HandleUseEquipmentInput(InputAction.CallbackContext cntxt)
	{
		OnUseEquipment.Invoke();
	}

	private static void HandleInteractInput(InputAction.CallbackContext cntxt)
	{
		OnInteract.Invoke();
	}

	private static void HandleConfirmInput(InputAction.CallbackContext cntxt)
	{
		OnConfirm.Invoke();
	}

	private static void HandleSelectInput(InputAction.CallbackContext cntxt)
	{
		OnSelect.Invoke();
	}

	private static void HandleNavigationInput(InputAction.CallbackContext cntxt)
	{
		OnNavigate.Invoke(cntxt.ReadValue<Vector2>());
	}

	/// <summary>
	/// Enable gameplay controls.
	/// </summary>
	public static void EnableGameplayControls()
	{
		if (controls == null)
		{
			controls = new PlayerControls();
		}
		controls.Gameplay.Enable();
	}

	/// <summary>
	/// Disable gameplay controls.
	/// </summary>
	public static void DisableGameplayControls()
	{
		if (controls == null)
		{
			controls = new PlayerControls();
		}
		controls.Gameplay.Disable();
	}

    /// <summary>
    /// Disable gameplay controls.
    /// </summary>
    public static void EnableMenuControls()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.UI.Enable();
    }

    //	/// <summary>
    ///// Disable gameplay controls.
    ///// </summary>
    public static void DisableMenuControls()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.UI.Disable();
	}
}
