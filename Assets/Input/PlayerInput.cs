using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

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
	public static UnityEvent OnBButton = new UnityEvent();
	public static UnityEvent<Vector2> OnNavigate = new UnityEvent<Vector2>();
	public static UnityEvent OnDragStarted = new UnityEvent();
	public static UnityEvent OnDragEnded = new UnityEvent();

	public static Vector2 MovementInput => movementInput;
	private static Vector2 movementInput;
	public static Vector2 RotationInput => rotationInput;
	private static Vector2 rotationInput;
	public static bool UsingMouseForRotation => usingMouseForRotation;
	private static bool usingMouseForRotation;

	private static List<FriendlyActorController> selectedFriendlies = new List<FriendlyActorController>();
	private static AIActorController targetedActor;

	public static Vector3 dragStart;
	public static Vector3 dragEnd;

	private static PlayerControls controls;

	public static void InitializeControls()
    {
		if (controls == null)
		{
			controls = new PlayerControls();
		}

		controls.Gameplay.EnterCommandMode.performed += HandleCommandModeEnter;
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
		controls.UI.BButton.performed += HandleBButtonInput;
		controls.Command.ExitCommandMode.performed += HandleCommandModeExit;
		controls.Command.Select.performed += HandleCommandSelect;
		controls.Command.RightClick.performed += HandleCommandRightClick;
		controls.Command.Drag.performed += HandleCommandDragStart;
		controls.Command.Drag.canceled += HandleCommandDragEnd;
		controls.Command.FollowMe.performed += HandleCommandFollow;
	}

	private static void HandleCommandFollow(InputAction.CallbackContext cntxt)
    {
		selectedFriendlies.ForEach(x => x.SetMoveTarget(MissionManager.Instance.GetPlayerGO().transform));
    }

	private static void HandleCommandSelect(InputAction.CallbackContext cntxt)
    {
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		RaycastHit[] hits = Physics.RaycastAll(worldPoint, Vector3.down);
		foreach (RaycastHit hit in hits)
        {
			// only select friendly actors for now.
			AIActorController actorCont = hit.transform.gameObject.GetComponent<AIActorController>();
			if (actorCont != null && actorCont.GetActor().team == Actor.ActorTeam.Friendly)
            {
				ClearSelectedFriendlies();

				selectedFriendlies.Add(hit.transform.gameObject.GetComponent<FriendlyActorController>());

				hit.transform.gameObject.GetComponent<FriendlyActorController>().UpdateSelection(true);
            }
        }
	}

	private static void ClearSelectedFriendlies()
    {
		foreach (FriendlyActorController friendly in selectedFriendlies)
		{
			friendly.UpdateSelection(false);
		}
		selectedFriendlies.Clear();
	}

	private static void HandleCommandDragStart(InputAction.CallbackContext cntxt)
	{
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		dragStart = worldPoint;

		OnDragStarted.Invoke();

		ClearSelectedFriendlies();
	}

	private static void HandleCommandDragEnd(InputAction.CallbackContext cntxt)
	{
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		dragEnd = worldPoint;

		float rightmostValue = dragStart.x > dragEnd.x ? dragStart.x : dragEnd.x;
		float leftmostValue = dragStart.x == rightmostValue ? dragEnd.x : dragStart.x;
		float topmostValue = dragStart.z > dragEnd.z ? dragStart.z : dragEnd.z;
		float bottommostValue = dragStart.z == topmostValue ? dragEnd.z : dragStart.z;

		foreach (FriendlyActorController friendly in MissionManager.Instance.friendlyActors)
        {
			if (friendly != null)
            {
				// if friendly within selection box
				if (friendly.transform.position.x > leftmostValue && friendly.transform.position.x < rightmostValue
					&& friendly.transform.position.z > bottommostValue && friendly.transform.position.z < topmostValue)
                {
					friendly.UpdateSelection(true);
					selectedFriendlies.Add(friendly);
                }
            }
        }

		OnDragEnded.Invoke();
	}

	private static void HandleCommandRightClick(InputAction.CallbackContext cntxt)
	{
		// now for the record it would be good to have a "Selection Manager" probably. Or maybe a "commands manager".

		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		bool isTargetingSomeone = false;
		Debug.DrawRay(worldPoint, Vector3.down, Color.red);
		RaycastHit[] hits = Physics.RaycastAll(worldPoint, Vector3.down);
		foreach (RaycastHit hit in hits)
		{
			// it's important that we check the collider of the hit not the transform. For some reason, the transform
			// is a reference to teh actual actor transform not the collider transform.
			AIActorController actorTarget = hit.collider.gameObject.GetComponent<AIActorController>();
			if (actorTarget != null && actorTarget.GetActor().team == Actor.ActorTeam.Enemy)
			{
				targetedActor = hit.collider.gameObject.GetComponent<AIActorController>();
				isTargetingSomeone = true;	
			}
		}

		if (!isTargetingSomeone)
        {
			targetedActor = null;
        }

		if (selectedFriendlies != null)
		{
			if (isTargetingSomeone)
			{
				selectedFriendlies.ForEach((friendly) => friendly.SetAttackTarget(targetedActor.gameObject));
			}
			else
			{
				worldPoint.y = 0f;

				selectedFriendlies.ForEach((friendly) => friendly.MoveToPosition(worldPoint));
			}
		}
	}

	private static void HandleCommandModeExit(InputAction.CallbackContext cntxt)
	{
		controls.Command.Disable();
		controls.Gameplay.Enable();
	}

	private static void HandleCommandModeEnter(InputAction.CallbackContext cntxt)
	{
		controls.Gameplay.Disable();
		controls.Command.Enable();
	}

	private static void HandleBButtonInput(InputAction.CallbackContext cntxt)
	{
		OnBButton.Invoke();
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
		// generally I don't like having more processing in here, but it seemed appropriate.
		Vector3 mousePosition = cntxt.ReadValue<Vector2>();
		Vector3 playerPositionScreen = Camera.main.WorldToScreenPoint(MissionManager.Instance.GetPlayerGO().transform.position);
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

	public static void EnableCommandControls()
	{
		if (controls == null)
		{
			controls = new PlayerControls();
		}

		controls.Command.Enable();
	}

	public static void DisableCommandControls()
	{
		if (controls == null)
		{
			controls = new PlayerControls();
		}

		controls.Command.Disable();
	}
}
